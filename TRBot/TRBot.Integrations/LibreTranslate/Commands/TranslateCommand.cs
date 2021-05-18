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
using System.Threading;
using System.Threading.Tasks;
using TRBot.Data;
using TRBot.Commands;
using TRBot.Connection;
using TRBot.Logging;

namespace TRBot.Integrations.LibreTranslate
{
    /// <summary>
    /// A command that translates text on a LibreTranslate instance.
    /// </summary>
    public class TranslateCommand : BaseCommand
    {
        private const string DEFAULT_TRANSLATE_URL = "http://127.0.0.1:5000";

        //Configuration for the connection
        private string ConnectURL = DEFAULT_TRANSLATE_URL;

        private string CachedLanguagesStr = string.Empty;

        private LibreTranslate Translation = null;

        private bool TranslationInitialized = false;

        public TranslateCommand()
        {

        }

        public override void Initialize()
        {
            base.Initialize();

            //Get database value for the URL
            ConnectURL = DataHelper.GetSettingString(LibreTranslateSettingsConstants.LIBRETRANSLATE_URL, DEFAULT_TRANSLATE_URL);

            Translation = new LibreTranslate(ConnectURL);

            InitializeTranslation();
        }

        private async void InitializeTranslation()
        {
            try
            {
                await Translation.PopulateAvailableLanguages();
            }
            catch (Exception e)
            {
                string message = $"Issue connecting to translation service - {e.Message}";

                QueueMessage(message);

                Translation = null;
                return;
            }
            finally
            {
                TranslationInitialized = true;
            }

            InitializeLangMessage();
        }

        public override void CleanUp()
        {
            Translation = null;
            TranslationInitialized = false;

            base.CleanUp();
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            //Translation not finished initializing
            if (TranslationInitialized == false)
            {
                QueueMessage("Please wait for the translation to finish initializing!");
                return;
            }

            //Validate arguments
            List<string> argList = args.Command.ArgumentsAsList;
            string argStr = args.Command.ArgumentsAsString;

            if (argList.Count < 2)
            {
                QueueMessage($"Please specify a source language to translate from and a target language to translate to: {CachedLanguagesStr}");
                return;
            }

            if (Translation == null)
            {
                QueueMessage("Translation service not initialized. This could be due to an invalid URL or a local server not properly set up. Please hard reload settings after fixing the configuration.");
                return;
            }

            string langSourceArg = argList[0].ToLowerInvariant();
            string langTargetArg = argList[1].ToLowerInvariant();

            //Invalid source language
            if (Translation.AvailableLanguageNames.TryGetValue(langSourceArg, out TranslateLanguageCode langSource) == false)
            {
                QueueMessage($"Source language \"{langSourceArg}\" is an invalid language. Please specify a valid language: {CachedLanguagesStr}");
                return;
            }

            //Invalid target language
            if (Translation.AvailableLanguageNames.TryGetValue(langTargetArg, out TranslateLanguageCode langTarget) == false)
            {
                QueueMessage($"Target language \"{langTargetArg}\" is an invalid language. Please specify a valid language: {CachedLanguagesStr}");
                return;
            }

            int translateMsgStartIndex = langSourceArg.Length + langTargetArg.Length + 2;

            string msgToTranslate = string.Empty;

            if (argStr.Length > translateMsgStartIndex)
            {
                msgToTranslate = argStr.Substring(translateMsgStartIndex);
            }

            //No arguments
            if (string.IsNullOrEmpty(msgToTranslate) == true)
            {
                QueueMessage("Please enter a message to translate!");
                return;
            }

            //Translate!
            PerformTranslation(langSource, langTarget, msgToTranslate);
        }

        private async void PerformTranslation(TranslateLanguageCode sourceLang, TranslateLanguageCode targetLang,
            string textToTranslate)
        {
            //Translate!
            try
            {
                string translatedMsg = await Translation.TranslateText(sourceLang, targetLang, textToTranslate);

                QueueMessage(translatedMsg);
            }
            catch (Exception e)
            {
                QueueMessage($"Error translating from language \"{sourceLang}\" to \"{targetLang}\" - {e.Message}");
                return;
            }
        }

        private void InitializeLangMessage()
        {
            //Build the message
            StringBuilder strBuilder = new StringBuilder(256);

            foreach (KeyValuePair<string, TranslateLanguageCode> kvPair in Translation.AvailableLanguageNames)
            {
                strBuilder.Append('"').Append(kvPair.Key).Append('"').Append(',').Append(' ');
            }

            if (strBuilder.Length > 1)
            {
                strBuilder.Remove(strBuilder.Length - 2, 2);

                CachedLanguagesStr = strBuilder.ToString();
            }
        }
    }
}
