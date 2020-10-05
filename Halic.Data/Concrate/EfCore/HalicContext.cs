using Halic.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Halic.Data.Concrate.EfCore
{
    public class HalicContext:DbContext
    {
        public DbSet<Article> articles { get; set; }
        public DbSet <Author> authors { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Video> Video { get; set; }
        public DbSet<News> News { get; set; } 
        public DbSet<NCategory> NCategories { get; set; }
        public DbSet<Activities> Activities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=HALBIM0003043\\SQLSERVER;database=DBHalicHubNews;integrated security=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticleCategory>()
            .HasKey(c => new { c.CategoryId, c.ArticleId });

            modelBuilder.Entity<ArticleAuthor>()
           .HasKey(c => new { c.ArticleId, c.AuthorId });

            modelBuilder.Entity<NewsHCategory>()
            .HasKey(c => new { c.NewsId, c.NCategoryId });  

            modelBuilder.Entity<NewsAuthor>()
           .HasKey(c => new { c.NewsId, c.AuthorId });
        }
    }
}
