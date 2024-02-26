using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Orderly.Database.Entities;

namespace Orderly.Database
{
    public class DatabaseContext : DbContext
    {
        public string DbName = "CoreDB.ordb";

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Credential> Credentials { get; set; }

        public DatabaseContext()
        {
            Database.EnsureCreated();
            Database.Migrate();
            
        }

        public DatabaseContext(string dbName)
        {
            DbName = dbName;
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(new SqliteConnectionStringBuilder() {
                DataSource = DbName,
                Pooling = false,
                Mode = SqliteOpenMode.ReadWriteCreate
            }.ConnectionString);
        }

        public void EnsureClosed()
        {
            Database.CloseConnection();
            GC.Collect();
        }
    }
}
