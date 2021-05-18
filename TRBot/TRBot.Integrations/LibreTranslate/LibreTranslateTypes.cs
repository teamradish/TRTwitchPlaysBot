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
using System.Net;

namespace TRBot.Integrations.LibreTranslate
{
    /// <summary>
    /// Represents a translate request to the HTTP server.
    /// </summary>
    internal class TranslateRequest
    {
        /// <summary>
        /// The text to translate.
        /// </summary>
        public string q = string.Empty;

        /// <summary>
        /// A string code representing the source language.
        /// </summary>
        public string source = string.Empty;

        /// <summary>
        /// A string code representing the target language to translate to.
        /// </summary>
        public string target = string.Empty;

        public TranslateRequest()
        {

        }

        public TranslateRequest(string textToTranslate, string sourceLang, string targetLang)
        {
            q = textToTranslate;
            source = sourceLang;
            target = targetLang;
        }
    }

    /// <summary>
    /// Represents a translate response from the HTTP server.
    /// </summary>
    internal class TranslateResponse
    {
        /// <summary>
        /// The output text translated to the target language.
        /// </summary>
        public string translatedText = string.Empty;

        public TranslateResponse()
        {

        }

        public TranslateResponse(string textTranslated)
        {
            translatedText = textTranslated;
        }
    }

    /// <summary>
    /// Represents a supported language from the HTTP server.
    /// </summary>
    internal class SupportedLanguage
    {
        /// <summary>
        /// The language code.
        /// </summary>
        public string code = string.Empty;

        /// <summary>
        /// The display name of the language.
        /// </summary>
        public string name = string.Empty;

        public SupportedLanguage()
        {

        }

        public SupportedLanguage(string langCode, string langName)
        {
            code = langCode;
            name = langName;
        }
    }

    /// <summary>
    /// A exception that is thrown when a host is unreachable.
    /// </summary>
    internal class UnreachableHostException : Exception
    {
        public UnreachableHostException()
        {
            
        }

        public UnreachableHostException(string message)
            : base(message)
        {

        }

        public UnreachableHostException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
