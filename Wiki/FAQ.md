# FAQ

## ...So, what is this again?
TRBot is software that lets you play games through text. If you type "right", the character in your game will move right if configured to do so. TRBot achieves this through virtual game controllers on your operating system that simulate real controllers.

## My inputs aren't doing anything in the game!
Make sure the [virtual controllers are set up properly](./Setup-VController.md).

If they still aren't working afterwards, make sure the game is using the virtual controllers as the input device. For emulators you can often choose the input device to use. For PC games this is more complex, as many games aren't flexible about input remapping. See more information on setting up PC games [here](./Setup-Misc.md#pc-games).

## My inputs are off! I press "a" but the game is pressing "b" instead!
Each TRBot console has different button values for each button. Make sure you're using the correct console for your game (Ex.  the N64 console for N64 games). You can view the current console and change consoles with the [`GetSetConsoleCommand`](../TRBot/TRBot.Commands/Commands/GetSetConsoleCommand.cs) (default: "!console").

If you're using a custom console, check your button values with the [`InputInfoCommand`](../TRBot/TRBot.Commands/Commands/InputInfoCommand.cs)(default: "!inputs") via "!inputs myconsole a" and "!inputs myconsole b" to verify the values aren't the same or mismatched.

## Why did my input not go through??
There may be several reasons an input didn't go through:

1. You're silenced and thus denied from making inputs. Check your abilities with the [`ListUserAbilitiesCommand`](../TRBot/TRBot.Commands/Commands/ListUserAbilitiesCommand.cs) (default: "!userabilities") and see if the silenced ability is on the list and not disabled.
2. Inputs are restricted to access levels higher than yours. View the global input access level through the [`GlobalInputPermissionsCommand`](../TRBot/TRBot.Commands/Commands/GlobalInputPermissionsCommand.cs) (default: "!inputperms") and your own access level through the [`LevelCommand`](../TRBot/TRBot.Commands/Commands/LevelCommand.cs) (default: "!level").
3. You input a dynamic macro that does not parse correctly. Make sure the arguments you entered are valid.
4. A dynamic macro **inside another** dynamic macro has a space between its arguments. For example, "#mash(a, b)" is invalid while "#mash(a,b)" is correct.
5. You forgot to specify the "*" or the number of repetitions in a repeated input. "[a]\*5" is valid, whereas "[a]5" and "[a]\*" are not!

TRBot will output an error message in certain circumstances, such as when an input goes over the max input duration. The error messages are limited due to how TRBot parses the syntax through regex: when an input is invalid, the regex will often not pick it up at all, making it unable to determine the exact error.

## How do I hold right and jump?
See the [syntax walkthrough](./Syntax-Walkthrough.md).

## How do I press "a" and "b" at the same time? I need to do special combos!
Again, see the [syntax walkthrough](./Syntax-Walkthrough.md) :)
