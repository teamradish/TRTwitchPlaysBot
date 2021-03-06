# Building from source
1. Clone the repo with `git clone https://codeberg.org/kimimaru/TRBot.git`
2. [Install the .NET 5.0 SDK and Runtime](https://dotnet.microsoft.com/download/dotnet/5.0)
  * Before installing, set the `DOTNET_CLI_TELEMETRY_OPTOUT` environment variable to 1 if you don't want dotnet CLI commands sending telemetry.

Once you have all the requirements, you can build TRBot using the provided .sln or through the CLI (instructions below). You can also use any code editor or IDE supporting .NET Core, such as VSCode/VSCodium, Visual Studio, or JetBrains Rider.

Command line:
* Main directory: `cd TRBot.Main`
* Building: `dotnet build`
* Publishing: `dotnet publish -c (config) -o (dir) --self-contained --runtime (RID)`
  * config = "Debug" or "Release"
  * dir = output directory
  * [RID](https://raw.githubusercontent.com/dotnet/runtime/main/src/libraries/Microsoft.NETCore.Platforms/src/runtime.json) = usually "win-x64" or "linux-x64". See link for a full list of runtime identifiers.
  * Example: `dotnet publish -c Debug -o ~/Desktop/TRBot --self-contained --runtime linux-x64`

**Note: TRBot runs on all major desktop operating systems, but virtual controller input works only on Windows (vJoy) and GNU/Linux (uinput) since virtual controllers are platform-specific. The virtual controller API is abstracted into an `IVirtualController` interface, making it possible to add new implementations. Please file an issue if your platform isn't supported.**

If you want to build the other applications, such as the data migration tool, run the publish command in the application's directory or build its project in your IDE.

## Native code
Some virtual controllers require native code. These will be in the ["Native" folder of `TRBot.VirtualControllers`](../TRBot/TRBot.VirtualControllers/Native). Below are steps on how to compile them. Note that these components are pre-compiled in source and binary releases of TRBot.

Note: The pre-built uinput virtual controllers were compiled on **Debian** and may not work out of the box with your distribution.

### uinput
To compile the uinput virtual controller implementation, compile `SetupVController.c` with **gcc** as a shared library (`gcc -fPIC -shared SetupVController.c -o SetupVController.so`). Use the newly compiled file in place of the old one.

### vJoy Wrapper
The vJoy C# wrapper code can be found on the [most up-to-date repository](https://github.com/jshafer817/vJoy/tree/master/apps/common/vJoyInterfaceCS/vJoyInterfaceWrap). While unconfirmed, you should be able to compile it with `make`.

Unfortunately, compiling the vJoy driver itself isn't very clear, but 64-bit versions of Windows will not load the driver unless it is signed. Doing this is out-of-scope for this guide, so in short, it's recommended to use the already-signed drivers provided through the official download.

## Migrations
TRBot uses a SQLite database with Entity Framework Core to store and manage its data. If you make code changes to any entities or contexts that affects the database, such as adding/deleting/renaming a column, adding/removing a DbSet, or modifying entity relationships in `OnModelCreating`, you will need to add a new migration:

* Go to the **TRBot.Data** project and run `dotnet ef migrations list` to list all migrations. Take note of the furthest one down, which is the most recent. If needed, you can verify the date prepended to the name of each migration.
* Run `dotnet ef migrations add (migrationhere)`, where "(migrationhere)" is the name of the migration.
  * Example: `dotnet ef migrations add NewUserPermissions`

Afterwards, simply run TRBot to apply the new migrations and update the database.

## Running Tests
All unit tests are in the **TRBot.Tests** project. Some of these tests include parser tests for correctness.

Simply run `dotnet test` inside this directory to run the tests. Please add new tests for code when feasible.

# Contributing
If you find any problems with TRBot, please file an [issue](https://codeberg.org/kimimaru/TRBot/issues/new). [Pull requests](https://codeberg.org/kimimaru/TRBot/pulls) are encouraged if you'd like to make contributions.

TRBot is free software; as such, you can run, study, modify, and distribute it for any purpose. See the [License](../LICENSE) for more information.
