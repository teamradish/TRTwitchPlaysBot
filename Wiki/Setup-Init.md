# Getting TRBot
* To get started immediately, download the latest release on the [releases page](https://codeberg.org/kimimaru/TRBot/releases). Keep in mind that there is no schedule for releases, so they may be far behind the latest on the [`develop` branch](https://codeberg.org/kimimaru/TRBot/src/branch/develop).
* To build from source, please follow the [building guide](./Building.md).

# Setting up TRBot
If you installed a pre-built binary, run `TRBot` (Ex. `TRBot.exe` on Windows, `./TRBot` on GNU/Linux). If you built the project, use either `dotnet run` or open the native executable depending on whether the runtime is self-contained or not.

After running TRBot once, it will create a **Data** folder in the same folder you ran it from along with a **TRBotData.db** database file, which holds all your settings. It's highly recommended to first go through the [tutorial on managing TRBot's data](./Managing-Data.md) to learn how to view and modify this data.

# Connecting

## Twitch
By default, TRBot connects to Twitch. This is defined by a `client_service_type` of **1** in the **TRBotData.db** database. If you are connecting through Twitch, TRBot will also create a template file for the login information in this folder. Open the **TwitchLoginSettings.txt** file and fill out the login information for your bot. The settings are described below:

*BotName* = Username of your bot.<br />
*Password* = OAuth token for your bot account. **This HAS to be an OAuth token and cannot be your raw password!** You can generate an OAuth token [here](https://twitchapps.com/tmi/) or [here](https://twitchtokengenerator.com/).<br />
*ChannelName* = The name of the channel to have the bot connect to. Multiple channels are currently not supported.

For security reasons, no user is an Admin or Superadmin by default. To set a user as an Admin or Superadmin, open up the **TRBotData.db** file in SQLite or a database viewer, find the user under the "Users" table, and manually change their level to 40 (Admin) or 50 (Superadmin), then save your changes. If the user is not there

After these are set, run TRBot again and you should see it connect to the channel.
<br />***IMPORTANT:*** If you don't see the bot's connection message on the channel, make sure the channel doesn't have chat restrictions, such as Followers-only, or have the bot account adhere to the restrictions so it can chat.
<br />***IMPORTANT 2:*** To improve the experience of using TRBot in Twitch chat, the bot account should be a VIP or moderator of the channel so it doesn't have restrictions on repeated messages. If you do this, you may want to also raise the [message_throttle_count](./Settings-Documentation.md#message_throttle_count) to the moderator values outlined [here](https://dev.twitch.tv/docs/irc/guide#command--message-limits) so your bot can send more messages.

TRBot internally uses [TwitchLib](https://github.com/TwitchLib/TwitchLib) to handle Twitch connection.

## Terminal (local)
If you'd like to run TRBot locally directly through the terminal, open **TRBotData.db** in the **Data** folder in SQLite or a database viewer, and change the `ValueInt` column of the `client_service_type` setting to **0**, save your changes, then restart TRBot if it's already running.

When running TRBot through the terminal, no other login settings are required or used. Instead, it prompts you for a username to use while the bot is running. This can be an existing user in the database. If no username is specified, it defaults to a user named "terminaluser".

In this mode, TRBot will read all lines you input to the terminal. Simply press Enter/Return after typing what you want to process it.

## WebSocket
You can also have TRBot connect to a WebSocket of choice and read inputs from the data sent through the socket. For instance, this can be an input form on a website. To set the connection to WebSocket, open **TRBotData.db** in the **Data** folder in SQLite or a database viewer, change the `ValueInt` column of the `client_service_type` setting to **2**, then save your changes and restart TRBot.

TRBot will create a template file named **WebSocketConnectionSettings.txt** for you to fill out. The settings are below:

*BotName* = Desired display username of your bot.<br />
*ConnectURL* = The full WebSocket address to connect to. This starts with "ws://" for unsecured connections and "wss://" for secured connections. For example, "ws://127.0.0.1:5333", will connect to a WebSocket on localhost (your own computer) through port 5333.

The WebSocket server will need to send the following JSON response for TRBot to parse it:

```
{
    "user": {
        "Name": "namehere"
    },
    "message": {
        "Text": "text here"
    }
}
```

The `user` field is not required; if omitted, all of TRBot's user-specific features will be unavailable for that response.

TRBot internally uses [websocket-sharp](https://github.com/sta/websocket-sharp), specifically the [WebSocketSharp-netstandard fork](https://github.com/PingmanTools/websocket-sharp/) as a library to handle WebSocket connections.

# Migrating Data from older releases
If you're upgrading TRBot from an older release, please see the [data migration guide](./Migrating-Data.md).

# Next Step - Setting up virtual controllers
Done with this step? Next, [configure the virtual controllers](./Setup-VController.md) so your viewers can play through chat!
