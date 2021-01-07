/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRBot.Connection;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.Parsing;
using TRBot.Data;
using TRBot.Permissions;
using TRBot.Logging;

namespace TRBot.Commands
{
    /// <summary>
    /// Adds a user ability.
    /// </summary>
    public sealed class UpdateUserAbilityCommand : BaseCommand
    {
        private const string NULL_EXPIRATION_ARG = "null";
        private string UsageMessage = "Usage: \"username\", \"ability name\", \"enabled state (bool)\" (\"ability value_str (string)\", \"ability value_int (int)\", \"expiration from now (Ex. 30 ms/s/m/h/d) - \"null\" for no expiration\") (optional group)";

        public UpdateUserAbilityCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count != 3 && arguments.Count != 6)
            {
                QueueMessage(UsageMessage);
                return;
            }

            //Get the user calling this
            string thisUserName = args.Command.ChatMessage.Username.ToLowerInvariant();

            string abilityUserName = arguments[0].ToLowerInvariant();
            string abilityName = arguments[1].ToLowerInvariant();
            string abilityEnabledStr = arguments[2].ToLowerInvariant();

            User thisUser = DataHelper.GetUser(thisUserName);
            User abilityUser = DataHelper.GetUser(abilityUserName);

            if (abilityUser == null)
            {
                QueueMessage("This user doesn't exist in the database!");
                return;
            }

            PermissionAbility permAbility = null;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                permAbility = context.PermAbilities.FirstOrDefault(p => p.Name == abilityName);
            }

            if (permAbility == null)
            {
                QueueMessage($"There is no permission ability named \"{abilityName}\".");
                return;
            }

            if (thisUser.Name != abilityUser.Name && thisUser.Level <= abilityUser.Level)
            {
                QueueMessage("You cannot modify abilities for other users with levels greater than or equal to yours!");
                return;
            }

            if ((long)permAbility.AutoGrantOnLevel > thisUser.Level)
            {
                QueueMessage("This ability isn't available to your level, so you cannot grant it!");
                return;
            }

            if ((long)permAbility.MinLevelToGrant > thisUser.Level)
            {
                QueueMessage($"You need to be at least level {(long)permAbility.MinLevelToGrant}, {permAbility.MinLevelToGrant}, to grant this ability.");
                return;
            }

            if (bool.TryParse(abilityEnabledStr, out bool enabledState) == false)
            {
                QueueMessage("Invalid enabled state.");
                return;
            }

            string valueStrArg = string.Empty;
            string valueIntArg = string.Empty;
            string expirationArg = string.Empty;
            int valueInt = 0;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                UserAbility newUserAbility = null;

                abilityUser = DataHelper.GetUserNoOpen(abilityUserName, context);
                abilityUser.TryGetAbility(abilityName, out newUserAbility);

                bool shouldAdd = false;

                if (newUserAbility == null)
                {
                    //TRBotLogger.Logger.Information($"New ability {abilityName}");

                    newUserAbility = new UserAbility();
                    shouldAdd = true;
                }

                newUserAbility.PermabilityID = permAbility.ID;
                newUserAbility.UserID = abilityUser.ID;
                newUserAbility.GrantedByLevel = thisUser.Level;
                newUserAbility.SetEnabledState(enabledState);

                //If there are only three arguments, enable/disable the ability
                if (arguments.Count == 3)
                {
                    if (shouldAdd == true)
                    {
                        //TRBotLogger.Logger.Information($"Adding ability {abilityName}");
                        abilityUser.UserAbilities.Add(newUserAbility);
                    }

                    if (enabledState == true)
                    {
                        QueueMessage($"Enabled the \"{abilityName}\" ability for {abilityUser.Name}!");
                    }
                    else
                    {
                        QueueMessage($"Disabled the \"{abilityName}\" ability for {abilityUser.Name}!");
                    }

                    //Save and exit here
                    context.SaveChanges();

                    return;
                }

                valueStrArg = arguments[3];
                valueIntArg = arguments[4];
                expirationArg = arguments[5].ToLowerInvariant();

                newUserAbility.ValueStr = valueStrArg;

                //Validate arguments
                if (int.TryParse(valueIntArg, out valueInt) == false)
                {
                    QueueMessage("Invalid value_int argument.");
                    return;
                }

                newUserAbility.ValueInt = valueInt;

                if (expirationArg == NULL_EXPIRATION_ARG)
                {
                    newUserAbility.Expiration = null;
                }
                else
                {
                    if (Helpers.TryParseTimeModifierFromStr(expirationArg, out TimeSpan timeFromNow) == false)
                    {
                        QueueMessage("Unable to parse expiration time from now.");
                        return;
                    }

                    //Set the time to this amount from now
                    newUserAbility.Expiration = DateTime.UtcNow + timeFromNow;
                }

                //Add if we should
                if (shouldAdd == true)
                {
                    abilityUser.UserAbilities.Add(newUserAbility);
                }

                //Save changes
                context.SaveChanges();
            }

            if (enabledState == true)
            {
                QueueMessage($"Enabled the \"{abilityName}\" ability to {abilityUser.Name} with values ({valueStrArg}, {valueInt}) and expires in {expirationArg}!");
            }
            else
            {
                QueueMessage($"Disabled the \"{abilityName}\" ability on {abilityUser.Name} with values ({valueStrArg}, {valueInt}) and expires in {expirationArg}!");
            }
        }
    }
}
