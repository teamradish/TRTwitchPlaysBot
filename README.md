# TRTwitchPlaysBot
[Example of an advanced command](https://tdeeb.github.io/projects/images/TRBot/TRBot_AdvancedCommands.gif)

A Twitch bot designed to facilitate setting up and managing Twitch Plays games. It uses [TwitchLib](https://github.com/TwitchLib/TwitchLib) to handle Twitch connection.

The bot is inspired by TwitchPlays_Everything; the input syntax allows for great precision, making it well-suited for many types of games.

## Features
* [Expressive yet simple input syntax](https://github.com/teamradish/TRTwitchPlaysBot/wiki/Syntax-Tutorial) - translate your text to game inputs easily. Make your inputs as simple or precise as you want!
* Game-agnostic; TRBot runs alongside your game.
* Emulator support - NES, SNES, N64, GCN, and more. Several [emulator controller config files](https://github.com/teamradish/TRTwitchPlaysBot/tree/master/Emulator%20Controller%20Configs) are available.
* Runs on Windows (vJoy) and Linux (uinput).
* Highly performant input handling with minimal delay between each input.
* Pluggable virtual controller architecture - add your own custom virtual controller.
* Twitch Plays quality-of-life enhancements - macros, game logs, savestates, user silencing (without timeout/ban), stop all ongoing inputs, and switch consoles on the fly.
* User data with access levels - control access to commands, inputs, and other features.
* Additional goodies and games - duel for credits, create memes, calculate expressions, and reverse text.

## Getting Started
Please see the [Getting Started](https://github.com/teamradish/TRTwitchPlaysBot/wiki/Getting-Started) guide on the wiki for setting up TRBot.

## See it in action
Want to see what players have done with TRBot? Check out the [examples](https://github.com/teamradish/TRTwitchPlaysBot/wiki/Real-Usage-Examples) page for clips from real playthroughs.

## Credits
The original Python version of the original parser was written by TwitchPlays_Everything.

## License
[![AGPL](https://www.gnu.org/graphics/agplv3-155x51.png)](https://www.gnu.org/licenses/agpl-3.0.en.html)

TRBot is free software; you are free to run, study, modify, and redistribute it. Specifically, you can modify and/or redistribute TRBot under the terms of the GNU Affero General Public License v3.0 or (at your option) any later version.

See the [LICENSE](https://github.com/teamradish/TRTwitchPlaysBot/blob/master/LICENSE) file for the full terms. See the [Dependency Licenses](https://github.com/teamradish/TRTwitchPlaysBot/blob/master/Dependency%20Licenses) file for the licenses of third party libraries used by TRBot.
