﻿using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HPages.Database.Tables;
using Task = System.Threading.Tasks.Task;

namespace HPages.Database
{
    public class HentaiDbContext : DbContext
    {
        public DbSet<HImage> Images { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<Similarity> SimilarityScores { get; set; }
        public DbSet<WorkerTask> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = @"D:/HentaiDb.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            builder.UseSqlite(connection);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TagsImages>();
            builder.Entity<TagsImages>().HasKey(c => new { c.ImageId, c.TagsId });
            builder.Entity<TagsImages>().HasOne(c => c.Image).WithMany(c => c.Tags).HasForeignKey(c=>c.ImageId);
            builder.Entity<TagsImages>().HasOne(c => c.Tag).WithMany(c => c.Images).HasForeignKey(c => c.TagsId);

            builder.Entity<HImage>().HasKey(c => c.ImageId);
            builder.Entity<HImage>().HasIndex(c => new { c.UploadDate });
            builder.Entity<HImage>().HasIndex(c => new { c.PixelData });
            
            builder.Entity<Similarity>().HasIndex(c => new { c.ChildImageId });
            
            base.OnModelCreating(builder);
        }
    }
}
