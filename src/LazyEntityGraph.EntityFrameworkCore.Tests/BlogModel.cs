using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LazyEntityGraph.EntityFrameworkCore.Tests
{
    public class Entity
    {
        [Key]
        public int Id { get; set; }
    }

    public class User : Entity
    {
        public string Username { get; set; }

        public virtual ContactDetails ContactDetails { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }

    public class ContactDetails
    {
        [Key]
        public virtual int UserId { get; set; }
        public virtual User User { get; set; }
    }

    public class Post : Entity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DatePublished { get; set; }

        public int PosterId { get; set; }
        public virtual User Poster { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }
   
    public class Story : Post
    {
        
    }

    public class Category : Entity
    {
        public string CategoryName { get; set; }
    }

    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options)
                : base(options)
        { }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Story> Stories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.Poster).IsRequired()
                .HasForeignKey(p => p.PosterId);

            modelBuilder.Entity<ContactDetails>()
                .HasOne(cd => cd.User)
                .WithOne(u => u.ContactDetails)
                .HasForeignKey<ContactDetails>(cd => cd.UserId);
        }
    }

    public class MultiColumnContext : DbContext
    {
        public MultiColumnContext(DbContextOptions<MultiColumnContext> options)
            : base(options)
        { }

        public DbSet<Parent> Parents { get; set; }
        public DbSet<Child> Children { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Parent>()
                .HasKey(p => new {p.Id, p.Version});

            modelBuilder.Entity<Parent>()
                .HasMany(u => u.Children)
                .WithOne(p => p.Parent)
                .IsRequired()
                .HasForeignKey(c => new {c.ParentId, c.ParentVersion});
        }

        public class Parent
        {
            public long Id { get; set; }
            public int Version { get; set; }
            public virtual ICollection<Child> Children { get; set; }
        }

        public class Child
        {
            public long ChildId { get; set; }
            public long ParentId { get; set; }
            public int ParentVersion { get; set; }
            public virtual Parent Parent { get; set; }
        }
    }
}
