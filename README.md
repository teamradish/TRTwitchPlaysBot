# <img src="./Logo/TRBotLogo.png" alt="TRBot" height="43" width="42" align="top"/> TRBot

TRBot is software that enables playing video games through text. In simple terms, if you type "a", the character in your game can jump.

TRBot contains a comprehensive set of projects to facilitate text-based gameplay, some of which allow for setting up and managing community-oriented remote play, including Twitch Plays. The goal of TRBot is to lower the barrier of entry for text-based gameplay and provide avenues to improve the experience.

TRBot is inspired by the bot used on the TwitchPlays_Everything channel. The input syntax allows for great precision, making it well-suited for many types of games, whether it's an RPG or a 3D platformer. You can use TRBot for many purposes, including collaborative remote play and automating tedious tasks, such as mashing buttons or grinding levels in an RPG.

The core application, `TRBot.Main`, connects all the projects of TRBot together to form a fully functional bot with input handling, commands, and more! You can use it as-is or build your own application around the projects.

## Documentation, setup, building from source, playing
- See the [wiki home](./Wiki/Home.md) for your use-case(s).
- See the [FAQ](./Wiki/FAQ.md) for answers to commonly asked questions.
- Want to see examples? [We've got your back](./Wiki/Play-Examples.md).

## Features
- [Simple yet expressive input syntax](./Wiki/Syntax-Walkthrough.md) - translate your text to game inputs easily. Make your inputs as simple or precise as you want!
- Portable and game-agnostic - just download and run! TRBot runs alongside your game and has great compatibility.
- Cross-platform - Runs on Windows (vJoy) and GNU/Linux (uinput).
- Virtual gamepads - Use analog inputs for precision and set custom button mappings in games to play how you prefer.
- Run locally, through Twitch, or connect to a WebSocket of choice. TRBot is also set up for integrating other services.
- Emulator support - NES, SNES, N64, GCN, and more. Several [emulator controller config files](./Controller%20Configs) are available.
- Multiplayer support - Control [multiple players](./Wiki/Syntax-Walkthrough.md#multi-controller-inputs) separately or simultaneously.
- Sleep prevention - prevent games and consoles from going to sleep with a periodic input and interval of your choosing.
- Reset prevention - forbid players from hitting button combos to reset the game.
- Highly performant input handling with near frame-perfect inputs. Play with precision and consistency.
- Flexible "console" infrastructure - change inputs, add new inputs, or even add a new console with a different set of inputs, on the fly.
- Twitch Plays quality-of-life enhancements - macros, game logs, user silencing (without timeout/ban), stop all ongoing inputs, and switch consoles readily.
- Pluggable virtual controller architecture - add your own virtual controller implementation to support more platforms.
- Modular - TRBot is separated into parts, allowing those parts to be used as libraries.
- Commands - Interact with and manipulate TRBot through commands entered as text. [Add your own commands](./Wiki/Custom-Commands.md) to extend TRBot's capabilities, even **while** it's running!
- SQLite database to manage access levels, commands, virtual controllers, and more. Data changes are immediately reflected in TRBot.
- Moderation features - control access to commands, inputs, and other features independent of platform.
- Additional goodies and games - duel for credits, create memes, calculate expressions, and [talk to a chatbot](./Wiki/Setup-Chatterbot.md).

## Credits
The original parser was written in Python by TwitchPlays_Everything and greatly helped jump-start TRBot's development.

TRBot's logo was designed by the talented [David Revoy](https://www.davidrevoy.com/), well-known for his Pepper & Carrot comic series.

## License
Copyright © 2019-2021 Thomas "Kimimaru" Deeb

[![AGPL](https://www.gnu.org/graphics/agplv3-155x51.png)](https://www.gnu.org/licenses/agpl-3.0.en.html)

TRBot is free software; you are free to run, study, modify, and redistribute it. Specifically, you can modify and/or redistribute TRBot under the terms of the GNU Affero General Public License v3.0.

In simple terms, if you give someone a copy of TRBot or deploy TRBot to an online service, you must provide a way to obtain the license and source code for that version of TRBot upon request. This includes modified versions of TRBot.

See the [LICENSE](./LICENSE) file for the full terms. See the [Dependency Licenses](./Dependency%20Licenses) file for the licenses of third party libraries used by TRBot. See the [logo license](./Logo/Logo%20License) file for the license of TRBot's logo.

## Attribution Banner
If you have found TRBot useful, please spread the word by placing one of our promo banners on your website, blog, video, or Twitch stream panel!

[Link to large banner](./Logo/TRBotLogo_Promo.png)
- Markdown code:
    ```
    ![Powered by TRBot](https://codeberg.org/kimimaru/TRBot/raw/branch/master/Logo/TRBotLogo_Promo.png "TRBot Logo")
    ```

[Link to small banner](./Logo/TRBotLogo_Promo_Small.png)
- Markdown code: 
    ```
    ![Powered by TRBot](https://codeberg.org/kimimaru/TRBot/raw/branch/master/Logo/TRBotLogo_Promo_Small.png "TRBot Logo")
    ```

## Contributing
Our main repository is on Codeberg: https://codeberg.org/kimimaru/TRBot.git

Issues and pull requests are greatly encouraged! Please file an issue if you encounter any bugs or have a feature request.

### Support
Feel free to ask questions or discuss development on our Matrix room at [#TRBot-Dev:matrix.org](https://matrix.to/#/!hTfcbsKMAuenQAetQm:matrix.org?via=matrix.org). You can also [contact us](mailto:trbot (at) posteo (dot) de) for support. Paid setup and support options are also available.

Developing software takes considerable time and effort, and we have poured hundreds of hours of our spare time into making TRBot as powerful and feature-rich as it is and freely available to everyone. Kindly consider [buying us a coffee](https://ko-fi.com/kimimaru) or [donating](https://liberapay.com/kimimaru/).
