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
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;database=patterndb;user=root;password=;SslMode=none");//
            //optionsBuilder.UseSqlServer(@"Server=(localdb)\\ubuntu;Database=;Trusted_Connection=True;")
        }
    }
}
