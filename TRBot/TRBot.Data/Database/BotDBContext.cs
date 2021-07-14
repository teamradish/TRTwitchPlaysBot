/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, version 3 of the License.
 *
 * TRBot is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with TRBot.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.IO;
using TRBot.Parsing;
using TRBot.Consoles;
using TRBot.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace TRBot.Data
{
    /// <summary>
    /// Database context for the bot.
    /// </summary>
    public class BotDBContext : DbContext
    {
        //We use properties here to lazy load and avoid needing Set<T>() to load the collections
        public DbSet<Settings> SettingCollection { get; set; } = null;
        public DbSet<GameLog> GameLogs { get; set; } = null; 
        public DbSet<CommandData> Commands { get; set; } = null;
        public DbSet<Meme> Memes { get; set; } = null;
        public DbSet<InputMacro> Macros { get; set; } = null;
        public DbSet<InputSynonym> InputSynonyms { get; set; } = null;
        public DbSet<GameConsole> Consoles { get; set; } = null;
        public DbSet<User> Users { get; set; } = null;
        public DbSet<PermissionAbility> PermAbilities { get; set; } = null;
        public DbSet<RoutineData> Routines { get; set; } = null;

        private string Datasource = string.Empty;

        public BotDBContext()
        {
            //Console.WriteLine("CONTEXT OPENED\n" + Environment.StackTrace);
        }

        public BotDBContext(string dataSource) : this()
        {
            Datasource = dataSource;
        }

        public override void Dispose()
        {
            //Console.WriteLine("CONTEXT DISPOSED\n" + Environment.StackTrace);
            base.Dispose();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseLazyLoadingProxies().UseSqlite($"Filename={Datasource}", ContextBuilder);

            //options.LogTo(Console.WriteLine, LogLevel.Debug);
            //options.EnableSensitiveDataLogging(true);

            base.OnConfiguring(options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Map the tables
            modelBuilder.Entity<Settings>().ToTable("Settings", "settings");
            modelBuilder.Entity<Settings>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Key).HasDefaultValue(string.Empty);
                entity.Property(e => e.ValueStr).HasDefaultValue(string.Empty);
                entity.Property(e => e.ValueInt).HasDefaultValue(0L);
                entity.HasIndex(e => e.Key).IsUnique();
            });

            modelBuilder.Entity<GameLog>().ToTable("GameLogs", "gamelogs");
            modelBuilder.Entity<GameLog>(entity => 
            {
                entity.HasKey(e => e.ID);
            });

            modelBuilder.Entity<CommandData>().ToTable("CommandData", "commanddata");
            modelBuilder.Entity<CommandData>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Level).HasDefaultValue(0);
                entity.Property(e => e.Enabled);
                entity.Property(e => e.DisplayInList);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<Meme>().ToTable("Memes", "memes");
            modelBuilder.Entity<Meme>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.HasIndex(e => e.MemeName).IsUnique();
            });

            modelBuilder.Entity<InputMacro>().ToTable("Macros", "macros");
            modelBuilder.Entity<InputMacro>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.MacroName).HasDefaultValue(string.Empty);
                entity.Property(e => e.MacroValue).HasDefaultValue(string.Empty);
                entity.HasIndex(e => e.MacroName).IsUnique();
            });

            modelBuilder.Entity<InputSynonym>().ToTable("InputSynonyms", "inputsynonyms");
            modelBuilder.Entity<InputSynonym>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.SynonymName).HasDefaultValue(string.Empty);
                entity.Property(e => e.SynonymValue).HasDefaultValue(string.Empty);
                entity.Property(e => e.ConsoleID).HasDefaultValue(1);
                entity.HasIndex(e => new { e.SynonymName, e.ConsoleID }).IsUnique();
            });

            modelBuilder.Entity<GameConsole>().ToTable("Consoles", "consoles");
            modelBuilder.Entity<GameConsole>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Name).HasDefaultValue("GameConsole");
                entity.Ignore(e => e.ConsoleInputs);
                entity.HasMany(e => e.InputList).WithOne(c => c.Console).HasForeignKey(c => c.ConsoleID).IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.InvalidCombos).WithOne().OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<InputData>().ToTable("Inputs", "inputs");
            modelBuilder.Entity<InputData>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Name).HasDefaultValue(string.Empty);
                entity.Property(e => e.ButtonValue).HasDefaultValue(0);
                entity.Property(e => e.AxisValue).HasDefaultValue(0);
                entity.Property(e => e.InputType).HasDefaultValue(InputTypes.Blank).HasConversion(new EnumToNumberConverter<InputTypes, int>());
                entity.Property(e => e.MinAxisVal).HasDefaultValue(0d);
                entity.Property(e => e.MaxAxisVal).HasDefaultValue(1d);
                entity.Property(e => e.MaxAxisPercent).HasDefaultValue(100d);
                entity.Property(e => e.Enabled).HasDefaultValue(1);
                entity.HasIndex(e => new { e.Name, e.ConsoleID }).IsUnique();
            });

            modelBuilder.Entity<InvalidCombo>().ToTable("InvalidInputCombos", "invalidinputcombos");
            modelBuilder.Entity<InvalidCombo>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.HasOne(e => e.Input).WithOne().HasForeignKey<InvalidCombo>(c => c.InputID).IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => e.InputID).IsUnique();
            });

            modelBuilder.Entity<User>().ToTable("Users", "users");
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Name).HasDefaultValue(string.Empty);
                entity.HasOne(e => e.Stats).WithOne(u => u.user).HasForeignKey<UserStats>(u => u.UserID).IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.UserAbilities).WithOne(c => c.user).HasForeignKey(u => u.UserID).IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.RestrictedInputs).WithOne(e => e.user).HasForeignKey(e => e.UserID).IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.RecentInputs).WithOne(e => e.user).HasForeignKey(e => e.UserID).IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => e.Name);
            });

            modelBuilder.Entity<UserStats>().ToTable("UserStats", "userstats");
            modelBuilder.Entity<UserStats>(entity =>
            {
                entity.HasKey(e => e.ID);
            });

            modelBuilder.Entity<PermissionAbility>().ToTable("PermissionAbilities", "permissionabilities");
            modelBuilder.Entity<PermissionAbility>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.AutoGrantOnLevel).HasConversion(new EnumToNumberConverter<PermissionLevels, int>());
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<UserAbility>().ToTable("UserAbilities", "userabilities");
            modelBuilder.Entity<UserAbility>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.HasOne(e => e.PermAbility).WithOne().HasForeignKey<UserAbility>(u => u.PermabilityID).IsRequired();
                entity.HasIndex(e => new { e.UserID, e.PermabilityID }).IsUnique();
            });

            modelBuilder.Entity<RestrictedInput>().ToTable("RestrictedInputs", "restrictedinputs");
            modelBuilder.Entity<RestrictedInput>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.HasOne(e => e.inputData).WithOne().HasForeignKey<RestrictedInput>(e => e.InputID).IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.UserID, e.InputID }).IsUnique();
            });

            modelBuilder.Entity<RecentInput>().ToTable("RecentInputs", "recentinputs");
            modelBuilder.Entity<RecentInput>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.HasIndex(e => e.UserID);
            });

            modelBuilder.Entity<RoutineData>().ToTable("RoutineData", "routinedata");
            modelBuilder.Entity<RoutineData>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Name).HasDefaultValue(string.Empty);
                entity.Property(e => e.ClassName).HasDefaultValue(string.Empty);
                entity.Property(e => e.Enabled).HasDefaultValue(1L);
                entity.Property(e => e.ResetOnReload).HasDefaultValue(0L);
                entity.Property(e => e.ValueStr).HasDefaultValue(string.Empty);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }

        private void ContextBuilder(SqliteDbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.MigrationsAssembly(System.Reflection.Assembly.GetExecutingAssembly().FullName);
        }
    }
}