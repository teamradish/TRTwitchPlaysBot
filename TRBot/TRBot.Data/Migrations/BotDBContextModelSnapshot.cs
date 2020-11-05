﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TRBot.Data;

namespace TRBot.Data.Migrations
{
    [DbContext(typeof(BotDBContext))]
    partial class BotDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0-rc.2.20475.6");

            modelBuilder.Entity("TRBot.Consoles.GameConsole", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("GameConsole");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Consoles", "consoles");
                });

            modelBuilder.Entity("TRBot.Consoles.InputData", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AxisValue")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(0);

                    b.Property<int>("ButtonValue")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(0);

                    b.Property<int>("ConsoleID")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Enabled")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(1L);

                    b.Property<int>("InputType")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(0);

                    b.Property<long>("Level")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaxAxisPercent")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(100);

                    b.Property<int>("MaxAxisVal")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(1);

                    b.Property<int>("MinAxisVal")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(0);

                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("");

                    b.HasKey("ID");

                    b.HasIndex("ConsoleID");

                    b.HasIndex("Name", "ConsoleID")
                        .IsUnique();

                    b.ToTable("Inputs", "inputs");
                });

            modelBuilder.Entity("TRBot.Consoles.InvalidCombo", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("GameConsoleID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("InputID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("GameConsoleID");

                    b.HasIndex("InputID")
                        .IsUnique();

                    b.ToTable("InvalidInputCombos", "invalidinputcombos");
                });

            modelBuilder.Entity("TRBot.Data.CommandData", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClassName")
                        .HasColumnType("TEXT");

                    b.Property<long>("DisplayInList")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Level")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(0L);

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("ValueStr")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("CommandData", "commanddata");
                });

            modelBuilder.Entity("TRBot.Data.GameLog", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LogDateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("LogMessage")
                        .HasColumnType("TEXT");

                    b.Property<string>("User")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("GameLogs", "gamelogs");
                });

            modelBuilder.Entity("TRBot.Data.Meme", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("MemeName")
                        .HasColumnType("TEXT");

                    b.Property<string>("MemeValue")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("MemeName")
                        .IsUnique();

                    b.ToTable("Memes", "memes");
                });

            modelBuilder.Entity("TRBot.Data.RestrictedInput", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("Expiration")
                        .HasColumnType("TEXT");

                    b.Property<int>("InputID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("InputID")
                        .IsUnique();

                    b.HasIndex("UserID", "InputID")
                        .IsUnique();

                    b.ToTable("RestrictedInputs", "restrictedinputs");
                });

            modelBuilder.Entity("TRBot.Data.Settings", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("");

                    b.Property<long>("ValueInt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(0L);

                    b.Property<string>("ValueStr")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("");

                    b.HasKey("ID");

                    b.HasIndex("Key")
                        .IsUnique();

                    b.ToTable("Settings", "settings");
                });

            modelBuilder.Entity("TRBot.Data.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("ControllerPort")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Level")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("");

                    b.HasKey("ID");

                    b.HasIndex("Name");

                    b.ToTable("Users", "users");
                });

            modelBuilder.Entity("TRBot.Data.UserAbility", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("Expiration")
                        .HasColumnType("TEXT");

                    b.Property<long>("GrantedByLevel")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PermabilityID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ValueInt")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ValueStr")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("PermabilityID")
                        .IsUnique();

                    b.HasIndex("UserID", "PermabilityID")
                        .IsUnique();

                    b.ToTable("UserAbilities", "userabilities");
                });

            modelBuilder.Entity("TRBot.Data.UserStats", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("AutoPromoted")
                        .HasColumnType("INTEGER");

                    b.Property<long>("BetCounter")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Credits")
                        .HasColumnType("INTEGER");

                    b.Property<long>("IgnoreMemes")
                        .HasColumnType("INTEGER");

                    b.Property<long>("IsSubscriber")
                        .HasColumnType("INTEGER");

                    b.Property<long>("OptedOut")
                        .HasColumnType("INTEGER");

                    b.Property<long>("TotalMessageCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserID")
                        .HasColumnType("INTEGER");

                    b.Property<long>("ValidInputCount")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("UserID")
                        .IsUnique();

                    b.ToTable("UserStats", "userstats");
                });

            modelBuilder.Entity("TRBot.Parsing.InputMacro", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("MacroName")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("");

                    b.Property<string>("MacroValue")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("");

                    b.HasKey("ID");

                    b.HasIndex("MacroName")
                        .IsUnique();

                    b.ToTable("Macros", "macros");
                });

            modelBuilder.Entity("TRBot.Parsing.InputSynonym", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ConsoleID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(1);

                    b.Property<string>("SynonymName")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("");

                    b.Property<string>("SynonymValue")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("");

                    b.HasKey("ID");

                    b.HasIndex("SynonymName", "ConsoleID")
                        .IsUnique();

                    b.ToTable("InputSynonyms", "inputsynonyms");
                });

            modelBuilder.Entity("TRBot.Permissions.PermissionAbility", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AutoGrantOnLevel")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MinLevelToGrant")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("ValueInt")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ValueStr")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("PermissionAbilities", "permissionabilities");
                });

            modelBuilder.Entity("TRBot.Consoles.InputData", b =>
                {
                    b.HasOne("TRBot.Consoles.GameConsole", "Console")
                        .WithMany("InputList")
                        .HasForeignKey("ConsoleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Console");
                });

            modelBuilder.Entity("TRBot.Consoles.InvalidCombo", b =>
                {
                    b.HasOne("TRBot.Consoles.GameConsole", null)
                        .WithMany("InvalidCombos")
                        .HasForeignKey("GameConsoleID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TRBot.Consoles.InputData", "Input")
                        .WithOne()
                        .HasForeignKey("TRBot.Consoles.InvalidCombo", "InputID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Input");
                });

            modelBuilder.Entity("TRBot.Data.RestrictedInput", b =>
                {
                    b.HasOne("TRBot.Consoles.InputData", "inputData")
                        .WithOne()
                        .HasForeignKey("TRBot.Data.RestrictedInput", "InputID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TRBot.Data.User", "user")
                        .WithMany("RestrictedInputs")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("inputData");

                    b.Navigation("user");
                });

            modelBuilder.Entity("TRBot.Data.UserAbility", b =>
                {
                    b.HasOne("TRBot.Permissions.PermissionAbility", "PermAbility")
                        .WithOne()
                        .HasForeignKey("TRBot.Data.UserAbility", "PermabilityID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TRBot.Data.User", "user")
                        .WithMany("UserAbilities")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PermAbility");

                    b.Navigation("user");
                });

            modelBuilder.Entity("TRBot.Data.UserStats", b =>
                {
                    b.HasOne("TRBot.Data.User", "user")
                        .WithOne("Stats")
                        .HasForeignKey("TRBot.Data.UserStats", "UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("TRBot.Consoles.GameConsole", b =>
                {
                    b.Navigation("InputList");

                    b.Navigation("InvalidCombos");
                });

            modelBuilder.Entity("TRBot.Data.User", b =>
                {
                    b.Navigation("RestrictedInputs");

                    b.Navigation("Stats");

                    b.Navigation("UserAbilities");
                });
#pragma warning restore 612, 618
        }
    }
}
