# Custom Routines
TRBot's custom routine support is handled through the [`ExecFromFileRoutine`](../TRBot/TRBot.Routines/Routines/ExecFromFileRoutine.cs). Like [custom commands](./Custom-Commands.md), if you provide a path to a C# source file as the `ValueStr` of a routine with this class, it'll read the text as code and execute it. This supports both absolute paths and files relative to the **Data** folder.

Custom routines are run every update loop, which is defined by the [`main_thread_sleep`](./Settings-Documentation.md#user-content-main_thread_sleep) setting in the database. If the value of `main_thread_sleep` is 500 milliseconds for instance, then a custom routine will run every 500 milliseconds.

This tutorial will be using a routine called "printmsg" with a source file called "PrintMessageRoutine.cs".

## Setting up a custom routine
First, we need to set up our custom routine in the database. If you don't know how to view the database, please read [managing data](./Managing-Data.md).

1. Open the database and navigate to the **RoutineData** table.
2. Create a new record and fill out the following information:
  - Set the `Name` of the routine to something descriptive that mentions what it does. In our case, the name should be "printmsg".
  - Set the `ClassName` of the routine to `TRBot.Routines.ExecFromFileRoutine`.
  - Set the `Enabled` value of the routine to 1 so it runs every update.
  - Set the `ResetOnReload` value of the routine to 0. This prevents the routine from being reset when bot data is reloaded with the [ReloadCommand](../TRBot/TRBot.Commands/Commands/ReloadCommand.cs).
  - Set the `ValueStr` to the path of the file we're going to use, "PrintMessageRoutine.cs", without quotes.
3. Write your changes to the database.

## Writing a custom routine
The database currently has a "printmsg" routine that executes custom code from a file called "PrintMessageRoutine.cs" that's in our **Data** folder. Go ahead and create a text file named "PrintMessageRoutine.cs" in your **Data** folder. Make sure file extensions are fully visible in your operating system, otherwise the file might actually be named "PrintMessageRoutine.cs.txt".

Open up that file and input the following lines:

```cs
using System.Linq;
using Microsoft.EntityFrameworkCore;

DataContainer.MessageHandler.QueueMessage($"{nameof(RoutineHndlr)} is of type {RoutineHndlr.GetType().FullName} | This routine's value is {ThisRoutine.ValueStr} | The current time is {CurrentTimeUTC}.");

long maxCredits = 0;

using (BotDBContext context = DatabaseManager.OpenContext())
{
    var allUsers = context.Users.Include(e => e.Stats);
    maxCredits = allUsers.Max(u => u.Stats.Credits);
}

DataContainer.MessageHandler.QueueMessage($"The max number of credits in the database is {maxCredits}!");
```

Save the file, load up TRBot (or hard reload with "!reload hard" if it's already running), then wait for it to invoke the routine (hopefully your `main_thread_sleep` isn't too long!).

If all went well, you should see the following messages in order:
1. "RoutineHndlr is of type TRBot.Routines.BotRoutineHandler | This routine's value is PrintMessageRoutine.cs | The current time is t." ("t" being the current time)
2. "The max number of credits in the database is x!" ("x" being the highest number of credits a user has in the database)

In basic terms, what this command did was print a message with information, including the source file name and the current time. Finally, it loaded all users in the database with their stats and printed the highest credit count among them.

Like custom commands, if things didn't work out, you should see a descriptive error message in your console. You can also change the source code for custom routines while TRBot is running and have those changes apply immediately!

## Important considerations
- Custom routines have access to the following global fields:
 - `ThisRoutine` - The routine instance being executed. From here you can access fields such as `Enabled`, `ResetOnReload`, and more.
 - `RoutineHndlr` - The bot routine handler that all routines have access to.
 - `DataContainer` - The data container instance all routines have access to. From here you can access the message handler, virtual controller manager, and more.
 - `CurrentTimeUTC` - The current time when the routine was executed. This is useful for routines that wait a certain amount of time, like [`DemocracyRoutine`](../TRBot/TRBot.Routines/Routines/DemocracyRoutine.cs) does when gathering inputs.
- Use `DataContainer.MessageHandler.QueueMessage` over `Console.WriteLine` to send messages through the current client service. On top of sending the messages to the correct destination, `QueueMessage` also respects the rate-limiting settings for TRBot.
- Just like custom commands, custom routines have most other TRBot projects and several common namespaces imported (such as `System`). Anything else will need their full type names, or you will need to explicitly include the namespace. For example, `StringBuilder` needs to be referenced as `System.Text.StringBuilder`, but if you want just `StringBuilder`, you will need to add `using System.Text;` at the top of your custom command's source file.
- While custom routines have access to most of TRBot, it may not be possible to add additional libraries or do much beyond what TRBot is already capable of.
- Since custom routines are directly utilizing TRBot code, and TRBot is directly executing the custom routines, [they are subject to the same licensing terms as TRBot itself](../LICENSE). Keep this in mind if you intend to include any sensitive data in your custom routines.
