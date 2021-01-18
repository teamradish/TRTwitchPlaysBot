#!/bin/bash
# This bash script will build TRBot and the migration tool for both Windows and GNU/Linux
# Run it in the same directory as this file

# Make a "build" folder if it doesn't exist
mkdir -p build

# Change directory to the main application
cd ./TRBot/TRBot.Main

# TRBot GNU/Linux build
dotnet publish -c Release -o ../../build/TRBotLinux --self-contained --runtime linux-x64

# TRBot Windows build
dotnet publish -c Release -o ../../build/TRBotWin --self-contained --runtime win-x64

# Change directory to the data migration tool 
cd ../TRBotDataMigrationTool

# Migration tool GNU/Linux build
dotnet publish -c Release -o ../../build/TRBotDataMigrationToolLinux --self-contained --runtime linux-x64

# Migration tool Windows build
dotnet publish -c Release -o ../../build/TRBotDataMigrationToolWindows --self-contained --runtime win-x64

printf "\nBuilds complete!\n"
