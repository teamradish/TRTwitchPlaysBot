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
        //Use a property here to lazy load and avoid requiring Set<T>()
        public DbSet<Settings> SettingCollection { get; set; } = null;
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
            });

           base.OnModelCreating(modelBuilder);
        }

        private void ContextBuilder(SqliteDbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.MigrationsAssembly(System.Reflection.Assembly.GetExecutingAssembly().FullName);
        }
    }
}