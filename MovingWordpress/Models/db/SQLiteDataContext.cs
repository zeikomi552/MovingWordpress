﻿using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models.db
{
    public class SQLiteDataContext : DbContext
    {
        public DbSet<TwitterUserBase> DbSet_TwitterUser { get; internal set; }
        public DbSet<MyFollowUserBase> DbSet_MyFollowUser { get; internal set; }
        public DbSet<MyFollowerUserBase> DbSet_MyFollowerUser { get; internal set; }

        public SQLiteDataContext()
        {
            ConfigM conf = new ConfigM();
            var tconf_dir = conf.ConfigDirPath;
            db_file_path = Path.Combine(tconf_dir, "TwitterLOG.sql");

        }

        // 最初にココを変更する
        string db_file_path = string.Empty;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = new SqliteConnectionStringBuilder { DataSource = db_file_path }.ToString();
            optionsBuilder.UseSqlite(new SqliteConnection(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TwitterUserBase>().HasKey(c => new { c.Id });
            modelBuilder.Entity<MyFollowUserBase>().HasKey(c => new { c.Id });
            modelBuilder.Entity<MyFollowerUserBase>().HasKey(c => new { c.Id });

        }
    }
}
