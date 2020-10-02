﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TRBot.Data;

namespace TRBot.Data.Migrations
{
    [DbContext(typeof(BotDBContext))]
    [Migration("20201002013539_CommandData")]
    partial class CommandData
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8");

            modelBuilder.Entity("TRBot.Data.CommandData", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("class_name")
                        .HasColumnType("TEXT");

                    b.Property<int>("level")
                        .HasColumnType("INTEGER");

                    b.Property<string>("name")
                        .HasColumnType("TEXT");

                    b.Property<string>("value_str")
                        .HasColumnType("TEXT");

                    b.HasKey("id");

                    b.HasIndex("name")
                        .IsUnique();

                    b.ToTable("CommandData","commanddata");
                });

            modelBuilder.Entity("TRBot.Data.GameLog", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LogDateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("LogMessage")
                        .HasColumnType("TEXT");

                    b.Property<string>("User")
                        .HasColumnType("TEXT");

                    b.HasKey("id");

                    b.ToTable("GameLogs","gamelogs");
                });

            modelBuilder.Entity("TRBot.Data.SavestateLog", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LogDateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("LogMessage")
                        .HasColumnType("TEXT");

                    b.Property<int>("SavestateNum")
                        .HasColumnType("INTEGER");

                    b.Property<string>("User")
                        .HasColumnType("TEXT");

                    b.HasKey("id");

                    b.ToTable("SavestateLogs","savestatelogs");
                });

            modelBuilder.Entity("TRBot.Data.Settings", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("");

                    b.Property<long>("value_int")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(0L);

                    b.Property<string>("value_str")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("");

                    b.HasKey("id");

                    b.HasIndex("key")
                        .IsUnique();

                    b.ToTable("Settings","settings");
                });
#pragma warning restore 612, 618
        }
    }
}
