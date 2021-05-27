# ChatterBot Intro
[ChatterBot](https://github.com/gunthercox/ChatterBot) is a self-hosted chatbot with a simple API. Give it a prompt, and it'll give a response; simple!

TRBot has the ability to interact with your local ChatterBot instance so viewers can talk to a chatbot on stream. This provides an additional level of engagement for viewers.

TRBot provides a [pre-configured ChatterBot instance](../Supplementary/ChatterBot.py) that it can interact with out of the box. It's recommended to use this as a base and configure the code as you see fit.

# Setting up ChatterBot
1. First you will want to install all the dependencies required to run ChatterBot, including Python. See [the official docs](https://chatterbot.readthedocs.io/en/stable/setup.html) for setup instructions.
2. Make sure the `value_int` column of the `chatbot_enabled` setting in the database (**TRBotData.db**) is set to 1. Read [this guide](./Managing-Data.md) if you don't know how to do this. This enables the chatbot.
3. Check the `value_str` column of the `chatbot_socket_path` setting and make sure it matches the **absolute path** of `SocketPath` in **ChatterBot.py**. If `chatbot_socket_path_is_relative` is 0, `chatbot_socket_path` is an absolute path, otherwise it's relative to the folder of the TRBot executable. `chatbot_socket_path_is_relative` is 0 by default. The recommended and default value of `chatbot_socket_path` is a file named "ChatterBotSocket" in the OS temp folder (Ex. "/tmp/ChatterBotSocket" on GNU/Linux). The socket file generated is how TRBot and ChatterBot interact with each other, so **the paths must point to the same file**.
  - Example: If `chatbot_socket_path` = "/MyFolder/ChatterBotSocket" and `chatbot_socket_path_is_relative` = 1, the path is **(TRBotExecutable)/MyFolder/ChatterBotSocket**. If `chatbot_socket_path_is_relative` = 0, the full path would be **MyFolder/ChatterBotSocket**.
4. Spin up your ChatterBot instance with `python3 ChatterBot.py` and keep it running in a console window.
5. You can talk to your ChatterBot instance through the `ChatbotCommand` on TRBot (default, mapped to "chat"). Type "!chat hi", and, if everything is set up properly, you should see a reply from your ChatterBot instance!
