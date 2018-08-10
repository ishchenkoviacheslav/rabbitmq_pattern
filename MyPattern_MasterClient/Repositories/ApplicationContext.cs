using Microsoft.EntityFrameworkCore;
using MyPattern_MasterClient.Entities;
using System;
using System.Collections.Generic;
using System.Text;
namespace MyPattern_MasterClient.Repositories
{
    class ApplicationContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL($"server={MasterClient.ConfigurationData.DbServerDomenOrIP};database={MasterClient.ConfigurationData.DbName};user={MasterClient.ConfigurationData.DbUserName};password={MasterClient.ConfigurationData.DbUserPassword};SslMode=none");
            //optionsBuilder.UseMySQL("server=localhost;database=patterndb;user=root;password=;SslMode=none");//
            //optionsBuilder.UseSqlServer(@"Server=(localdb)\\ubuntu;Database=;Trusted_Connection=True;")
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.QueueName).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Salt).IsUnique();
            modelBuilder.Entity<Session>().HasKey(s => s.SessionId); // primary key
        }

    }
}
