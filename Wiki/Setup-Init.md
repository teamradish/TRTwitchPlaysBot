# Getting TRBot
* To get started immediately, download the latest release on the [releases page](https://github.com/teamradish/TRTwitchPlaysBot/releases). Keep in mind that there is no schedule for releases, so they may be far behind the latest on the [`develop` branch](https://github.com/teamradish/TRTwitchPlaysBot/tree/develop).
* To build from source, please follow the [building guide](./Building.md).

# Setting up TRBot
If you installed a pre-built binary, run `TRBot` (Ex. `TRBot.exe` on Windows, `./TRBot` on GNU/Linux). If you built the project, use either `dotnet run` or open the native executable depending on whether the runtime is self-contained or not.

After running TRBot once, it will create a **Data** folder in the same folder you ran it from along with a **TRBotData.db** database file, which holds all your settings. It's highly recommended to first go through the [tutorial on managing TRBot's data](./Managing-Data.md) to learn how to view and modify this data.

By default, TRBot connects to Twitch. If you are connecting through Twitch, TRBot will also create a template file for the login information in this folder. Open the **TwitchLoginSettings.txt** file and fill out the login information for your bot. The settings are described below:

*BotName* = Username of your bot.<br />
*Password* = Password for your bot. This may start with "oauth."<br />
*ChannelName* = The name of the channel to have the bot connect to. Multiple channels are not currently supported.

For security reasons, no user is an Admin or Superadmin by default. To set a user as an Admin or Superadmin, open up the **TRBotData.db** file in SQLite or a database viewer, find the user under the "Users" table, and manually change their level to 40 (Admin) or 50 (Superadmin), then save your changes. If the user is not there

After these are set, run TRBot again and you should see it connect to the channel.
<br />***IMPORTANT:*** If you don't see the bot's connection message on the channel, make sure the channel doesn't have chat restrictions, such as Followers-only, or have the bot account adhere to the restrictions so it can chat.
<br />***IMPORTANT 2:*** To improve the experience of using TRBot in Twitch chat, the bot account should be a VIP or moderator of the channel so it doesn't have restrictions on repeated messages.

The bot internally uses [TwitchLib](https://github.com/TwitchLib/TwitchLib) to handle Twitch connection.

## Running via Terminal (optional)
If you'd like to run TRBot locally directly through the terminal, open **TRBotData.db** in the **Data** folder in SQLite or a database viewer, and change the `ValueInt` column of the `client_service_type` setting to 0, save your changes, then restart TRBot if it's already running. Set it back to 1 if you'd like to connect to Twitch once again.

When running TRBot through the terminal, **LoginInformation.txt** isn't used or required at all. Instead, it uses a static user named "terminalUser".

In this mode, TRBot will read all lines you input to the terminal. Simply press Enter/Return after typing what you want to process it.

## Migrating Data from older releases
If you're upgrading TRBot from an older release, please see the [data migration guide](./Migrating-Data.md).

# Next Step - Setting up virtual controllers
Done with this step? Next, [configure the virtual controllers](./Setup-VController.md) so your viewers can play through chat!
