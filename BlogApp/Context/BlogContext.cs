using BlogApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApp.Context
{
    public class BlogContext: IdentityDbContext<AppUser, AppRole, string>
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<BlogCategory>().HasKey(i => new { i.CategoryId, i.BlogId });
            builder.Entity<BlogCategory>().HasOne(b => b.Blog).WithMany(bc => bc.BlogCategories).HasForeignKey(b=>b.BlogId);
            builder.Entity<BlogCategory>().HasOne(b => b.Category).WithMany(bc => bc.BlogCategories).HasForeignKey(b=>b.CategoryId);
            base.OnModelCreating(builder);
        }
    }
    public class Blog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id{ get; set; }
        public string Title{ get; set; }
        public string Content{ get; set; }
        public string Date{ get; set; }
        public string Image{ get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public List<BlogCategory> BlogCategories { get; set; }
    }
    public class Category
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public List<BlogCategory> BlogCategories { get; set; }

    }
    public class BlogCategory
    {
        public string BlogId { get; set; }
        public Blog Blog { get; set; }
        public string CategoryId { get; set; }
        public Category Category { get; set; }
    }
    public class Comment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string CommentText { get; set; }
        public string Photo { get; set; }
        public string BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
