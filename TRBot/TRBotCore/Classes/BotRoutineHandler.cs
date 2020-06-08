/* This file is part of TRBot.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
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
using System.Collections.Generic;
using System.Text;

namespace TRBot
{
    /// <summary>
    /// Handles bot routines.
    /// </summary>
    public class BotRoutineHandler
    {
        private readonly List<BaseRoutine> BotRoutines = new List<BaseRoutine>(8);

        private IClientService ClientService = null;

        public BotRoutineHandler(IClientService clientService)
        {
            SetClientService(clientService);
        }

        public void CleanUp()
        {
            for (int i = 0; i < BotRoutines.Count; i++)
            {
                BotRoutines[i].CleanUp(ClientService);
            }
        }

        public void SetClientService(IClientService clientService)
        {
            ClientService = clientService;
        }

        public void Update(in DateTime now)
        {
            //Update routines
            for (int i = 0; i < BotRoutines.Count; i++)
            {
                if (BotRoutines[i] == null)
                {
                    //Console.WriteLine($"NULL BOT ROUTINE AT {i} SOMEHOW!!");
                    continue;
                }

                BotRoutines[i].UpdateRoutine(ClientService, now);
            }
        }

        public void AddRoutine(BaseRoutine routine)
        {
            routine.Initialize(ClientService);
            BotRoutines.Add(routine);
        }

        public void RemoveRoutine(BaseRoutine routine)
        {
            routine.CleanUp(ClientService);
            BotRoutines.Remove(routine);
        }

        public BaseRoutine FindRoutine<T>()
        {
            return BotRoutines.Find((routine) => routine is T);
        }

        public BaseRoutine FindRoutine(Predicate<BaseRoutine> predicate)
        {
            return BotRoutines.Find(predicate);
        }
    }
}
