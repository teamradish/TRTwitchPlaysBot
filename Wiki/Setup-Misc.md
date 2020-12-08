Here you'll find supplementary information that can be used to enhance TRBot! 

# Chatbot
See the [chatbot guide](./Setup-ChatterBot.md) for setting up a chatbot that your viewers can talk to!

# Game Message
TRBot has an optional game message that can be set with the `SetMessageCommand`. An example of such a message may be "Beat level 1". Once set, the message is saved into a file specified by the **game_message_path** setting in the database (the default is a **GameMessage.txt** file in the **Data** folder). You can display this message on screen using OBS via the following steps:

1. Create Text (GDI+, FreeType2, etc.)
2. Check the box labeled "Read from file"
3. Browse and select the text file.

Now the message should be displayed on screen for all your viewers to see. Whenever the message is changed through the bot, it will be updated in the file and subsequently on screen.

# Displaying Twitch Chat
There are several options for displaying Twitch chat on your stream:

* [ChatGameFontificator](https://github.com/GlitchCog/ChatGameFontificator) is very easy to set up, customizable, and has themes for many games out of the box.
* [KapChat](https://www.nightdev.com/kapchat/) can be set up in a few minutes and is capable of displaying Twitch chat, including emotes, through OBS and other streaming software.
* [Restream Chat](https://restream.io/chat)
* Find another API or write your own chat display using the messages that come in through the bot.

# Displaying Game Inputs
* Many emulators support displaying inputs directly.
* [Open Joystick Display](https://github.com/KernelZechs/open-joystick-display) is easy to use, supports many game controllers, and has a streaming mode, which displays only the controller.
* Write your own input display using TRBot's virtual controller capabilities:
  * `IVirtualController.GetInputState` can tell if a given input name is pressed on the controller.
  * `IVirtualController.GetButtonState` can tell if a given button number is pressed on the controller.
  * `IVirtualController.GetAxisState` tells the percentage a given axis is moved in. 

# Timer Options
* [obs-advanced-timer](https://github.com/cg2121/obs-advanced-timer) for a non-I/O intensive timer on OBS (especially good for HDDs).
  * [This fork](https://github.com/tdeeb/obs-advanced-timer/tree/CountupStart) adds the ability to start a countup timer from a given time ("CountupStart" branch).

# PC Games
Inputs should work for PC games that can recognize the virtual controllers. Some games using the SDL library may need an input mapping string to recognize the controllers. You can put this input mapping as an environment variable if there are no options for inserting them directly into a file for the game itself. For more information, [see this SDL mapping tool](https://generalarcade.com/gamepadtool/).

There is also experimental keyboard and mouse controls accessible on GNU/Linux systems running X11 if [`xdotool`](https://www.semicomplete.com/projects/xdotool/) is installed.

## PC Game Precautions
**Be very careful when playing PC games!** Make sure that players can't exit the game, access files or perform any other malicious activities, such as shutting down the system. If you're streaming a PC game, highly consider capturing only the window with the game and not the entire display; this way if players manage to exit the game, they won't be able to see anything else on your computer. This is easier to accomplish by playing the game in windowed mode.

## Automatically restart PC games on exit
If there's no way to prevent players from exiting the game, you can set up a script to restart the game once it's closed. With this, OBS should also detect that the game window is available and display it on stream again. Here's an example for **bash**:
```
(while true; do 
    ./mygame
done)
```

This may not always work for all games, especially those with DRM and/or can only be launched through a client, such as Steam. However, you can find a **bash** script that does this for Steam games [here](../Supplementary/RestartSteamGame.sh).

# Sleep Inactivity
For games and consoles that sleep after a period of inactivity, you can enable periodic inputs to have TRBot automatically perform an input sequence on a virtual controller at a regular interval (Ex. "a" every 5 minutes). Doing so can prevent the game or console from sleeping. Here's how to configure it:

1. Set the `ValueInt` of [periodic_input_enabled](./Settings-Documentation.md#periodic_input_enabled) to 1 in the database.
2. Provide the input(s) you'd like to press by setting the `ValueStr` of [periodic_input_value](./Settings-Documentation.md#periodic_input_value). These can also be input macros and input synonyms.
3. Set the controller port you'd like to press this input on through [periodic_input_port](./Settings-Documentation.md#periodic_input_port).
4. Set the interval to press your input(s) with [periodic_input_time](./Settings-Documentation.md#periodic_input_time).

# Reset Prevention
Some games and game consoles have a reset function performed by inputting a specific combination of buttons. One example is the Game Boy Advance, in which pressing "A", "B", "Start", and "Select" will reset the game.

TRBot supports an invalid input combo for each console that can be used to prevent these combinations from being pressed to forbit players from resetting the game. Invalid input combos apply per controller port.

- The [`AddInvalidInputComboCommand`](../TRBot/TRBot.Commands/Commands/AddInvalidInputComboCommand.cs) (default: "!addinvalidcombo") can be used to add an input to the invalid input combo. The given input must already be a valid input on the console itself.
- The [`RemoveInvalidInputComboCommand`](../TRBot/TRBot.Commands/Commands/RemoveInvalidInputComboCommand.cs) (default: "!removeinvalidcombo") can be used to remove an input from the invalid input combo.
- The [`ListInvalidInputComboCommand`](../TRBot/TRBot.Commands/Commands/ListInvalidInputComboCommand.cs) (default: "!invalidcombo") can be used to list the invalid input combo for a given game console.

See the [Commands Documentation](./Commands-Documentation.md) for more information on these commands.

# Contributing
If you find any problems with TRBot, please file an [issue](https://github.com/teamradish/TRTwitchPlaysBot/issues). [Pull requests](https://github.com/teamradish/TRTwitchPlaysBot/pulls) are encouraged if you'd like to make contributions.

TRBot is free software; as such, you can run, study, modify, and distribute it for any purpose. See the [License](./LICENSE) for more information.
