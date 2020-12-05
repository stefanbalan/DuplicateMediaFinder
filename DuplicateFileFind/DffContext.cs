using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
//using Microsoft.Extensions.Logging.Debug;

namespace DuplicateFileFind
{
    public class DffContext : DbContext
    {
        public static readonly LoggerFactory ConsoleLoggerFactory
            = new LoggerFactory(new[] {new ConsoleLoggerProvider((s, level) => true, false)});

        //EXPLAIN QUERY PLAN SELECT* FROM 'Files' where length = 387850

        public DbSet<Directory> Directories { get; set; }
        public DbSet<File> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Directory>()
                .HasIndex(dir => dir.Path);

            modelBuilder.Entity<File>()
                .HasIndex(f => f.DirectoryId);

            modelBuilder.Entity<File>()
                .HasIndex(f => f.Name);

            modelBuilder.Entity<File>()
                .HasIndex(f => f.Length);

            modelBuilder.Entity<File>()
                .HasIndex(f => f.Hash);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new SqliteConnectionStringBuilder { DataSource = "c:\\dff\\files.db" };

            optionsBuilder.UseSqlite(builder.ConnectionString);
            optionsBuilder.UseLoggerFactory(ConsoleLoggerFactory);

            base.OnConfiguring(optionsBuilder);
        }
    }
    public class Directory
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public ICollection<File> Files { get; set; }
    }

    public class File
    {
        public int Id { get; set; }
        public int DirectoryId { get; set; }
        public Directory Directory { get; set; }
        public string Name { get; set; }
        public long Length { get; set; }
        public DateTime CreationTime { get; set; }
        public string Hash { get; set; }
    }
}
