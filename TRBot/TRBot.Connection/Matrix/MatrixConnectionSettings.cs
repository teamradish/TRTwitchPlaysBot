using System;

namespace TRBot.Connection
{
    /// <summary>
    /// Connection settings for Matrix.
    /// </summary>
    public record MatrixConnectionSettings
    {
        /// <summary>
        /// The URL for the Matrix homeserver.
        /// </summary>
        public string HomeServerURL { get; init; } = string.Empty;
        
        /// <summary>
        /// The ID of the room to join.
        /// </summary>
        public string RoomID { get; init; } = string.Empty;
        
        /// <summary>
        /// The account username.
        /// </summary>
        public string Username { get; init; } = string.Empty;
        
        /// <summary>
        /// The account password.
        /// </summary>
        public string Password { get; init; } = string.Empty;

        public MatrixConnectionSettings(string homeServerURL, string roomID, string username, string password)
        {
            HomeServerURL = homeServerURL;
            RoomID = roomID;
            Username = username;
            Password = password;
        }
    }
}