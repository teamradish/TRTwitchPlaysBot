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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

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
                entity.Property(e => e.enabled).HasDefaultValue(1);
                entity.Property(e => e.display_in_list).HasDefaultValue(1);
                entity.HasIndex(e => e.name).IsUnique();
            });

           base.OnModelCreating(modelBuilder);
        }

        private void ContextBuilder(SqliteDbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.MigrationsAssembly(System.Reflection.Assembly.GetExecutingAssembly().FullName);
        }
    }
}