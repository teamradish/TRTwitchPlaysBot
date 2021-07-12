# Custom Commands
TRBot supports custom commands through the [`ExecFromFileCommand`](../TRBot/TRBot.Commands/Commands/ExecFromFileCommand.cs). If you provide a path to a C# source file as the `ValueStr` of a command with this class, it'll read the text as code and execute it. This supports both absolute paths and files relative to the **Data** folder.

There are some important things to note with custom commands that will be mentioned later.

This tutorial will be using a command called "testcmd" with a source file called "TestCmd.cs".

## Setting up a custom command
First, we need to set up our custom command in the database. If you don't know how to view the database, please read [managing data](./Managing-Data.md).

1. Open the database and navigate to the **CommandData** table.
2. Create a new record and fill out the following information:
  - Set the `Name` of the command to be something descriptive that mentions what it does. In our case, the name should be "testcmd".
  - Set the `ClassName` of the command to be `TRBot.Commands.ExecFromFileCommand`.
  - Set the `Level` value of the command to be the access level you want. For this tutorial, we will have this accessible to everyone, so set it to 0 (User level).
  - Set the `Enabled` value of the command to be 1 so we can actually use it.
  - Set the `DisplayInList` value of the command to 1. We can still use it if it's 0, but unless you don't want users seeing it in the help list for `ListCmdsCommand`, set it to 1.
  - Set the `ValueStr` to the path of the file we're going to use, "TestCmds.cs", without quotes.
3. Write your changes to the database.

## Writing a custom command
Right now the database has a "testcmd" command that will execute custom code from a file called "TestCmd.cs" that's in our **Data** folder. Go ahead and create a text file named "TestCmd.cs" in your **Data** folder. Make sure file extensions are fully visible in your operating system, otherwise the file might actually be named "TestCmd.cs.txt".

Open up that file and input the following lines:

```cs
DataContainer.MessageHandler.QueueMessage("This is a test cmd!");

using (BotDBContext context = DatabaseManager.OpenContext())
{
    User user = DataHelper.GetUserNoOpen("terminaluser", context);
    
    DataContainer.MessageHandler.QueueMessage($"{user.Name} has {user.Stats.Credits} credits!!!!!!!");
}

DataContainer.MessageHandler.QueueMessage($"Cmd Level: {ThisCmd.Level} | Enabled: {ThisCmd.Enabled}");

DataContainer.MessageHandler.QueueMessage($"Evt chat command args: {Args.Command.ArgumentsAsString}");
```

Save the file, load up TRBot (or hard reload with "!reload hard" if it's already running), then type "!testcmd a b cd" to invoke the command.

If all went well, you should see the following messages in order:
1. "This is a test cmd!"
2. "terminaluser has x credits!!!!!!!" ("x" being the number of credits for "terminaluser")
3. "Cmd Level: 0 | Enabled: True"
4. "Evt chat command args: a b cd"

In basic terms, what this command did was print a message, load a user object from the database and print their credit count. Then, it printed data about the command itself and finally printed the arguments given to the command.

If things didn't go so well, you should see a descriptive error message in your console. The nice thing about custom commands is you can change their source code while TRBot is running, and those changes will be applied immediately!

## Important things to note
- Custom commands have access to the following global fields:
 - `ThisCmd` - The command instance being executed. From here you can access fields such as `Level`, `Enabled`, and more.
 - `CmdHandler` - The command handler that all commands have access to.
 - `DataContainer` - The data container instance all commands have access to. From here you can access the message handler, virtual controller manager, and more.
 - `RoutineHandler` - The bot routine handler.
 - `Args` - The `EvtChatCommandArgs` containing data about the user who ran the command, the supplied arguments, and more.
- Use `DataContainer.MessageHandler.QueueMessage` over `Console.WriteLine` to send messages through the current client service. On top of sending the messages to the correct destination, `QueueMessage` also respects the rate-limiting settings for TRBot.
- Custom commands have most other TRBot projects and several common namespaces imported (such as `System`). Anything else will need their full type names, or you will need to explicitly include the namespace. For example, `StringBuilder` needs to be referenced as `System.Text.StringBuilder`, but if you want just `StringBuilder`, you will need to add `using System.Text;` at the top of your custom command's source file.
- While custom commands have access to most of TRBot, it may not be possible to add additional libraries or do much beyond what TRBot is already capable of.
- Since custom commands are directly utilizing TRBot code, and TRBot is directly executing the custom commands, [they are subject to the same licensing terms as TRBot itself](../LICENSE). Keep this in mind if you intend to include any sensitive data in your custom commands.
