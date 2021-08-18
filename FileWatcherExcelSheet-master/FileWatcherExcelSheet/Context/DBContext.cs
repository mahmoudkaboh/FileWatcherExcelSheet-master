
using FileWatcherExcelSheet.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileWatcherExcelSheet.Conext
{
    public class DBContext : DbContext
    {

        public DBContext():base()
        {
        }

        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {


        }

        public virtual DbSet<Customer> customer { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.;Database=FileWahtcherDB;Integrated Security=true;");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // for chain with base for Idntity 
            base.OnModelCreating(modelBuilder);

        }


    }// class
} // nanespace
