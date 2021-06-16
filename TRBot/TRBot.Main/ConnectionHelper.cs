using System;
using System.Collections.Generic;
using TRBot.Parsing;
using TRBot.Connection;
using TRBot.Consoles;
using TRBot.VirtualControllers;
using TRBot.Data;
using TRBot.Utilities;
using TwitchLib;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using Newtonsoft.Json;

namespace TRBot.Main
{
    /// <summary>
    /// Contains helper methods regarding connections.
    /// </summary>
    public static class ConnectionHelper
    {
        /// <summary>
        /// Validates if the file for Twitch credentials exists and reads it if so.
        /// If it does not exist, it creates it and returns a newly initialized object.
        /// </summary>
        /// <returns>A TwitchLoginSettings object.</returns>
        public static TwitchLoginSettings ValidateTwitchCredentialsPresent(string dataFolderPath, string loginSettingsFilename)
        {
            TwitchLoginSettings loginSettings = null;

            //Read from the file and create it if it doesn't exist
            string text = FileHelpers.ReadFromTextFileOrCreate(dataFolderPath, loginSettingsFilename);
            
            if (string.IsNullOrEmpty(text) == true)
            {
                //Create default settings and save to the file
                loginSettings = new TwitchLoginSettings();
                string loginText = JsonConvert.SerializeObject(loginSettings, Formatting.Indented);
                FileHelpers.SaveToTextFile(dataFolderPath, loginSettingsFilename, loginText);
            }
            else
            {
                //Read from the file
                loginSettings = JsonConvert.DeserializeObject<TwitchLoginSettings>(text);
            }

            return loginSettings;
        }

        /// <summary>
        /// Creates connection credentials based on twitch login settings.
        /// </summary>
        /// <returns>A ConnectionCredentials object.</returns>
        public static ConnectionCredentials MakeCredentialsFromTwitchLogin(TwitchLoginSettings twitchLoginSettings)
        {
            return new ConnectionCredentials(twitchLoginSettings.BotName, twitchLoginSettings.Password); 
        }

        /// <summary>
        /// Validates if the file for WebSocket connection credentials exists and reads it if so.
        /// If it does not exist, it creates it and returns a newly initialized object.
        /// </summary>
        /// <returns>A WebSocketConnectSettings object.</returns>
        public static WebSocketConnectSettings ValidateWebSocketCredentialsPresent(string dataFolderPath, string loginSettingsFilename)
        {
            WebSocketConnectSettings loginSettings = null;

            //Read from the file and create it if it doesn't exist
            string text = FileHelpers.ReadFromTextFileOrCreate(dataFolderPath, loginSettingsFilename);
            
            if (string.IsNullOrEmpty(text) == true)
            {
                //Create default settings and save to the file
                loginSettings = new WebSocketConnectSettings();
                string loginText = JsonConvert.SerializeObject(loginSettings, Formatting.Indented);
                FileHelpers.SaveToTextFile(dataFolderPath, loginSettingsFilename, loginText);
            }
            else
            {
                //Read from the file
                loginSettings = JsonConvert.DeserializeObject<WebSocketConnectSettings>(text);
            }

            return loginSettings;
        }
    }
}
