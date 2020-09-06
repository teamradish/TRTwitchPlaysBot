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

namespace TRBot.Common
{
    /// <summary>
    /// Handles bot routines.
    /// </summary>
    public class BotRoutineHandler
    {
        private readonly List<BaseRoutine> BotRoutines = new List<BaseRoutine>(8);

        public BotRoutineHandler()
        {

        }

        public void CleanUp()
        {
            for (int i = 0; i < BotRoutines.Count; i++)
            {
                BotRoutines[i].CleanUp();
            }
        }

        public void Update(in DateTime now)
        {
            //Update routines
            for (int i = 0; i < BotRoutines.Count; i++)
            {
                if (BotRoutines[i] == null)
                {
                    continue;
                }

                BotRoutines[i].UpdateRoutine(now);
            }
        }

        public void AddRoutine(BaseRoutine routine)
        {
            routine.Initialize();
            BotRoutines.Add(routine);
        }

        public void RemoveRoutine(string identifier)
        {
            BaseRoutine routine = FindRoutine(identifier, out int index);
            if (index >= 0)
            {
                if (routine != null)
                {
                    routine.CleanUp();
                }
                BotRoutines.RemoveAt(index);
            }
        }

        public void RemoveRoutine(BaseRoutine routine)
        {
            routine.CleanUp();
            BotRoutines.Remove(routine);
        }

        public BaseRoutine FindRoutine(string identifier, out int indexFound)
        {
            for (int i = 0; i < BotRoutines.Count; i++)
            {
                BaseRoutine routine = BotRoutines[i];
                if (routine.Identifier == identifier)
                {
                    indexFound = i;
                    return routine;
                }
            }

            indexFound = -1;
            return null;
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
