Welcome to the TRBot wiki! This short guide should help get you up and running with TRBot, your own Twitch Plays bot!

#### Table of Contents
* [Getting TRBot](#getting-trbot)
* [Setting up TRBot](#setting-up-trbot)
  * [Running via Terminal (optional)](#running-via-terminal-optional)
* [Setting up virtual controllers for inputs](#setting-up-virtual-controllers-for-inputs)
  * [GNU/Linux](#gnu-linux)
  * [Windows](#windows)
  * [Testing Virtual Controller Configuration](#testing-virtual-controller-configuration)
* [Additional Notes & Utilities](#additional-notes)
  * [Game Message](#game-message)
  * [Displaying Twitch Chat](#displaying-twitch-chat)
  * [Displaying Game Inputs](#displaying-game-inputs)
  * [Timer Options](#timer-options)
  * [PC Games](#pc-games)
* [Contributing](#contributing)


# Getting TRBot
* To get started immediately, download the latest release on the [releases page](https://github.com/teamradish/TRTwitchPlaysBot/releases). Keep in mind that there is no schedule for releases, so they may be far behind the latest on develop.
* To build from source, please follow the [building guide](https://github.com/teamradish/TRTwitchPlaysBot/Wiki/Building.md).

# Setting up TRBot
If you installed a pre-built binary, run `TRBot` (Ex. `TRBot.exe` on Windows, `./TRBot` on GNU/Linux). If you built the project, use either `dotnet run` or open the native executable depending on whether the runtime is self-contained or not.

After running TRBot once, it will create a **Data** folder in the same folder you ran it from along with a **TRBotData.db** database file, which holds all your settings. If you are connecting through Twitch or another online service, TRBot will also create a template file for the login information in this folder. Open the **LoginInfo.txt** file and fill out the login information for your bot. The settings are described below:

*BotName* = Username of your bot.<br />
*Password* = Password for your bot. This may start with "oauth."<br />
*ChannelName* = The name of the channel to have the bot connect to. Multiple channels are not currently supported.

For security reasons, no user is an Admin or Superadmin by default. To set a user as an Admin or Superadmin, open up the **TRBotData.db** file in SQLite or a database viewer, find the user under the "Users" table, and manually change their level to 40 (Admin) or 50 (Superadmin), then save your changes. 

After these are set, run TRBot again and you should see it connect to the channel.
<br />***IMPORTANT:*** If you don't see the bot's connection message on the channel, make sure the channel doesn't have chat restrictions, such as Followers-only, or have the bot account adhere to the restrictions so it can chat.
<br />***IMPORTANT 2:*** To improve the experience of using TRBot in Twitch chat, the bot account should be a moderator of the channel so it doesn't have restrictions on repeated messages.

The bot internally uses [TwitchLib](https://github.com/TwitchLib/TwitchLib) to handle connection to Twitch.

## Running via Terminal (optional)
If you'd like to run TRBot locally directly through the terminal, open **TRBotData.db** in the **Data** folder in SQLite or a database viewer, and change the `value_int` of the `client_service_type` setting to 0, save your changes, then restart TRBot if it's already running. Set it back to 1 if you'd like to connect to Twitch once again.

When running TRBot through the terminal, **LoginInformation.txt** isn't used or required at all. Instead, it uses a static user named "terminalUser".

In this mode, TRBot will read all lines you input to the terminal. Simply press Enter/Return after typing what you want to process it.

# Setting up virtual controllers for inputs
## GNU/Linux
Make sure the `uinput` kernel module is enabled with `sudo modprobe uinput`. Unless TRBot is run as root, you'll also need permissions to read and write in `/dev/uinput` with `sudo chmod a+rw /dev/uinput`. TRBot creates and manages the `uinput` virtual controllers on GNU/Linux automatically, so there is nothing to install.

If the native code fails to run for your distro, head to the `TRBot.VirtualControllers/Native` folder and compile `SetupVController.c` with **gcc** as a shared library (`gcc -fPIC -shared SetupVController.c -o SetupVController.so`). Use the newly compiled .so file in place of the old one and run the bot again.

## Windows
TRBot uses vJoy on Windows to send inputs to the game. While it has no trouble parsing inputs that come through chat, the inputs won't be able to reach the games you're streaming if you don't have vJoy set up.

1. Install [vJoy](https://sourceforge.net/projects/vjoystick/files/Beta/Configurable/CC290512/). Click on "Download Latest Version" and run the setup file.
2. After installation, run a newly installed program called "Configure vJoy". Set up the number of devices you want and configure them. **Make sure that at least 32 buttons are mapped on each virtual controller to ensure enough inputs are available**. Both vJoy and TRBot are capable of handling up to 128 buttons, but keep in mind that some emulators may not be able to map button values past 32.

## Testing Virtual Controller Configuration
Your virtual controllers should be set up and good to go right now. Run TRBot and check the console window to see if it's able to acquire the virtual controllers set up. If everything looks good, you should next test out how the inputs work.

**IMPORTANT: The default console for TRBot is NES, which has no analog inputs. If you wish to test analog inputs, switch to a console with them, such as Gamecube, with `!console gc`. You will need an access level of admin to switch the console (covered in [Setting up TRBot](#setting-up-trbot)).**

A simple way to test actual inputs is on the [Dolphin](https://dolphin-emu.org/) emulator using the [provided vJoy or uinput controller profiles in this repository](https://github.com/teamradish/TRTwitchPlaysBot/tree/master/Emulator%20Controller%20Configs/Dolphin). Once the mappings are set in the emulator (you can set them using chat inputs as well), you can test it out by typing inputs with the bot (for vJoy, the *"vJoy Feeder (Demo)"* program will also work). You should see buttons being pressed and axes being moved in Dolphin's controller configuration screen if everything was configured correctly.

Alternatively, you can use another application to test joystick inputs. One such application on GNU/Linux is **jstest-gtk**.

**NOTE: Make sure background input is enabled in the emulator, as this will make things much easier! If you don't see an option in the menus, check any config files. For instance, BizHawk has it in its config file as `AcceptBackgroundInput`.**

If everything looks okay at this point, viewers should now be capable of playing games through your chat!

# Additional Notes
## Game Message
TRBot has an optional game message that can be set with the `SetMessageCommand`. An example of such a message may be "Beat level 1". Once set, the message is saved into a **GameMessage.txt** file in the **Data** folder. You can display this message on screen using OBS via the following steps:

1. Create Text (GDI+, FreeType2, etc.)
2. Check the box labeled "Read from file"
3. Browse and select **GameMessage.txt**

Now the message should be displayed on screen for all your viewers to see! Whenever the message is changed through the bot, it will be updated in the file and subsequently on screen.

## Displaying Twitch Chat
There are several options for displaying Twitch chat on your stream:

* [ChatGameFontificator](https://github.com/GlitchCog/ChatGameFontificator) is very easy to set up, customizable, and has themes for many games out of the box.
* [KapChat](https://www.nightdev.com/kapchat/) can be set up in a few minutes and is capable of displaying Twitch chat, including emotes, through OBS and other streaming software.
* [Restream Chat](https://restream.io/chat)
* Find another API or write your own chat display using the messages that come in through the bot.

## Displaying Game Inputs
* Many emulators support displaying inputs directly.
* [Open Joystick Display](https://github.com/KernelZechs/open-joystick-display) is easy to use, supports many game controllers, and has a streaming mode, which displays only the controller.
* Write your own input display using TRBot's virtual controller capabilities:
  * `IVirtualController.GetInputState` can tell if a given input name is pressed on the controller.
  * `IVirtualController.GetButtonState` can tell if a given button number is pressed on the controller.
  * `IVirtualController.GetAxisState` tells the percentage a given axis is moved in. 

## Timer Options
* [obs-advanced-timer](https://github.com/cg2121/obs-advanced-timer) for a non-I/O intensive timer on OBS (especially good for HDDs).
  * [This fork](https://github.com/tdeeb/obs-advanced-timer/tree/CountupStart) adds the ability to start a countup timer from a given time ("CountupStart" branch).

## PC Games
Inputs should work for PC games that can recognize the virtual controllers. There is experimental keyboard and mouse controls accessible on GNU/Linux if [`xdotool`](https://www.semicomplete.com/projects/xdotool/) is installed.

**Be very careful when playing PC games!** Make sure that players can't exit the game, access files or perform any other malicious activities, such as shutting down the system. If you're streaming a PC game, highly consider capturing only the window with the game and not the entire display; this way if players manage to exit the game, they won't be able to see anything else on your computer. This is easier to accomplish by playing the game in windowed mode.

If there's no way to prevent players from exiting the game, you can set up a script to restart the game once it's closed. With this, OBS should also detect that the game window is available and display it on stream again. Here's an example for **bash**:
```
(while true; do 
    ./mygame
done)
```

This may not work for games that have DRM and can only be launched through a client, such as Steam.

# Contributing
If you find any problems with TRBot, please file an [issue](https://github.com/teamradish/TRTwitchPlaysBot/issues). [Pull requests](https://github.com/teamradish/TRTwitchPlaysBot/pulls) are encouraged if you'd like to make contributions.

TRBot is free software; as such, you can run, study, modify, and distribute it for any purpose. See the [License](https://github.com/teamradish/TRTwitchPlaysBot/blob/master/LICENSE) for more information.
