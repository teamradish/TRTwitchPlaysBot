# TRBot
TRBot is software capable of playing video games through text. It also contains many features to facilitate setting up and managing Twitch Plays games. It uses [TwitchLib](https://github.com/TwitchLib/TwitchLib) to handle Twitch connection.

The bot is inspired by TwitchPlays_Everything; the input syntax allows for great precision, making it well-suited for many types of games.

[Example of an advanced command.](https://tdeeb.github.io/projects/images/TRBot/TRBot_AdvancedCommands.gif)

## Features
* [Simple yet expressive input syntax](./Wiki/Syntax-Tutorial.md) - translate your text to game inputs easily. Make your inputs as simple or precise as you want!
* Game-agnostic; TRBot runs alongside your game.
* Emulator support - NES, SNES, N64, GCN, and more. Several [emulator controller config files](https://github.com/teamradish/TRTwitchPlaysBot/tree/master/Emulator%20Controller%20Configs) are available.
* Runs on Windows (vJoy) and GNU/Linux (uinput).
* Highly performant input handling with minimal delay between inputs.
* Robust, pluggable virtual controller architecture - add your own custom virtual controller.
* Flexible console infrastructure - change inputs, add new inputs, or even add a new console with an entirely new set of inputs, on the fly.
* Twitch Plays quality-of-life enhancements - macros, game logs, user silencing (without timeout/ban), stop all ongoing inputs, and switch consoles on the fly.
* SQLite database with configurable data and settings, including access levels, commands, virtual controller count, and more. Any data changes are immediately reflected in TRBot.
* Moderation features - control access to commands, inputs, and other features independent of platform.
* Additional goodies and games - duel for credits, create memes, calculate expressions, and talk to a chatbot.

## Documentation, setup, building from source, playing
Please see the [wiki home](./Wiki/Home.md) for your use-cases.

## Credits
The original Python version of the original parser was written by TwitchPlays_Everything and greatly helped jump-start TRBot.

## License
[![AGPL](https://www.gnu.org/graphics/agplv3-155x51.png)](https://www.gnu.org/licenses/agpl-3.0.en.html)

TRBot is free software; you are free to run, study, modify, and redistribute it. Specifically, you can modify and/or redistribute TRBot under the terms of the GNU Affero General Public License v3.0 or (at your option) any later version.

See the [LICENSE](https://github.com/teamradish/TRTwitchPlaysBot/blob/master/LICENSE) file for the full terms. See the [Dependency Licenses](https://github.com/teamradish/TRTwitchPlaysBot/blob/master/Dependency%20Licenses) file for the licenses of third party libraries used by TRBot.

## Contributing
Issues and pull requests are greatly encouraged!

Developing software takes considerable time and effort, and we have poured hundreds of hours into making TRBot as powerful as it is and freely available to everyone in our spare time. Kindly consider donating to us on Liberapay: https://liberapay.com/kimimaru/

All donations go towards improving TRBot, and if enough donations have been met, working on it full-time.
