# TRTwitchPlaysBot
A Twitch bot designed to facilitate setting up and managing Twitch Plays games. It uses [TwitchLib](https://github.com/TwitchLib/TwitchLib) to handle Twitch connection.

The bot is inspired by TwitchPlays_Everything; the input syntax allows for great precision, making it well-suited for many types of games.

[Example of an advanced command.](https://tdeeb.github.io/projects/images/TRBot/TRBot_AdvancedCommands.gif)


## Features
* [Expressive yet simple input syntax](https://github.com/teamradish/TRTwitchPlaysBot/Wiki/Syntax-Tutorial.md) - translate your text to game inputs easily. Make your inputs as simple or precise as you want!
* Game-agnostic; TRBot runs alongside your game.
* Emulator support - NES, SNES, N64, GCN, and more. Several [emulator controller config files](https://github.com/teamradish/TRTwitchPlaysBot/tree/master/Emulator%20Controller%20Configs) are available.
* Runs on Windows (vJoy) and Linux (uinput).
* Highly performant input handling with minimal delay between inputs.
* Pluggable virtual controller architecture - add your own custom virtual controller.
* Twitch Plays quality-of-life enhancements - macros, game logs, savestates, user silencing (without timeout/ban), stop all ongoing inputs, and switch consoles on the fly.
* User data with access levels - control access to commands, inputs, and other features.
* Additional goodies and games - duel for credits, create memes, calculate expressions, and reverse text.

## Building from source
* Clone the repo with `git clone https://github.com/teamradish/TRTwitchPlaysBot.git`
  * Alternatively, download the zip.
* [.NET Core 3.1 SDK and Runtime](https://dotnet.microsoft.com/download/dotnet-core)
  * Before installing, set the `DOTNET_CLI_TELEMETRY_OPTOUT` environment variable to 1 if you don't want dotnet CLI commands sending telemetry.

You can build TRBot using the provided .sln or through the CLI (instructions below). You can also use any IDE supporting .NET Core, such as VSCode/VSCodium, Visual Studio, or JetBrains Rider.

Command line:
* Main directory: `cd TRBotCore`
* Building: `dotnet build`
* Publishing: `dotnet publish -c (config) -o (dir) --self-contained --runtime (RID)`
  * config = "Debug" or "Release"
  * dir = output directory
  * [RID](https://github.com/dotnet/runtime/blob/master/src/libraries/pkg/Microsoft.NETCore.Platforms/runtime.json) = usually "win-x64" or "linux-x64". See link for a full list of runtime identifiers.
  * Example: `dotnet publish -c Debug -o TRBot --self-contained --runtime linux-x64`

When building, make sure to define the correct preprocessor directives in the .csproj project file - `WINDOWS` for Windows, and `LINUX` for Linux.

## Running Tests
All unit tests are in the **TRBot.Tests** project. Simply run `dotnet test` inside this directory or the base **TRBot** directory to run the tests.

## Getting Started
Please see the [Getting Started](https://github.com/teamradish/TRTwitchPlaysBot/Wiki/Getting-Started.md) guide for setting up TRBot.

## See it in action
Want to see what players have done with TRBot? Check out the [examples](https://github.com/teamradish/TRTwitchPlaysBot/Wiki/Real-Usage-Examples.md) page for clips from real playthroughs.

## Credits
The original Python version of the original parser was written by TwitchPlays_Everything.

## License
[![AGPL](https://www.gnu.org/graphics/agplv3-155x51.png)](https://www.gnu.org/licenses/agpl-3.0.en.html)

TRBot is free software; you are free to run, study, modify, and redistribute it. Specifically, you can modify and/or redistribute TRBot under the terms of the GNU Affero General Public License v3.0 or (at your option) any later version.

See the [LICENSE](https://github.com/teamradish/TRTwitchPlaysBot/blob/master/LICENSE) file for the full terms. See the [Dependency Licenses](https://github.com/teamradish/TRTwitchPlaysBot/blob/master/Dependency%20Licenses) file for the licenses of third party libraries used by TRBot.


Liberapay: https://liberapay.com/kimimaru/
