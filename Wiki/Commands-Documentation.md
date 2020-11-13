This page documents many common commands for TRBot. You will often use these commands either while playing on or managing a TRBot instance.

## Instructional
- [`ListCmdsCommand`](../TRBot/TRBot.Commands/Commands/ListCmdsCommand.cs) (default: "!help") - Lists all commands available at your access level in alphabetical order.
- [`VersionCommand`](../TRBot/TRBot.Commands/Commands/VersionCommand.cs) (default: "!version") - Displays the application and data version numbers of the TRBot instance you're interacting with.
- Default: "!tutorial" (internally a [`MessageCommand`](../TRBot/TRBot.Commands/Commands/MessageCommand.cs)) - Displays the tutorial message, including how to play.
- Default: "!sourcecode" (internally a [`MessageCommand`](../TRBot/TRBot.Commands/Commands/MessageCommand.cs)) - Displays where to obtain the source code for the TRBot instance you're interacting with.

## Commands
- [`AddCmdCommand`](../TRBot/TRBot.Commands/Commands/AddCmdCommand.cs) (default: "!addcmd") - Adds a new command TRBot can recognize. If the command already exists, it'll be updated.
  - Example: "!addcmd testmsg TRBot.Commands.MessageCommand This is a test message User true true"
- [`RemoveCmdCommand`](../TRBot/TRBot.Commands/Commands/RemoveCmdCommand.cs) (default: "!removecmd") - Removes a command from TRBot.
  - Example: "!removecmd testmsg"
- [`ToggleCmdCommand`](../TRBot/TRBot.Commands/Commands/ToggleCmdCommand.cs) (default: "!togglecmd") - Enables or disables a specific command from TRBot.
  - Example: "!togglecmd testmsg true"
  - Example: "!togglecmd tutorial false"

## Moderation
- [`AddRestrictedInputCommand`](../TRBot/TRBot.Commands/Commands/AddRestrictedInputCommand.cs) (default: "!restrictinput") - Restricts a user from performing a given input on a given game console for a given period of time. "null" indicates an indefinite restriction.
  - Example: "!restrictinput user1 nes a 30m" - user1 is restricted from pressing the "a" button for NES games for 30 minutes.
  - Example: "!restrictinput user2 gc l null" - user2 is restricted from pressng the "l" button and axis for GameCube games indefinitely.
- [`RemoveRestrictedInputCommand`](../TRBot/TRBot.Commands/Commands/RemoveRestrictedInputCommand.cs) (default: "!unrestrictinput") - Removes a restricted input on a user for a given game console.
  - Example: "!unrestrictinput user1 nes a"
- [`ListRestrictedInputsCommand`](../TRBot/TRBot.Commands/Commands/ListRestrictedInputsCommand.cs) (default: "!listresinputs") - Lists all restricted inputs on a given user.
  - Example: "!listresinputs user1"
- [`GlobalInputPermissionsCommand`](../TRBot/TRBot.Commands/Commands/GlobalInputPermissionsCommand.cs) (default: "!inputperms") - Obtains the global minimum access level required to perform inputs, or sets it if you have sufficient privileges and provide an argument. Only users at or above this level can perform inputs.
  - Example: "!inputperms"
  - Example: "!inputperms Admin" - Allows users only with Admin-level access and up to perform inputs.
- [`SetUserLevelCommand`](../TRBot/TRBot.Commands/Commands/SetUserLevelCommand.cs) (default: "!setlevel") - Sets a user's access level if you have sufficient privileges. You cannot set a user's access level to a value equal to or above your own.
  - Example: "!setlevel user1 VIP"
- [`UpdateUserAbilityCommand`](../TRBot/TRBot.Commands/Commands/UpdateUserAbilityCommand.cs) (default: "!toggleability") - Adds or updates a user ability on a given user for a given period of time. "null" indicate the ability is enabled or disabled for an indefinite time. This can be used to silence misbehaving users or give temporary access to a trusted individual to help moderate while you're away.
  - Example: "!toggleability user1 silenced true null 0 30d" - Silences user1 for 30 days, disallowing them from making inputs during this time.
  - Example: "!toggleability user2 duel false null 0 null" - Disables user2 from being able to duel others indefinitely.
- [`ListUserAbilitiesCommand`](../TRBot.TRBot.Commands/Commands/ListUserAbilitiesCommand.cs) (default: "!userabilities") - Lists all user abilities on a given user, including their enabled state and the expiration date, if any.
  - Example: "!userabilities user1"
- [`ListPermissionAbilitiesCommand`](../TRBot.TRBot.Commands/Commands/ListPermissionAbilitiesCommand.cs) (default: "!allabilities") - Lists all available permission abilities in the database. These are all the abilities that can be enabled or disabled on a given user.

## Input and Console-related
- [`StopAllInputsCommand`](../TRBot/TRBot.Commands/Commands/StopAllInputsCommand.cs) (default: "!stopall") - Stops all ongoing inputs on all virtual controllers. While inputs are being stopped, new inputs will not be processed. Most machines will often stop all inputs and re-enable them within 50 milliseconds or less (likely less). This command is very useful to stop long or repetitive input sequences instead of waiting for them to finish.
- [`AddConsoleCommand`](../TRBot/TRBot.Commands/Commands/AddCmdCommand.cs) (default: "!addconsole") - Adds a new game console to the database. The new console will **not** have any inputs, so you will need to add some afterwards for it to be usable.
  - Example: "!addconsole turbografx"
- [`RemoveConsoleCommand`](../TRBot/TRBot.Commands/Commands/RemoveConsoleCommand.cs) (default: "!removeconsole") - Removes a game console from the database.
  - Example: "!removeconsole turbografx"
- [`GetSetConsoleCommand`](../TRBot/TRBot.Commands/Commands/GetSetConsoleCommand.cs) (default: "!console") - Obtains the current game console in use, or sets it if you have sufficient privileges and provide an argument.
  - Example: "!console"
  - Example: "!console ps4" - Sets the current console to ps4. 
- [`AddInputCommand`](../TRBot/TRBot.Commands/Commands/AddInputCommand.cs) (default: "!addinput") - Adds a new input to a given game console with a name, button value, axis value, input type, and min/max axis range. If the input already exists, it'll be updated with the new information.
  - Example: "!addinput nes spareinput1 8 0 1 0 0 100"
  - Example: "!addinput genesis blankinput 0 0 0 0 0 0"
  - Example: "!addinput gc l 5 0 3 0 1 99" - This is how to add the "L" button on the Nintendo GameCube controller, which acts as an axis until it is 100% pressed in, in which it is a button. The last value indicates the max range (0 - 100) the axis can be pressed before it's considered a button.
- [`RemoveInputCommand`](../TRBot/TRBot.Commands/Commands/RemoveInputCommand.cs) (default: "!removeinput") - Removes an input from a given game console.
  - Example: "!removeinput nes spareinput1"
- [`SetInputLevelCommand`](../TRBot/TRBot.Commands/Commands/SetInputLevelCommand.cs) (default: "!setinputlevel") - Sets the access level for a specific input on a given game console.
  - Example: "!setinputlevel gba r Moderator"
- [`SetInputEnabledCommand`](../TRBot/TRBot.Commands/Commands/SetInputEnabledCommand.cs) (default: "!toggleinput") - Enables or disables a specific input on a given game console.
  - Example: "!toggleinput gba b false"
  - Example: "!toggleinput wii c true"
- [`InputInfoCommand`](../TRBot/TRBot.Commands/Commands/InputInfoCommand.cs) (default: "!inputs") - Displays all available inputs for a given game console, or displays information about a specific input on a given game console.
  - Example: "!inputs" - Displays all inputs for the current console.
  - Example: "!inputs ps2" - Displays all inputs for the ps2 console.
  - Example: "!inputs snes y" - Displays detailed input data for the snes console's "y" input.
- [`AddInputSynonymCommand`](../TRBot/TRBot.Commands/Commands/AddInputSynonymCommand.cs) (default: "!addsyn") - Adds an input synonym to a given input on a given game console. If the input synonym already exists, it'll be updated.
  - Example: "!addsyn n64 . #" - This causes all instances of "." in the input sequence to be replaced with "#".
- [`RemoveInputSynonymCommand`](../TRBot/TRBot.Commands/Commands/RemoveInputSynonymCommand.cs) (default: "!removesyn") - Removes an input synonym from a given input on a given game console.
  - Example: "!removesyn n64 ."
- [`ListInputSynonymsCommand`](../TRBot/TRBot.Commands/Commands/ListInputSynonymsCommand.cs) (default: "!listsyn") - Lists all available input synonyms for a given game console.
  - Example: "!listsyn ps2"
- [`AddMacroCommand`](../TRBot/TRBot.Commands/Commands/AddMacroCommand.cs) (default: "!addmacro") - Adds an input macro for the given input sequence. If the macro already exists, it'll be updated. Input macros must begin with "#".
  - Example: "!addmacro #slide _down34ms a -down34ms"
  - Example: "!addmacro #mash(\*) [<0>34ms #34ms]\*20"
- [`RemoveMacroCommand`](../TRBot/TRBot.Commands/Commands/RemoveMacroCommand.cs) (default: "!removemacro") - Removes an input macro.
  - Example: "!removemacro #slide"
- [`ShowMacroCommand`](../TRBot/TRBot.Commands/Commands/ShowMacroCommand.cs) (default: "!showmacro") - Shows the input sequence for a given input macro.
  - Example: "!showmacro #slide"
  - Example: "!showmacro #mash(\*)"
- [`ListMacrosCommand`](../TRBot/TRBot.Commands/Commands/ListMacrosCommand.cs) (default: "!macros") - Lists all available input macros.
- [`DefaultInputDurCommand`](../TRBot/TRBot.Commands/Commands/DefaultInputDurCommand.cs) (default: "!defaultinputdur") - Obtains the global default input duration, or sets it if you have sufficient privileges and provide an argument.
  - Example: "!defaultinputdur"
  - Example: "!defaultinputdur 500" - Sets the global default input duration to 500 milliseconds.
- [`MaxInputDurCommand`](../TRBot/TRBot.Commands/Commands/MaxInputDurCommand.cs) (default: "!maxinputdur") - Obtains the global max input duration, or sets it if you have sufficient privileges and provide an argument.
  - Example: "!maxinputdur"
  - Example: "!maxinputdur 120000" - Sets the global max input duration to 2 minutes.
- [`ControllerPortCommand`](../TRBot/TRBot.Commands/Commands/ControllerPortCommand.cs) (default: "!port") - Obtains or sets your controller port. Controller ports start at 1 and cannot exceed the number of controllers plugged in.
  - Example: "!port"
  - Example: "!port 2"
- [`ControllerCountCommand`](../TRBot/TRBot.Commands/Commands/ControllerCountCommand.cs) (default: "!controllercount") - Obtains the number of controllers plugged into the current virtual controller, or sets the count if you have sufficient privileges and provide an argument. Changing the controller count will stop all ongoing inputs and reinitialize all controllers.
- [`ListPressedInputsCommand`](../TRBot/TRBot.Commands/Commands/ListPressedInputsCommand.cs) (default: "!pressedinputs") - Lists all inputs currently pressed on a given virtual controller. By default, this uses your own controller port, but you can supply another port as an argument.
  - Example: "!pressedinputs 1" - Lists all pressed inputs on controller port 1.
  - Example: "!pressedinputs 2" - Lists all pressed inputs on controller port 2

## Game Progress/Logging
- [`AddGameLogCommand`](../TRBot/TRBot.Commands/Commands/AddGameLogCommand.cs) (default: "!addlog") - Adds a time-stamped game log to the database. This log is used to indicate others of game progress.
  - Example: "!addlog Beat King Bob-Omb! Currently at 8 Power Stars!"
- [`ViewGameLogCommand`](../TRBot/TRBot.Commands/Commands/ViewGameLogCommand.cs) (default: "!viewlog") - Views a game log. You can supply an argument for how many logs back to check, with higher values being older. If the user who created this log is currently opted into bot stats, it will also display their so other users can ask them for more details about the log.
  - Example: "!viewlog" - Displays the most recent game log.
  - Example: "!viewlog 5" - Displays the fifth most recent game log.
- [`SetGameMessageCommand`](../TRBot/TRBot.Commands/Commands/SetGameMessageCommand.cs) (default: "!setmessage") - Sets a game message that can be displayed on stream if the streamer provided it. This is useful for informing others of the current objective in a game.
  - Example: "!setmessage Beat Phantom Ganon"

## Games/Fun
- [`AddMemeCommand`](../TRBot/TRBot.Commands/Commands/AddMemeCommand.cs) (default: "!addmeme") - Adds a meme to the database. If the meme already exists, it'll be updated.
  - Example: "!addmeme lol Kappa"
- [`RemoveMemeCommand`](../TRBot/TRBot.Commands/Commands/RemoveMemeCommand.cs) (default: "!removememe") - Removes a meme from the database.
  - Example: "!removememe lol"
- [`ListMemesCommand`](../TRBot/TRBot.Commands/Commands/ListMemesCommand.cs) (default: "!memes") - Lists all memes.
- [`CreditsCommand`](../TRBot/TRBot.Commands/Commands/DuelCommand.cs) (default: "!credits") - Lists the number of credits you have, or optionally the number of credits another user has.
  - Example: "!credits"
  - Example: "!credits user1"
- [`BetCreditsCommand`](../TRBot/TRBot.Commands/Commands/ListMemesCommand.cs) (default: "!bet") - Bets credits for a chance to win double your bet.
  - Example: "!bet 500"
- [`DuelCommand`](../TRBot/TRBot.Commands/Commands/DuelCommand.cs) (default: "!duel") - Challenges another user to a duel for credits, or accepts/denies a duel if you've been challenged.
  - Example: "!duel user1 500"
  - Example: "!duel accept"
  - Example: "!duel deny"
- [`EnterGroupBetCommand`](../TRBot/TRBot.Commands/Commands/EnterGroupBetCommand.cs) (default: "!groupbet") - Enters the group bet with a given bet. If you're already in the group bet, this will adjust your bet to the new value. If you entered the group bet and there are now enough participants, the group bet will begin.
  - Example: "!groupbet 500
- [`LeaveGroupBetCommand`](../TRBot/TRBot.Commands/Commands/LeaveGroupBetCommand.cs) (default: "!exitgroupbet") - Leaves the group bet. If there are no longer enough participants for the group bet after leaving, the group bet will be cancelled.
