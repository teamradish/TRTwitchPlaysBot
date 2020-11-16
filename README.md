# <img src="./Logo/TRBotLogo.png" alt="TRBot" height="43" width="42" align="top"/> TRBot

TRBot is software that enables playing video games through text. It contains a comprehensive set of projects to facilitate text-based gameplay, some of which allow for setting up and managing community-oriented remote play, such as Twitch Plays. The goal of TRBot is to lower the barrier of entry for text-based gameplay, such as Twitch Plays, and provide avenues to improve the experience.

TRBot is inspired by TwitchPlays_Everything. The input syntax allows for great precision, making it well-suited for many types of games. You can use it for a variety of purposes, including, but not limited to, allowing others to play games remotely and automating tedious/repetitive tasks in your own playthroughs, such as mashing buttons or grinding levels in an RPG.

## Features
* [Simple yet expressive input syntax](./Wiki/Syntax-Walkthrough.md) - translate your text to game inputs easily. Make your inputs as simple or precise as you want!
* Game-agnostic - TRBot runs alongside your game and can be configured as you like.
* Cross-platform - Runs on Windows (vJoy) and GNU/Linux (uinput).
* Emulator support - NES, SNES, N64, GCN, and more. Several [emulator controller config files](./Emulator%20Controller%20Configs) are available.
* Multiplayer support - Control [multiple players](./Wiki/Syntax-Walkthrough.md#multi-controller-inputs) separately or simultaneously.
* Highly performant input handling with near frame-perfect inputs.
* Robust, pluggable virtual controller architecture - add your own custom virtual controller to support additional platforms and types of play.
* Run locally or through Twitch - TRBot is set up for easily integrating other services.
* Flexible console infrastructure - change inputs, add new inputs, or even add a new console with an entirely different set of inputs, on the fly.
* Modular - TRBot is separated into parts, allowing those parts to be used as a library in an application. The core application, `TRBot.Main`, functions this way.
* Commands - Modify, interact with, and obtain information from TRBot through commands entered as text. Commands can be simple, such as [`SayCommand`](./TRBot/TRBot.Commands/Commands/SayCommand.cs), or more complex, such as [`AddInputCommand`](./TRBot/TRBot.Commands/Commands/AddInputCommand.cs). 
* Twitch Plays quality-of-life enhancements - macros, game logs, user silencing (without timeout/ban), stop all ongoing inputs, and switch consoles on the fly.
* SQLite database with configurable data and settings, including access levels, commands, virtual controller count, and more. Any data changes are immediately reflected in TRBot.
* Sleep prevention - prevent games and consoles from going to sleep with a periodic input, controller port, and interval of your choosing.
* Moderation features - control access to commands, inputs, and other features independent of platform.
* Additional goodies and games - duel for credits, create memes, calculate expressions, and talk to a chatbot.

## Documentation, setup, building from source, playing
Please see the [wiki home](./Wiki/Home.md) for your use-case(s).

## Credits
The original Python version of the original parser was written by TwitchPlays_Everything and greatly helped jump-start TRBot.

TRBot's logo was designed by the talented [David Revoy](https://www.davidrevoy.com/), well-known for his Pepper & Carrot comic series.

## License
[![AGPL](https://www.gnu.org/graphics/agplv3-155x51.png)](https://www.gnu.org/licenses/agpl-3.0.en.html)

TRBot is free software; you are free to run, study, modify, and redistribute it. Specifically, you can modify and/or redistribute TRBot under the terms of the GNU Affero General Public License v3.0 or (at your option) any later version.

See the [LICENSE](https://github.com/teamradish/TRTwitchPlaysBot/blob/master/LICENSE) file for the full terms. See the [Dependency Licenses](./Dependency%20Licenses) file for the licenses of third party libraries used by TRBot. See the [logo license](./Logo/Logo%20License) file for the license of TRBot's logo.

## Contributing
Issues and pull requests are greatly encouraged! Please file an issue for a feature request, such as a new platform or service, or regarding any bugs you encounter.

Developing software takes considerable time and effort, and we have poured hundreds of hours into making TRBot as powerful as it is and freely available to everyone in our spare time. Kindly consider donating to us on Liberapay: https://liberapay.com/kimimaru/

All donations go towards improving TRBot, and if enough donations have been met, working on it full-time.
