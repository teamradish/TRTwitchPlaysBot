# ChatterBot Intro
[ChatterBot](https://github.com/gunthercox/ChatterBot) is a self-hosted chatbot with a simple API. Give it a prompt, and it'll give a response; simple!

TRBot has the ability to interact with your local ChatterBot instance so viewers can talk to a chatbot on stream. This provides an additional level of engagement for viewers.

TRBot provides a [pre-configured ChatterBot instance](../Supplementary/ChatterBot.py) that it can interact with out of the box. It's recommended to use this as a base and configure the code as you see fit.

# Setting up ChatterBot
1. First you will want to install all the dependencies required to run ChatterBot, including Python. See [the official docs](https://chatterbot.readthedocs.io/en/stable/setup.html) for setup instructions.
2. Make sure the `value_int` column of the `chatbot_enabled` setting in the database (**TRBotData.db**) is set to 1. Read [this guide](./Managing-Data.md) if you don't know how to do this. This enables the chatbot.
3. Check the `value_str` column of the `chatbot_socket_hostname` setting and make sure it matches the `SocketHost` in **ChatterBot.py**.
4. Check the `value_int` column of the `chatbot_socket_port` setting and make sure it matches the `SocketPort` in **ChatterBot.py**.
5. Spin up your ChatterBot instance with `python ChatterBot.py` or `python3 ChatterBot.py` and keep it running in a console window.
6. You can talk to your ChatterBot instance through the `ChatbotCommand` on TRBot (default: "!chat"). Type "!chat hi", and, if everything is set up properly, you should see a reply from your ChatterBot instance!
