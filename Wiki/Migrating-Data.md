# Migrating Data
## 1.7 to 1.8
Version 1.8 has a minor change in the **InputAccess** data structure. Find the following section in your **BotData.txt** file:

```
"InputAccess": {
    "InputAccessDict": {
      "ss1": 2
    }
}
```

Replace each entry like so:

```
"InputAccess": {
    "InputAccessDict": {
      "ss1": {
        "AccessLevel": 2,
        "AccessType": 0,
        "AccessVal": null
      }
    }
  },
```

The number beside the input name in 1.7 should be the value of "AccessLevel" in 1.8. The "AccessType" and "AccessVal" are not used, yet they are required to load the data.

## 1.8 to 2.0+
Version 2.0 and onwards utilize a SQLite database to manage data. There is a **TRBotDataMigrationTool** designed to migrate your data from 1.8 to 2.0. **Before running the tool, migrate your data from 1.7 to 1.8 if you haven't already!**

Steps for migration:
1. Back up your **BotData.txt** and **Settings.txt** files somewhere safe. The migration tool only reads these files, but you never know. Better to be safe than sorry.
2. Have your **BotData.txt** and **Settings.txt** files in the same directory as the migration tool. They can be anywhere else, but this makes typing in the path easier.
3. Run the migration tool. It will initialize the new database file to store all your data.
4. The migration tool will prompt you for the path to load your **BotData.txt** file.
5. Input the path to **BotData.txt**.
6. Wait for the tool to finish importing your bot data. It will output what data it's importing and if there are any errors importing your data.
7. The migration tool will then ask for the path to load your **Settings.txt** file.
8. Wait for the tool to finish importing your settings. It will notify you if there are any errors importing your settings.
9. That's it!

If you wish to skip loading either **BotData.txt** or **Settings.txt**, you can type "skip" when prompted for the path to the respective file. The migration tool will mention this.

Notes on migrating:

- Some data from 1.8 is either not implemented in TRBot 2.0 or has a vastly different structure and cannot be imported. In most cases, 2.0 has improved equivalents.
- **LoginInfo.txt** has been renamed to **TwitchLoginSettings.txt**. The data is otherwise identical.
- No other data files from your 1.8 install aside from **BotData.txt** and **Settings.txt** are required.

For information on managing your new database, please see [Managing Data](./Managing-Data.md). If you experience errors migrating your data that the error messages don't help with, please [file an issue](https://github.com/teamradish/TRTwitchPlaysBot/issues/new/choose)!

## 2.x to later 2.x
Simply run the newer version of TRBot with your existing database to automatically apply all migrations - that's it! If you encounter a problem, please [file an issue](https://github.com/teamradish/TRTwitchPlaysBot/issues/new/choose)!
