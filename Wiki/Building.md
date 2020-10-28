# Building from source
* Clone the repo with `git clone https://github.com/teamradish/TRTwitchPlaysBot.git`
  * Alternatively, download the zip.
* [.NET Core 3.1 SDK and Runtime](https://dotnet.microsoft.com/download/dotnet-core)
  * Before installing, set the `DOTNET_CLI_TELEMETRY_OPTOUT` environment variable to 1 if you don't want dotnet CLI commands sending telemetry.

You can build TRBot using the provided .sln or through the CLI (instructions below). You can also use any code editor or IDE supporting .NET Core, such as VSCode/VSCodium, Visual Studio, or JetBrains Rider.

Command line:
* Main directory: `cd TRBot.Main`
* Building: `dotnet build`
* Publishing: `dotnet publish -c (config) -o (dir) --self-contained --runtime (RID)`
  * config = "Debug" or "Release"
  * dir = output directory
  * [RID](https://github.com/dotnet/runtime/blob/master/src/libraries/pkg/Microsoft.NETCore.Platforms/runtime.json) = usually "win-x64" or "linux-x64". See link for a full list of runtime identifiers.
  * Example: `dotnet publish -c Debug -o TRBot --self-contained --runtime linux-x64`

**Note: TRBot runs on all major desktop operating systems, but virtual controller input works only on Windows (vJoy) and GNU/Linux (uinput) since virtual controllers are platform-specific. The virtual controller API is abstracted into an `IVirtualController` interface, making it simple to add new implementations. Please file an issue if your platform isn't supported.**

## Migrations
TRBot uses a SQLite database with Entity Framework Core to store and manage its data. If you make code changes to any entities or contexts that affects the database, such as adding/deleting/renaming a column, adding/removing a DbSet, or modifying entity relationships in `OnModelCreating`, you will need to add a new migration:

* Go to the **TRBot.Data** project and run `dotnet ef migrations list` to list all migrations. Take note of the furthest one down, which is the most recent.
* Run `dotnet ef migrations add (migrationhere)`, where "(migrationhere)" is the name of the migration.
  * Example: `dotnet ef migrations add NewUserPermissions`

Afterwards, simply run TRBot to apply the new migrations and update the database.

## Running Tests
All unit tests are in the **TRBot.Tests** project. Simply run `dotnet test` inside this directory to run the tests. Please add new tests for code when possible.

# Contributing
If you find any problems with TRBot, please file an [issue](https://github.com/teamradish/TRTwitchPlaysBot/issues). [Pull requests](https://github.com/teamradish/TRTwitchPlaysBot/pulls) are encouraged if you'd like to make contributions.

TRBot is free software; as such, you can run, study, modify, and distribute it for any purpose. See the [License](https://github.com/teamradish/TRTwitchPlaysBot/blob/master/LICENSE) for more information.
