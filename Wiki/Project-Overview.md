* The _Classes_ folder contains all of the code you'll find in TRBot
  * **BotProgram** is the main code that connects everything together.
  * **BotMessageHandler** handles sending messages with rate limiting.
  * **CommandHandler** stores all the bot commands for quick fetching and execution.
  * **ClientServices** is where connections to the streaming service (such as Twitch) are handled. The base interface is **IClientService**.
  * **Commands** contains all the bot commands. You can find code for commands such as "!say" here.
  * **DataObjects** contains all the bot data, such as game logs, user objects, input access, and more.
  * **EventHandlers** is where the event handlers reside. They are used with the client service.
  * **Input** is where anything input-related is handled. You can find the virtual controllers and their implementations, consoles, and the input parser here.
    * **Parser** is the input parser, transforming text into inputs.
    * **InputHandler** is where inputs are actually carried out to the virtual controllers.
    * **InputGlobals** is a helper class to facilitate input-related functions, such as setting virtual controllers and consoles and obtaining a list of valid inputs for the current console.
    * **IVirtualControllerManager** is the interface for virtual controller managers.
    * **IVirtualController** is the interface for virtual controllers.
  * **Routines** contains all non-command routines that are periodically run and updated, such as reconnection and the periodic message.
  * **Native** contains all native code and wrappers. You can find the native C code for setting up virtual controllers with uinput on Linux here.
  * **Supplementary** contains anything not directly related to the bot. You can find the Python code for setting up a local ChatterBot instance for viewers to interact with here.
