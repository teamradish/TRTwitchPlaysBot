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
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;
using System.Linq;
using TRBot.Data;
using TRBot.Logging;

namespace TRBot.Routines
{
    /// <summary>
    /// Handles bot routines.
    /// </summary>
    public class BotRoutineHandler
    {
        private readonly ConcurrentDictionary<string, BaseRoutine> BotRoutines = new ConcurrentDictionary<string, BaseRoutine>(Environment.ProcessorCount * 2, 8);

        private DataContainer DataContainer = null;

        /// <summary>
        /// Additional assemblies to look in when adding routines.
        /// This is useful if the routine's Type is outside this assembly.
        /// </summary>
        private Assembly[] AdditionalAssemblies = Array.Empty<Assembly>();

        public BotRoutineHandler()
        {

        }

        public BotRoutineHandler(Assembly[] additionalAssemblies)
        {
            AdditionalAssemblies = additionalAssemblies;
        }

        public void Initialize(DataContainer dataContainer)
        {
            DataContainer = dataContainer;

            DataContainer.DataReloader.SoftDataReloadedEvent -= OnDataReloadedSoft;
            DataContainer.DataReloader.SoftDataReloadedEvent += OnDataReloadedSoft;

            DataContainer.DataReloader.HardDataReloadedEvent -= OnDataReloadedHard;
            DataContainer.DataReloader.HardDataReloadedEvent += OnDataReloadedHard;

            PopulateRoutinesFromDB();
        }

        public void CleanUp()
        {
            DataContainer.DataReloader.SoftDataReloadedEvent -= OnDataReloadedSoft;
            DataContainer.DataReloader.HardDataReloadedEvent -= OnDataReloadedHard;

            DataContainer = null;

            CleanUpRoutines();
        }

        private void CleanUpRoutines()
        {
            foreach (KeyValuePair<string, BaseRoutine> routine in BotRoutines)
            {
                routine.Value.CleanUp();
            }
        }

        private void PopulateRoutinesFromDB()
        {
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                foreach (RoutineData routineData in context.Routines)
                {
                    AddRoutine(routineData.Name, routineData.ClassName, routineData.ValueStr, routineData.Enabled > 0, routineData.ResetOnReload > 0);
                }
            }
        }

        private void OnDataReloadedSoft()
        {
            UpdateRoutinesFromDB();
        }

        private void OnDataReloadedHard()
        {
            //Clean up routines only if they hard reload
            KeyValuePair<string, BaseRoutine>[] allRoutines = BotRoutines.ToArray();
            
            for (int i = 0; i < allRoutines.Length; i++)
            {
                KeyValuePair<string, BaseRoutine> keyValue = allRoutines[i];
                BaseRoutine baseRoutine = keyValue.Value;

                if (baseRoutine == null || baseRoutine.ResetOnReload == false)
                {
                    continue;
                }

                RemoveRoutine(keyValue.Key);
            }

            UpdateRoutinesFromDB();
        }

        private void UpdateRoutinesFromDB()
        {
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                List<string> encounteredRoutines = new List<string>(context.Routines.Count());

                foreach (RoutineData routineData in context.Routines)
                {
                    string routineName = routineData.Name;
                    if (BotRoutines.TryGetValue(routineName, out BaseRoutine baseRoutine) == true)
                    {
                        //Remove this routine if the type name is different so we can reconstruct it
                        if (baseRoutine.GetType().FullName != routineData.ClassName)
                        {
                            RemoveRoutine(routineName);
                        }

                        baseRoutine = null;
                    }

                    //Add this routine if it doesn't exist and should
                    if (baseRoutine == null)
                    {
                        //Add this routine
                        AddRoutine(routineName, routineData.ClassName, routineData.ValueStr,
                            routineData.Enabled > 0, routineData.ResetOnReload > 0 );
                    }
                    else
                    {
                        baseRoutine.Enabled = routineData.Enabled > 0;
                        baseRoutine.ResetOnReload = routineData.ResetOnReload > 0;
                        baseRoutine.ValueStr = routineData.ValueStr;
                    }

                    encounteredRoutines.Add(routineName);
                }

                //Remove routines that are no longer in the database
                foreach (string routine in BotRoutines.Keys)
                {
                    if (encounteredRoutines.Contains(routine) == false)
                    {
                        RemoveRoutine(routine);
                    }
                }
            }
        }

        public void Update(in DateTime nowUTC)
        {
            //Update routines
            foreach (BaseRoutine routine in BotRoutines.Values)
            {
                if (routine == null || routine.Enabled == false)
                {
                    continue;
                }

                routine.UpdateRoutine(nowUTC);
            }
        }

        public bool AddRoutine(string routineName, string routineTypeName, string valueStr,
            in bool routineEnabled, in bool resetOnReload)
        {
            Type routineType = Type.GetType(routineTypeName, false, true);
            if (routineType == null && AdditionalAssemblies?.Length > 0)
            {
                //Look for the type in our other assemblies
                for (int i = 0; i < AdditionalAssemblies.Length; i++)
                {
                    Assembly asm = AdditionalAssemblies[i];

                    routineType = asm.GetType(routineTypeName, false, true);
                    
                    if (routineType != null)
                    {
                        TRBotLogger.Logger.Debug($"Found \"{routineTypeName}\" in assembly \"{asm.GetName()}\"!");
                        break;
                    }
                }                
            }

            if (routineType == null)
            {
                DataContainer.MessageHandler.QueueMessage($"Cannot find routine type \"{routineTypeName}\" for routine \"{routineName}\" in all provided assemblies.");
                return false;
            }

            BaseRoutine routine = null;

            //Try to create an instance
            try
            {
                routine = (BaseRoutine)Activator.CreateInstance(routineType, Array.Empty<object>());
                routine.Enabled = routineEnabled;
                routine.ResetOnReload = resetOnReload;
                routine.ValueStr = valueStr;
            }
            catch (Exception e)
            {
                DataContainer.MessageHandler.QueueMessage($"Unable to add routine \"{routineName}\": \"{e.Message}\"");
            }

            return AddRoutine(routineName, routine);
        }

        public bool AddRoutine(string routineName, BaseRoutine routine)
        {
            if (routine == null)
            {
                TRBotLogger.Logger.Warning("Cannot add null routine.");
                return false;
            }

            //Clean up the existing routine before overwriting it with the new value
            if (BotRoutines.TryGetValue(routineName, out BaseRoutine existingRoutine) == true)
            {
                existingRoutine.CleanUp();
            }

            BotRoutines[routineName] = routine;
            routine.SetRequiredData(this, DataContainer);
            routine.Initialize();

            return true;
        }

        //public void RemoveRoutine(in int index)
        //{
        //    if (index < 0 || index >= BotRoutines.Count)
        //    {
        //        TRBotLogger.Logger.Information($"Index {index} is out of the routine count of 0 through {BotRoutines.Count}."); 
        //        return;
        //    }
        //
        //    //Clean up and remove the routine
        //    BaseRoutine routine = BotRoutines[index];
        //
        //    if (routine != null)
        //    {
        //        routine.CleanUp();
        //    }
        //
        //    BotRoutines.RemoveAt(index);
        //}

        public void RemoveRoutine(string routineName)
        {
            bool removed = BotRoutines.Remove(routineName, out BaseRoutine routine);

            routine?.CleanUp();
        }

        public void RemoveRoutine(BaseRoutine routine)
        {
            string routineName = string.Empty;
            bool found = false;

            //Look for the reference
            foreach (KeyValuePair<string, BaseRoutine> kvPair in BotRoutines)
            {
                if (kvPair.Value == routine)
                {
                    routineName = kvPair.Key;
                    found = true;
                    break;
                }
            }

            //The routine was found, so remove it
            if (found == true)
            {
                RemoveRoutine(routineName);
            }
        }

        public BaseRoutine FindRoutine(string name)
        {
            BotRoutines.TryGetValue(name, out BaseRoutine routine);
            
            return routine;
        }

        public T FindRoutine<T>() where T : BaseRoutine
        {
            foreach (BaseRoutine routine in BotRoutines.Values)
            {
                if (routine is T typeRoutine)
                {
                    return typeRoutine;
                }
            }

            return null;
        }

        //public BaseRoutine FindRoutine(Predicate<BaseRoutine> predicate)
        //{
        //    return BotRoutines.Find(predicate);
        //}
    }
}
