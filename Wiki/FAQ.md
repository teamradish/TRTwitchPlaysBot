# FAQ

## ...So, what is this again?
TRBot is software that lets you play games through text. If you type "right", the character in your game will move right if configured to do so. TRBot achieves this through virtual game controllers on your operating system that simulate real controllers.

## Show me what you mean by that gibberish!
View examples [here](./Play-Examples.md).

## My inputs aren't doing anything in the game!
Make sure the [virtual controllers are set up properly](./Setup-VController.md).

If they still aren't working afterwards, make sure the game is using the virtual controllers as the input device. On emulators you can often choose the input device to use. On PC games this is more complex, as many games aren't flexible about input remapping. See more information on setting up PC games [here](./Setup-Misc.md#pc-games).

## My inputs are incorrect! I press "a" but the game is pressing "b" instead!
Each TRBot console has different button values for each button. Make sure you're using the correct console for your game (Ex. the N64 console for N64 games). You can view the current console and change consoles with the [`GetSetConsoleCommand`](../TRBot/TRBot.Commands/Commands/GetSetConsoleCommand.cs) (default: "!console").

If you're using a custom console, check your button values with the [`InputInfoCommand`](../TRBot/TRBot.Commands/Commands/InputInfoCommand.cs)(default: "!inputs") via "!inputs myconsole a" and "!inputs myconsole b" to verify the values aren't the same or mismatched.

## Why did my input not go through??
There may be several reasons:

1. You're silenced and thus denied from making inputs. Check your abilities with the [`ListUserAbilitiesCommand`](../TRBot/TRBot.Commands/Commands/ListUserAbilitiesCommand.cs) (default: "!userabilities") and see if the silenced ability is on the list and not disabled.
2. Inputs are restricted to access levels higher than yours. View the global input access level through the [`GlobalInputPermissionsCommand`](../TRBot/TRBot.Commands/Commands/GlobalInputPermissionsCommand.cs) (default: "!inputperms") and your own access level through the [`LevelCommand`](../TRBot/TRBot.Commands/Commands/LevelCommand.cs) (default: "!level").
3. You input a dynamic macro that does not parse correctly. Make sure the arguments you entered are valid.
4. A dynamic macro **inside another** dynamic macro has a space between its arguments. For example, "#mash(a,b)" is correct, while "#mash(a, b)" is not!
5. You forgot to specify the "*" or the number of repetitions in a repeated input. "[a]\*5" is valid, whereas "[a]5" and "[a]\*" are not!

TRBot will sometimes output an error message if it runs into a problem while parsing, such as when an input sequence goes over the max input duration. The error messages are limited due to how TRBot parses the syntax through regex: when an input is invalid, the regex will often not pick it up at all, making it unable to determine the exact error.

## How do I hold right and jump?
See the [syntax walkthrough](./Syntax-Walkthrough.md).

## How do I press "a" and "b" at the same time? I need to do special combos!
Again, see the [syntax walkthrough](./Syntax-Walkthrough.md) :)

## When I type "a a", it holds "a" instead of pressing it twice! What gives?
This is intentional. When TRBot processes the input sequence, it does so without any delay in between. What you're seeing here is TRBot releasing the first "a" then pressing the second **immediately** after in the same code block.

If you want TRBot to automatically insert artificial delays between your inputs, grant yourself the [usermidinputdelay](./Permission-Documentation.md#usermidinputdelay) ability and set the integer value to the desired delay, in milliseconds. You can do so using the [`UpdateUserAbilityCommand`](../TRBot/TRBot.Commands/Commands/UpdateUserAbilityCommand.cs) (default: "!toggleability"):

- "!toggleability myusername usermidinputdelay true null 50 null" - Will add a 50 millisecond delay between non-blank inputs.

You can remove the delay by changing the "true" argument to "false" to disable the ability.

## Help! I was modifying the database manually, and now my bot froze!
Make sure you write or revert your changes to the database.

When you manually make changes to the database, the database application (Ex. sqlitebrowser) locks the database file, making it temporarily inaccessible to other applications to prevent data corruption. The next time TRBot needs to read or write from the database (likely soon), it will have to wait for this lock to be released before it can continue. Release the lock by writing or reverting the changes in your database application.

## I WASN'T modifying the database manually, and my bot froze/crashed!
This is a bug, so please [file an issue](https://codeberg.org/kimimaru/TRBot/issues/new) and include details about your configuration and what led to the freeze/crash. Submit any notable information from logs in the "Logs" and "CrashLogs" folder if applicable.  If you can reliably reproduce the issue, it would be much easier and quicker to fix the bug!

## How do I shut down the bot?
Click the X to the console window or press Ctrl + C in the window to end the process.

## Can you add feature X, Y, and Z?
Please [file an issue](https://codeberg.org/kimimaru/TRBot/issues/new) for new feature requests.
