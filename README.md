# TRTwitchPlaysBot
A TwitchPlays bot for Team Radish. Continuing the legacy of TwitchPlays_Everything.

This Twitch bot contains many features that facilitate handling Twitch Plays games. It uses [TwitchLib](https://github.com/TwitchLib/TwitchLib) to handle Twitch connection.

## Features
* Powerful input parser with a flexible, expressive syntax - provided by JDog (aka TwitchPlays_Everything) and converted to C# by [tdeeb](https://github.com/tdeeb) (aka Kimimaru)
* Efficient vJoy controller handling with simple methods to press and release input buttons and axes
* Performant multi-threaded input handler with support for stopping all ongoing inputs
* Flexible console infrastructure - supports NES, SNES, N64, GC, Wii, and more that can be easily added
* Powerful Twitch Plays features - macros, savestate support, game logs, and silencing users (without having to timeout on Twitch)
* User data with access levels that restrict/allow access to certain commands
* Several additional bot goodies and games, such as dueling, jump rope, and memes

## Getting Started
Instructions for building and setting up the bot can be found on the [wiki](https://github.com/teamradish/TRTwitchPlaysBot/wiki).

## Disclaimer
TRBot is free, open source software. All data TRBot stores is local to the streamer running the machine. Outside of Twitch chat connectivity via [TwitchLib](https://github.com/TwitchLib/TwitchLib), TRBot does not making any further network requests. Note that streamers may have their own custom builds with additional settings and features. Consult the streamer in question if you'd like to learn more about their build. You can find the license for TwitchLib [here](https://github.com/TwitchLib/TwitchLib/blob/master/LICENSE).
