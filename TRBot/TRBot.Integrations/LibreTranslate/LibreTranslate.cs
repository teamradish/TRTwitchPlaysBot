/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, version 3 of the License.
 *
 * TRBot is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with TRBot.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TRBot.Utilities;

namespace TRBot.Integrations.LibreTranslate
{
    /// <summary>
    /// Facilitates communicating with the LibreTranslate API.
    /// <summary>
    public class LibreTranslate
    {
        private const string LANGUAGES_API = "languages";
        private const string TRANSLATE_API = "translate";

        //Allow easy lookup to and from codes and names
        public readonly Dictionary<TranslateLanguageCode, string> AvailableLanguageCodes = null;
        public readonly Dictionary<string, TranslateLanguageCode> AvailableLanguageNames = null;

        public readonly string ServerURL = string.Empty;

        public LibreTranslate(string serverURL)
        {
            ServerURL = serverURL;

            AvailableLanguageCodes = new Dictionary<TranslateLanguageCode, string>(EnumUtility.GetEnumLength<TranslateLanguageCode>());
            AvailableLanguageNames = new Dictionary<string, TranslateLanguageCode>(EnumUtility.GetEnumLength<TranslateLanguageCode>());
        }

        public async Task PopulateAvailableLanguages()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(5);

                HttpResponseMessage responseMsg = await httpClient.GetAsync($"{ServerURL}/{LANGUAGES_API}");

                //Unsuccessful code
                if (responseMsg.IsSuccessStatusCode == false)
                {
                    throw new Exception($"Error with response at \"{ServerURL}/{LANGUAGES_API}\" - {responseMsg.StatusCode}: {responseMsg.ReasonPhrase}");
                }

                string supportedLangJson = await responseMsg.Content.ReadAsStringAsync();

                SupportedLanguage[] supportedLangResponse = JsonConvert.DeserializeObject<SupportedLanguage[]>(supportedLangJson);

                for (int i = 0; i < supportedLangResponse.Length; i++)
                {
                    SupportedLanguage supported = supportedLangResponse[i];

                    if (Enum.TryParse<TranslateLanguageCode>(supported.code, true,
                        out TranslateLanguageCode supportedLang) == true)
                    {
                        //Fill the dictionaries
                        //Lower the language names
                        AvailableLanguageCodes[supportedLang] = supported.name;
                        AvailableLanguageNames[supported.name.ToLowerInvariant()] = supportedLang;
                    }
                }
            }
        }

        public async Task<string> TranslateText(TranslateLanguageCode sourceLang, TranslateLanguageCode targetLang,
            string textToTranslate)
        {
            TranslateRequest translateRequest = new TranslateRequest(textToTranslate,
                sourceLang.ToString(), targetLang.ToString());
            
            string requestJson = JsonConvert.SerializeObject(translateRequest, Formatting.None);

            using (HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{ServerURL}/{TRANSLATE_API}"))
            {
                httpRequest.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(5);

                    HttpResponseMessage responseMsg = await httpClient.SendAsync(httpRequest);

                    if (responseMsg.IsSuccessStatusCode == false)
                    {
                        throw new Exception($"Error with response at \"{ServerURL}/{TRANSLATE_API}\" - {responseMsg.StatusCode}: {responseMsg.ReasonPhrase}");
                    }

                    string responseJson = await responseMsg.Content.ReadAsStringAsync();

                    TranslateResponse response = JsonConvert.DeserializeObject<TranslateResponse>(responseJson);
                    
                    return response.translatedText;
                }
            }
        }
    }
}
