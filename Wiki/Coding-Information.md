# Coding Information
This document serves to highlight important information regarding TRBot development.

We are on Matrix at [#TRBot-Dev:matrix.org](https://matrix.to/#/!hTfcbsKMAuenQAetQm:matrix.org?via=matrix.org). Feel free to ask any questions or discuss development there!

We have the following two remotes for TRBot:
* GitHub: https://github.com/teamradish/TRTwitchPlaysBot.git
* Codeberg: https://codeberg.org/kimimaru/TRBot.git

# Data-Related
- Most data-related code can be found in the `TRBot.Data` project.
- **Always** open a new database context with `DatabaseManager.OpenContext()` or `DatabaseManager.OpenCloseContext(DBContextAction<BotDBContext> dbContextAction)`. These methods use the correct path for the database and are consistent. **Do not instantiate a `BotDBContext` manually!** I'll repeat it as many times as it takes! 
- **Always** wrap the `BotDBContext` inside a `using`. This guarantees that we don't encounter any memory leaks leading to funky behavior.
- Make all uses of the database context as concise as possible. This is recommended as a result of great pains in the past where some ordinary-looking code was producing odd behavior. Keep things brief and to the point!
- `DataHelper` contains many useful utility methods to obtain information from the database.
- If you have a database context open, **do NOT** use the `DataHelper` methods that don't end with "NoOpen". This will cause it to re-open the database context and probably not do whatever you wanted it to do. In this case, use the "NoOpen" variants instead, **but remember to keep things concise**. I'll say it again and again!
- `DataContainer` holds several important objects, such as the message handler and the current virtual controller manager. If you make a change to one of its objects, make sure to make the change with the `DataContainer`'s reference. This container is shared between the `TRBot.Main` application, all the commands, and all the bot routines, giving them access to the same crucial information.
-To add new default data, add them to `DefaultData.cs`. If you need to add a new category of data that isn't there, you will have to modify `DataHelper.InitDefaultData`. Make sure to add to the count of new entries, and don't add the entries if they already exist to prevent overwriting existing data.

# Parsing Inputs
- Inputs are parsed through the `TRBot.Parsing.IParser` interface. There is currently only one implementation, `StandardParser`.
- It's recommended to use the static `StandardParser.CreateStandard` method to create a parser configured consistently with the rest of the project. This may be revisited in the future if there end up being more parser implementations.
- The `StandardParser` relies on `ParserComponents` to piece together the regex. The standard order of components is as follows:
  1. `PortParserComponent`
  2. `HoldParserComponent`
  3. `ReleaseParserComponent`
  4. `InputParserComponent`
  5. `PercentParserComponent`
  6. `MillisecondParserComponent`
  7. `SecondParserComponent`
  8. `SimultaneousParserComponent`
- Call `ParsedInputSequence ParseInputs(string message)` with your `IParser` instance to get the parsed input sequence. If the `ParsedInputResult` is Valid, you're good to go. Otherwise, you can print the `Error` to get a detailed message on what went wrong with parsing. 

## Important Notes
- Parsing inputs using `IParser.ParseInputs` is fairly simple, but there is some work required to prepare the input string beforehand. These are done through `IPreparsers`, which can be passed into the constructor of a `StandardParser`. Their functions include removing whitespace, expanding repeated inputs, and populating macros. If you use `StandardParser.CreateStandard`, these will already be handled for you.
- The standard order of `IPreparser`s is as follows:
  1. `InputMacroPreparser`
  2. `InputSynonymPreparser`
  3. `ExpandPreparser`
  4. `RemoveWhitespacePreparser`
  5. `LowercasePreparser`
- It's recommended to parse inputs with an opened database context so you have all the information you need to create the standard parser configuration.
- After parsing, but before carrying out the input sequence, you'll want to perform some post-processing. Without post-processing, there is no permission checks nor any mid input delay insertions.
- Look at the static `ParserPostProcess` class in the `TRBot.Misc` project for various methods to help validate the input sequence.

# Carrying Out Inputs
- The static `InputHandler` class is used to carry out inputs. This class lives in the `TRBot.Misc` project.
- `InputHandler.CarryOutInput` will attempt to start carrying out the input on another thread. It will first convert the supplied lists into arrays to improve performance when executing the inputs.
- `InputHandler.ExecuteInput` is where the inputs are executed, and it is internally called by `InptuHandler.CarryOutInput` on a separate thread. **This is a very tight, performant method, so be extremely careful with any changes you make**. In particular, once the first loop starts, all the code within it has be to as optimal as possible to minimize delays between inputs.
  - If you find any bugs or have any other suggestions for improving this code, **please file an issue first!** It cannot be stressed how important this method is, as it's the core of TRBot's functionality.

## Important Notes
- Before calling `InputHandler.CarryOutInput`, make sure to check if `InputHandler.InputsHalted` is false. If it's true, do not call `InputHandler.CarryOutInput`, as something important is going on, which may include changing the virtual controller implementation, changing the number of virtual controllers, or a player desiring all ongoing inputs to be stopped. Failure to do this may likely result in the bot crashing.
- The console passed into `InputHandler.CarryOutInput` should be a **brand new instance** constructed from one in the database. This ensures that everything that was valid **at that time** will still be valid while executing the input. Basically, this guarantees that if someone removes an input, or even the current console, while executing an input sequence, nothing will go haywire.

# Logging
- Use the static `TRBotLogger` class for logging. By default, it logs to a file and the console. This class lives in the `TRBot.Logging` project.
- Internally, TRBot uses Serilog for logging. Use the appropriate methods for the types of information - for example, `Information` should be for general logs whereas `Error` or `Fatal` should be used if something went wrong.
- Keep in mind that even if logs aren't output based on the log settings, it will still be doing the work when it's constructing the string. If the work involved is expensive (Ex. parsing or reverse parsing) and exclusive to the log, consider commenting it out and uncommenting it when it's needed.

# Contributing
If you find any problems with TRBot, please file an [issue](https://github.com/teamradish/TRTwitchPlaysBot/issues). [Pull requests](https://github.com/teamradish/TRTwitchPlaysBot/pulls) are also highly encouraged!

TRBot is free software; as such, you can run, study, modify, and distribute it for any purpose under the terms of the GNU Affero General Public License v3.0 or (at your option) any later version. See the [License](../LICENSE) for more information and [Dependency Licenses](../Dependency%20Licenses) file for the licenses of third party libraries used by TRBot.
