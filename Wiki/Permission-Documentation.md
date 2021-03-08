This page documents the permissions system for TRBot.

## Durations
Several aspects of the permissions system can be applied either indefinitely or for a period of time. Here are the modifiers available:

- "null" - Indefinite duration
- "ms" - Duration in milliseconds (Ex. "500ms")
- "s" - Duration in seconds (Ex. "10s")
- "m" - Duration in minutes (Ex. "30m")
- "h" - Duration in hours (Ex. "4h")
- "d" - Duration in days (Ex. "75d")

## User-specific Restricted Inputs
TRBot supports restricting specific users from making specific inputs. Each user has a set of restricted inputs, which can be viewed in the **RestrictedInputs** table of the database. Restricted inputs are console-specific and can last either indefinitely or for a specific amount of time.

- You can view a user's restricted inputs with the [`ListRestrictedInputsCommand`](../TRBot/TRBot.Commands/Commands/ListRestrictedInputsCommand.cs) (Default: "!listresinputs").
- You can add a restricted input with the [`AddRestrictedInputCommand`](../TRBot/TRBot.Commands/Commands/AddRestrictedInputCommand.cs) (Default: "!restrictinput").
- You can lift an input restriction input with the [`RemoveRestrictedInputCommand`](../TRBot/TRBot.Commands/Commands/RemoveRestrictedInputCommand.cs) (Default: "!unrestrictinput").

**Example:** "!restrictinput user1 gc a 30m" - Prevents user1 from pressing the "a" button on the GameCube console for 30 minutes.

**Example:** "!restrictinput user2 ps2 square null" - Prevents user2 from pressing the "square" button on the PlayStation 2 console indefinitely.

**Example:** "!unrestrictinput user2 ps2 square" - Lifts the restriction on user2 pressing the "square" button on the PlayStation 2.

## Access Level Overview
Each user has an access level. The access levels are as follows:

- 0 (User)
- 10 (Whitelisted)
- 20 (VIP)
- 30 (Moderator)
- 40 (Admin)
- 50 (Superadmin)

Each user starts out at level 0 (User) by default. They can be auto-promoted to another level if they perform enough valid inputs based on the [auto promote settings](./Settings-Documentation.md#auto_promote_enabled) configured in the database. Otherwise, users can be promoted by Moderators and up, but not up to their own level. For example, an Admin can change a user's level to Moderator and below, but not up to Admin. Likewise, a Superadmin can promote anyone to Admin and below, but cannot promote anyone else to Superadmin.

To view your own access level or another user's access level, use the [`LevelCommand`](../TRBot/TRBot.Commands/Commands/LevelCommand.cs) (Default: "!level"), and to set a user's access level, use the [`SetUserLevelCommand`](../TRBot/TRBot.Commands/Commands/SetUserLevelCommand.cs) (Default: "!setlevel").

It's recommended for the streamer to set their own level to 50 (Superadmin) in the database, then update their own abilities with the [`UpdateAllUserAbilitiesCommand`](../TRBot/TRBot.Commands/Commands/UpdateAllUserAbilitiesCommand.cs) (Default: "!updateabilities").

## Ability Overview
Abilities are a large part of the moderation features TRBot provides. These abilities are another layer on top of the access level system, and they define what users can and cannot do.

- `PermissionAbilities` are static and simply define all the available abilities. They are stored in the **PermissionAbilities** table of the database. You can list all available `PermissionAbilities` with the [`ListPermissionAbilitiesCommand`](../TRBot/TRBot.Commands/Commands/ListPermissionAbilitiesCommand.cs) (Default: "!allabilities").
- `UserAbilities` are specific to each user and can be enabled or disabled either indefinitely or for a period of time. They are stored in the **UserAbilities** table of the database. You can list all abilities on a user with the [`ListUserAbilitiesCommand`](../TRBot/TRBot.Commands/Commands/ListUserAbilitiesCommand.cs) (Default: "!userabilities").

The ability system is robust and flexible. For example, if you want to prevent a troll from making any inputs to chat, you can enable the [`silenced`](#silenced) ability on them for 30 minutes.

To enable or disable abilities on a user, use the [`UpdateUserAbilityCommand`](../TRBot/TRBot.Commands/Commands/UpdateUserAbilityCommand.cs) (Default: "!toggleability").

Each access level has different commands available to them, determined by the **CommandData** table in the database. If a user attempts to perform a command without a sufficient access level, they will be denied.

Each access level also has different abilities available to them by default, which will be outlined below. Upon changing levels, a user's abilities will be automatically adjusted. Users can also update their own abilities with the [`UpdateAllUserAbilitiesCommand`](../TRBot/TRBot.Commands/Commands/UpdateAllUserAbilitiesCommand.cs) (Default: "!updateabilities") to obtain all abilities they should have at their level and remove any that they shouldn't. It's recommended to do this after changing a user's access level directly through the database. This will **not** cleanse a user of abilities enabled/disabled from a higher-leveled user than themselves, meaning a troll can't use this command to remove their silence.

## All Abilities

### silenced
Determines if the user can perform inputs. If enabled, the user cannot perform inputs.

### userdefaultinputdur
Determines the user's specific default input duration.

### usermaxinputdur
Determines the user's specific maximum input sequence duration.

### usermidinputdelay
Determines the user's specific mid input delay duration.

### bet
**Default level: User (0)**

Determines if the user can bet with the [`BetCreditsCommand`](../TRBot/TRBot.Commands/Commands/BetCreditsCommand.cs).

### duel
**Default level: User (0)**

Determines if the user can duel other users with the [`DuelCommand`](../TRBot/TRBot.Commands/Commands/DuelCommand.cs).

### groupbet
**Default level: User (0)**

Determines if the user can participate in a group bet through the [`EnterGroupBetCommand`](../TRBot/TRBot.Commands/Commands/EnterGroupBetCommand.cs).

### inputexercise
**Default level: User (0)**

Determines if the user can generate and solve input exercises through the [`InputExerciseCommand`](../TRBot/TRBot.Commands/Commands/InputExerciseCommand.cs).

### calculate
**Default level: User (0)**

Determines if the user can calculate expressions through the [`CalculateCommand`](../TRBot/TRBot.Commands/Commands/CalculateCommand.cs).

### chatbot
**Default level: User (0)**

Determines if the user can speak with a chatbot through the [`ChatbotCommand`](../TRBot/TRBot.Commands/Commands/ChatbotCommand.cs).

### bingo
**Default level: User (0)**

Determines if the user can interact with a bingo board through the [`BingoCommand`](../TRBot/TRBot.Commands/Commands/BingoCommand.cs).

### transfer
**Default level: User (0)**

Determines if the user can transfer credits to others through the [`TransferCreditsCommand`](../TRBot/TRBot.Commands/Commands/TransferCreditsCommand.cs).

### slots
**Default level: User (0)**

Determines if the user can play the slots through the [`SlotsCommand`](../TRBot/TRBot.Commands/Commands/SlotsCommand.cs).

### voteinputmode
**Default level: User (0)**

Determines if the user can vote to change the input mode through the [`VoteForInputModeCommand`](../TRBot/TRBot.Commands/Commands/VoteForInputModeCommand.cs).

### addinputmacro
**Default level: User (0)**

Determines if the user can add an input macro through the [`AddMacroCommand`](../TRBot/TRBot.Commands/Commands/AddMacroCommand.cs).

### removeinputmacro
**Default level: User (0)**

Determines if the user can remove an input macro through the [`RemoveMacroCommand`](../TRBot/TRBot.Commands/Commands/RemoveMacroCommand.cs).

### addmeme
**Default level: User (0)**

Determines if the user can add a meme through the [`AddMemeCommand`](../TRBot/TRBot.Commands/Commands/AddMemeCommand.cs).
     
### removememe
**Default level: User (0)**

Determines if the user can remove a meme through the [`RemoveMemeCommand`](../TRBot/TRBot.Commands/Commands/AddMemeCommand.cs).

### addinputsynonym
**Default level: User (0)**

Determines if the user can add an input synonym through the [`AddInputSynonymCommand`](../TRBot/TRBot.Commands/Commands/AddInputSynonymCommand.cs).

### removeinputsynonym
**Default level: User (0)**

Determines if the user can remove an input synonym through the [`RemoveInputSynonymCommand`](../TRBot/TRBot.Commands/Commands/RemoveInputSynonymCommand.cs).

### setgamemessage
**Default level: VIP (20)**

Determines if the user can set the game message displayed on screen through the [`SetGameMessageCommand`](../TRBot/TRBot.Commands/Commands/SetGameMessageCommand.cs).

### setteamsmode
**Default level: Moderator (30)**

Determines if the user can enable or disable [teams mode](./Settings-Documentation.md#teams_mode_enabled) through the [`GetSetTeamsModeCommand`](../TRBot/TRBot.Commands/Commands/GetSetTeamsModeCommand.cs).

### setteamsmodemaxport
**Default level: Moderator (30)**

Determines if the user can set the [max controller port available for teams mode](./Settings-Documentation.md#teams_mode_max_port) through the [`GetSetTeamsModeMaxPortCommand`](../TRBot/TRBot.Commands/Commands/GetSetTeamsModeMaxPortCommand.cs).

### setconsole
**Default level: Moderator (30)**

Determines if the user can set the active game console through the [`GetSetConsoleCommand`](../TRBot/TRBot.Commands/Commands/GetSetConsoleCommand.cs).

### setdefaultinputdur
**Default level: Moderator (30)**

Determines if the user can set the global default input duration through the [`DefaultInputDurCommand`](../TRBot/TRBot.Commands/Commands/DefaultInputDurCommand.cs).

### setmaxinputdur
**Default level: Moderator (30)**

Determines if the user can set the global maximum input sequence duration through the [`MaxInputDurCommand`](../TRBot/TRBot.Commands/Commands/MaxInputDurCommand.cs).

### setmidinputdelay
**Default level: Moderator (30)**

Determines if the user can toggle the global mid input delay and set its duration through the [`MidInputDelayCommand`](../TRBot/TRBot.Commands/Commands/MidInputDelayCommand.cs).

### setperiodicinput
**Default level: Moderator (30)**

Determines if the user can enable or disable the periodic input through the ['TogglePeriodicInputCommand'](../TRBot/TRBot.Commands/Commands/TogglePeriodicInputCommand.cs).

### setperiodicinputport
**Default level: Moderator (30)**

Determines if the user can set the default periodic input controller port through the ['GetSetPeriodicInputPortCommand'](../TRBot/TRBot.Commands/Commands/GetSetPeriodicInputPortCommand.cs).

### setperiodicinputtime
**Default level: Moderator (30)**

Determines if the user can set the periodic input interval through the ['GetSetPeriodicInputTimeCommand'](../TRBot/TRBot.Commands/Commands/GetSetPeriodicInputTimeCommand.cs).

### setperiodicinputsequence
**Default level: Moderator (30)**

Determines if the user can set the periodic input sequence through the ['GetSetPeriodicInputSequenceCommand'](../TRBot/TRBot.Commands/Commands/GetSetPeriodicInputSequenceCommand.cs).

Attempting to set this causes the command to undergo steps to validate that the user setting this value cannot bypass their normal input restrictions. This includes comparing the user's level to the global input permission level and the permission level of each input in the input sequence, checking the user's restricted inputs, and verifying the controller port for each input.

### setmaxuserrecentinputs
**Default level: Moderator (30)**

Determines if the user can set the max number of recent inputs stored per user through the [`GetSetMaxUserRecentInputsCommand`](../TRBot/TRBot.Commands/Commands/GetSetMaxUserRecentInputsCommand.cs).

A value of 0 will essentially disable the feature. Any excess recent inputs stored above the limit will be removed next time a new one is added.

### updateotheruserabilities
**Default level: Admin (40)**

Determines if the user can update other users' abilities through the [`UpdateAllUserAbilitiesCommand`](../TRBot/TRBot.Commands/Commands/UpdateAllUserAbilitiesCommand.cs).

### setglobalinputlevel
**Default level: Admin (40)**

Determines if the user can set the global minimum access level to perform inputs through the [`GlobalInputPermissionsCommand`](../TRBot/TRBot.Commands/Commands/GlobalInputPermissionsCommand.cs).

### setvcontrollertype
**Default level: Admin (40)**

Determines if the user can set the virtual controller type through the [`VirtualControllerCommand`](../TRBot/TRBot.Commands/Commands/VirtualControllerCommand.cs).

### setvcontrollercount
**Default level: Admin (40)**

Determines if the user can set the number of virtual controllers available through the [`ControllerCountCommand`](../TRBot/TRBot.Commands/Commands/ControllerCountCommand.cs).

### setdemocracyvotetime
**Default level: Admin (40)**

Determines if the user can set the [Democracy vote time](./Settings-Documentation.md#democracy_input_vote_time) through the [`GetSetDemocracyVoteTimeCommand`](../TRBot/TRBot.Commands/Commands/GetSetDemocracyVoteTimeCommand.cs).

### setdemocracyresolutionmode
**Default level: Admin (40)**

Determines if the user can set the [Democracy resolution mode](./Settings-Documentation.md#democracy_resolution_mode) through the [`GetSetDemocracyResModeCommand`](../TRBot/TRBot.Commands/Commands/GetSetDemocracyResModeCommand.cs).

### setinputmodevotetime
**Default level: Admin (40)**

Determines if the user can set the length of the voting period when changing the input mode.

### setinputmodechangecooldown
**Default level: Admin (40)**

Determines if the user can set the cooldown after voting for an input mode.

### setinputmode
**Default level: Admin (40)**

Determines if the user can directly set the [input mode](./Settings-Documentation.md#input_mode) through the [`GetSetInputModeCommand`](../TRBot/TRBot.Commands/Commands/GetSetInputModeCommand.cs).
