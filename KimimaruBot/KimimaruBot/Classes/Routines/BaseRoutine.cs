using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;

namespace KimimaruBot
{
    public abstract class BaseRoutine
    {
        public BaseRoutine()
        {

        }

        public virtual void Initialize(in TwitchClient client)
        {

        }

        public virtual void CleanUp(in TwitchClient client)
        {

        }

        public abstract void UpdateRoutine(in TwitchClient client, in DateTime currentTime);
    }
}
