# TRTwitchPlaysBot
[Example of an advanced command](https://tdeeb.github.io/projects/images/TRBot/TRBot_AdvancedCommands.gif)

A Twitch bot that contains many features to facilitate handling Twitch Plays games. It uses [TwitchLib](https://github.com/TwitchLib/TwitchLib) to handle Twitch connection.

The bot is inspired by TwitchPlays_Everything; the input syntax allows for great precision, making it well-suited for 2D and 3D games of any kind.

## Features
* Powerful input parser with a flexible, expressive syntax - original Python implementation by TwitchPlays_Everything, improved and converted to C#.
* Virtual controller handling through a simple interface with currently two implementations: vJoy (Windows) and uinput (Linux)
* Performant multi-threaded input handler with support for stopping all ongoing inputs
* Flexible console infrastructure - supports NES, SNES, N64, GC, Wii, and more that can be added
* Powerful Twitch Plays features - macros, savestate support, game logs, and silencing users (without having to timeout on Twitch)
* User data with access levels that restrict or allow access to commands and inputs
* Several additional bot goodies and games, such as dueling, jump rope, and memes

## Getting Started
Instructions for building and setting up the bot can be found on the [wiki](https://github.com/teamradish/TRTwitchPlaysBot/wiki/Getting-Started).

## See it in action
Want to see what players can do with TRBot? Head over to the wiki's [examples](https://github.com/teamradish/TRTwitchPlaysBot/wiki/Real-Usage-Examples) page to see clips from real playthroughs.
