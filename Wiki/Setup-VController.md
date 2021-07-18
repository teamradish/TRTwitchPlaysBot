# Setting up virtual controllers for inputs
## GNU/Linux
Make sure the `uinput` kernel module is enabled with `sudo modprobe uinput`. Unless TRBot is run as root, you'll also need permissions to read and write in `/dev/uinput` with `sudo chmod a+rw /dev/uinput`. TRBot creates and manages the `uinput` virtual controllers on GNU/Linux automatically, so there is nothing to install.

If the native code fails to run for your distro, please follow [these instructions](./Building.md#uinput) for building it on your machine.

## Windows
TRBot uses vJoy on Windows to send inputs to the game. While TRBot has no trouble parsing inputs that come through, the inputs won't be able to reach your games if vJoy isn't set up.

1. Install [vJoy](https://sourceforge.net/projects/vjoystick/files/Beta/Configurable/CC290512/). Click on "Download Latest Version" and run the setup file.
2. After installation, run a newly installed program called "Configure vJoy". Set up the number of devices you want and configure them. **Make sure that at least 32 buttons are mapped on each virtual controller to ensure enough inputs are available**. Both vJoy and TRBot are capable of handling up to 128 buttons, but keep in mind that most games and emulators may not be able to map button values past 32.

## Testing Virtual Controller Configuration
Your virtual controllers should be set up and good to go right now. Run TRBot and check the console window to see if it's able to acquire those virtual controllers. If everything looks good, you should next test out how the inputs work.

**IMPORTANT: The default console for TRBot is NES, which has no analog inputs. If you wish to test analog inputs, switch to a console with them, such as the GameCube, with the [`GetSetConsoleCommand`](../TRBot/TRBot.Commands/Commands/GetSetConsoleCommand.cs) (default: "!console") via "!console gc". You will need an access level of Moderator (30) by default to switch the console (see the [initial tutorial](./Setup-Init.md#connecting)).**

A simple way to test actual inputs is on the [Dolphin](https://dolphin-emu.org/) emulator using the [provided vJoy or uinput controller profiles in this repository](../Controller%20Configs/Dolphin). You can set the mappings in the emulator using text inputs through TRBot. After setting each mapping, you can test it out by typing the same input again. For vJoy, the *"vJoy Feeder (Demo)"* program can also be used to press buttons and axes on the virtual controllers. If everything was configured correctly, you should see buttons being pressed and axes being moved in Dolphin's controller configuration screen after typing the associated inputs in the bot.

Alternatively, you can use another application to test joystick inputs. One such application on GNU/Linux is **jstest-gtk**; on Windows, **XOutput** is an option.

**NOTE: Make sure background input is enabled in the emulator, as this will make things much easier! If you don't see an option in the menus, check any config files. For instance, BizHawk has it in its config file as `AcceptBackgroundInput`.**

If everything looks good at this point, viewers should now be capable of playing games through your chat!

**NOTE: For PC games, the "PC" console is typically reserved for the experimental xdotool virtual controller on GNU/Linux. It's recommended to [create a new console with custom inputs or modify an existing one](./Adding-ConsolesInputs.md) catered to your game.**

# Next step - Miscellaneous
Done with this step? Check the [next section](./Setup-Misc.md) for additional settings, tools for displaying chat on stream, and more!
