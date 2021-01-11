TRBot contains several different projects with the purpose of isolating functionality for a more modular approach. This approach allows TRBot to scale more elegantly over time, and it allows each project to use only the parts it needs instead of requiring the entire package.

# Project Structure
## Core Components
* **TRBot.Commands** - Contains code for all commands entered through chat.
* **TRBot.Connection** - Handles sending and receiving events to and from client services and TRBot. You'll find the Twitch integration here.
* **TRBot.Consoles** - The console infrastructure that TRBot uses as the foundation for its input system. Many pre-configured consoles are available.
* **TRBot.Data** - Contains everything relating to data and the SQLite database, including the database context.
* **TRBot.Logging** - Handles logging messages.
* **TRBot.Misc** - Shared code used by various other projects that doesn't fit neatly into any single other project. You'll find the crash handler here.
* **TRBot.Parsing** - Strictly handles TRBot's parser, which transforms text into inputs that TRBot can read.
* **TRBot.Permissions** - Code for the access levels and other moderation features.
* **TRBot.Routines** - Deals with bot routines, or pieces of code that run each tick.
* **TRBot.Utilities** - Contains various utilities, including operating system detection, methods for reading/writing files, algorithms, and other mathematical operations. Relies on no other projects.
* **TRBot.VirtualControllers** - Handles everything related to virtual controllers, including the implementations for each platform and pressing/releasing buttons on them. This also contains the native code required to manage a given virtual controller implementation.

## Applications
* **TRBot.Main** - The main application that utilizes the core components together for a full-fledged bot. This is what you're running with each TRBot release.
* **TRBotDataMigrationTool** - The 1.8 to 2.0+ data migration tool. Converts data from the text files in 1.8 into the SQLite database that versions 2.0 and above use.
* **TRBot.Tests** - Unit tests for TRBot.

## Other
* **Supplementary** - Contains resources and code not directly related to TRBot, such as a ChatterBot instance that users can talk to through TRBot.
