TRBot contains several different projects with the purpose of isolating functionality. This approach allows TRBot to scale more elegantly over time, and it allows each project to use only the parts it needs instead of requiring the entire package.

# Project Structure
* **TRBot.Commands** - Contains code for all commands entered through chat.
* **TRBot.Connection** - Handles sending and receiving events to and from client services and TRBot. You'll find the Twitch integration here.
* **TRBot.Consoles** - The console infrastructure that TRBot uses as the foundation for its input system. Many pre-configured consoles are available.
* **TRBot.Data** - Contains everything relating to the database, including the database context.
* **TRBot.Main** - The main application that connects everything together. This is what you are running with a binary release.
* **TRBot.Misc** - Shared code used by various other projects that doesn't fit neatly into any single other project. You'll find the crash handler here.
* **TRBot.Parsing** - Strictly handles TRBot's parser, which transforms text into inputs that TRBot can read.
* **TRBot.Permissions** - Code for the access levels and other moderation features.
* **TRBot.Routines** - Deals with bot routines, or pieces of code that run each tick.
* **TRBot.Tests** - Unit tests for TRBot.
* **TRBot.Utilities** - Contains various utilities, including operating system detection, methods for reading/writing files, algorithms and other mathematical operations. Relies on no other projects.
* **TRBot.VirtualControllers** - Handles everything related to virtual controllers, including the implementations for each platform and pressing/releasing buttons on them. This also contains the native code required to manage a given virtual controller implementation.
* **Supplementary** - Contains resources and code not directly related to TRBot, such as a ChatterBot instance that users can talk to through TRBot.
