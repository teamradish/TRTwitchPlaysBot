# LiveSplitOne Intro
[LiveSplitOne](https://github.com/LiveSplit/LiveSplitOne) is a cross-platform version of the popular LiveSplit application. With LiveSplitOne, you can display a speedrun timer and track time on your playthroughs.

TRBot has the ability to directly control a locally deployed instance of LiveSplitOne through the [LiveSplitOneCommand](../TRBot/TRBot.Integrations/LiveSplitOne/Commands/LiveSplitOneCommand.cs). Internally, this sets up a WebSocket server which LiveSplitOne can then connect to and receive messages from.

There is some setup required, so keep reading on to see how to achieve this integration.

# Setting up database entries
Since TRBot's integration code is strictly separated from the other libraries, you will need to manually add the required data to the database. If you do not know how to modify TRBot's database, please see the [managing data guide](./Managing-Data.md).
1. Open your database file by however means you prefer.
2. In the **CommandData** table, add a new record. Set the new record's `ClassName` to **TRBot.Integrations.LiveSplitOne.LiveSplitOneCommand**. The other columns are your preference, but it's recommended to have `Level` as Superadmin (50) and `Enabled` to 1 so the command is immediately available.
3. In the **Settings** table, add a new record with the `Key` as **lso_websocket_port_num**. Set its `ValueInt` to the port to host the WebSocket server on (default is 4347).
4. In the **Settings** table, add a new record with the `Key` as **lso_websocket_path**. Set its `ValueStr` to the relative path for the WebSocket service (default is "/", the root).

Steps 3 and 4 are optional; if they aren't found, they'll use the defaults of 4347 and "/", respectively. If you have other WebSocket services running on your machine, you may want to change these values.

Now simply run TRBot, and the WebSocket server will be started automatically. As the WebSocket server is currently tied to the LiveSplitOne command, the server will be open as long as this command is present in memory.

# Connecting with LiveSplitOne
Before you begin, please build and deploy your own LiveSplitOne instance following [these instructions](https://github.com/LiveSplit/LiveSplitOne#build-instructions). **The live version of LiveSplitOne on the web will not work!**

Once the page is up, click "Connect to Server" on the left. Type in "ws://127.0.0.1:(port)(path)", substituting "(port)" for the port of the server and "(path)" for the path of the service if it's not the root. For example, the default is "ws://127.0.0.1:4347". If the port was 6754 and the path was "/lso", you would enter "ws://127.0.0.1:6754/lso". If everything works out, you will establish a connection and see a "Disconnect" button on the left of the LiveSplitOne interface.

Remember that **TRBot must be running to keep the server alive!**

# Controlling LiveSplitOne
LiveSplitOne has several commands you can issue to control it, all available through TRBot's [LiveSplitOneCommand](../TRBot/TRBot.Integrations/LiveSplitOne/Commands/LiveSplitOneCommand.cs). Simply invoke the command without any arguments, and it will tell you all the available LiveSplitOne commands you can issue. As LiveSplitOne is still in development, these commands may change over time.

Some examples (assuming the command is named "split" in the database):
- "!split start" - Starts a split.
- "!split reset" - Resets the timer.
- "!split togglepause" - Pauses/unpauses the timer.
- "!split setgametime 5" - Sets the game timer to have 5 seconds on the clock.
