﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab4.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab4.Data
{
    public class SchoolCommunityContext : DbContext
    {
        public SchoolCommunityContext(DbContextOptions<SchoolCommunityContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable("student");
            modelBuilder.Entity<Community>().ToTable("community");
            modelBuilder.Entity<Advertisements>().ToTable("advertisement");
            modelBuilder.Entity<CommunityMembership>().HasKey(c => new { c.StudentID, c.CommunityID });
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Community> Communities { get; set; }

        public DbSet<CommunityMembership> CommunityMemberships { get; set; }

        public DbSet<Advertisements> Advertisements { get; set; }
    }
}
