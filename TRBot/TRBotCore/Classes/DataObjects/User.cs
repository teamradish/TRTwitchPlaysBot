using System;
using System.Collections.Generic;
using System.Text;

namespace TRBot
{
    /// <summary>
    /// Represents a user object.
    /// </summary>
    public class User
    {
        public string Name;
        public int Level;
        public long Credits;
        public long TotalMessages;
        public long ValidInputs;
        public bool Silenced;
        public int Team;
        public int Subscriber;
        public int BetCounter;
        public bool AutoWhitelisted = false;
    }
}
