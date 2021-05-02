using HentaiPages.Database.Tables;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HentaiPages.Database
{
    public class HentaiDbContext : DbContext
    {
        public DbSet<Image> Images { get; set; }
        public DbSet<Tags> Tags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "HentaiDb.db" };
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

            builder.Entity<Image>().HasIndex(c => new { c.ImageId, c.UploadDate });

            base.OnModelCreating(builder);
        }
    }
}
