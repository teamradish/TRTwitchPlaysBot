/* This file is part of TRBot.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
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
        public DbSet<SavestateLog> SavestateLogs { get; set; } = null;
        public DbSet<CommandData> Commands { get; set; } = null;
        public DbSet<Meme> Memes { get; set; } = null;
        public DbSet<InputMacro> Macros { get; set; } = null;
        public DbSet<InputSynonym> InputSynonyms { get; set; } = null;
        public DbSet<GameConsole> Consoles { get; set; } = null;
        public DbSet<User> Users { get; set; } = null;
        public DbSet<PermissionAbility> PermAbilities { get; set; } = null;

        private string Datasource = string.Empty;

        public BotDBContext()
        {
            
        }

        public BotDBContext(string dataSource) : this()
        {
            Datasource = dataSource;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Filename={Datasource}", ContextBuilder);
            options.UseLazyLoadingProxies();
            base.OnConfiguring(options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Map the tables
            modelBuilder.Entity<Settings>().ToTable("Settings", "settings");
            modelBuilder.Entity<Settings>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.Property(e => e.key).HasDefaultValue(string.Empty);
                entity.Property(e => e.value_str).HasDefaultValue(string.Empty);
                entity.Property(e => e.value_int).HasDefaultValue(0L);
                entity.HasIndex(e => e.key).IsUnique();
            });

            modelBuilder.Entity<GameLog>().ToTable("GameLogs", "gamelogs");
            modelBuilder.Entity<GameLog>(entity => 
            {
                entity.HasKey(e => e.id);
            });

            modelBuilder.Entity<SavestateLog>().ToTable("SavestateLogs", "savestatelogs");
            modelBuilder.Entity<SavestateLog>(entity =>
            {
                entity.HasKey(e => e.id);
            });

            modelBuilder.Entity<CommandData>().ToTable("CommandData", "commanddata");
            modelBuilder.Entity<CommandData>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.Property(e => e.level).HasDefaultValue(0);
                entity.Property(e => e.enabled);
                entity.Property(e => e.display_in_list);
                entity.HasIndex(e => e.name).IsUnique();
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
                entity.HasKey(e => e.id);
                entity.Property(e => e.MacroName).HasDefaultValue(string.Empty);
                entity.Property(e => e.MacroValue).HasDefaultValue(string.Empty);
                entity.HasIndex(e => e.MacroName).IsUnique();
            });

            modelBuilder.Entity<InputSynonym>().ToTable("InputSynonyms", "inputsynonyms");
            modelBuilder.Entity<InputSynonym>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.Property(e => e.SynonymName).HasDefaultValue(string.Empty);
                entity.Property(e => e.SynonymValue).HasDefaultValue(string.Empty);
                entity.Property(e => e.console_id).HasDefaultValue(1);
                entity.HasIndex(e => new { e.SynonymName, e.console_id }).IsUnique();
            });

            modelBuilder.Entity<GameConsole>().ToTable("Consoles", "consoles");
            modelBuilder.Entity<GameConsole>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.Property(e => e.Name).HasDefaultValue("GameConsole");
                entity.Ignore(e => e.InputRegex);
                entity.Ignore(e => e.ConsoleInputs);
                entity.HasMany(e => e.InputList).WithOne(c => c.Console).HasForeignKey(c => c.console_id).IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.InvalidCombos).WithOne().OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<InputData>().ToTable("Inputs", "inputs");
            modelBuilder.Entity<InputData>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.Property(e => e.Name).HasDefaultValue(string.Empty);
                entity.Property(e => e.ButtonValue).HasDefaultValue(0);
                entity.Property(e => e.AxisValue).HasDefaultValue(0);
                entity.Property(e => e.InputType).HasDefaultValue(InputTypes.None).HasConversion(new EnumToNumberConverter<InputTypes, int>());
                entity.Property(e => e.MinAxisVal).HasDefaultValue(0);
                entity.Property(e => e.MaxAxisVal).HasDefaultValue(1);
                entity.Property(e => e.MaxAxisPercent).HasDefaultValue(100);
                entity.HasIndex(e => new { e.Name, e.console_id }).IsUnique();
            });

            modelBuilder.Entity<InvalidCombo>().ToTable("InvalidInputCombos", "invalidinputcombos");
            modelBuilder.Entity<InvalidCombo>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.HasOne(e => e.Input).WithOne().HasForeignKey<InvalidCombo>(c => c.input_id).IsRequired().OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<User>().ToTable("Users", "users");
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.Property(e => e.Name).HasDefaultValue(string.Empty);
                entity.HasOne(e => e.Stats).WithOne(u => u.user).HasForeignKey<UserStats>(u => u.user_id).IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.UserAbilities).WithOne(c => c.user).HasForeignKey(u => u.user_id).IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.RestrictedInputs).WithOne().HasForeignKey(e => e.user_id).IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => e.Name);
            });

            modelBuilder.Entity<UserStats>().ToTable("UserStats", "userstats");
            modelBuilder.Entity<UserStats>(entity =>
            {
                entity.HasKey(e => e.id);
            });

            modelBuilder.Entity<PermissionAbility>().ToTable("PermissionAbilities", "permissionabilities");
            modelBuilder.Entity<PermissionAbility>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.Property(e => e.AutoGrantOnLevel).HasConversion(new EnumToNumberConverter<PermissionLevels, int>());
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<UserAbility>().ToTable("UserAbilities", "userabilities");
            modelBuilder.Entity<UserAbility>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.HasOne(e => e.PermAbility).WithOne().HasForeignKey<UserAbility>(u => u.permability_id).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }

        private void ContextBuilder(SqliteDbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.MigrationsAssembly(System.Reflection.Assembly.GetExecutingAssembly().FullName);
        }
    }
}