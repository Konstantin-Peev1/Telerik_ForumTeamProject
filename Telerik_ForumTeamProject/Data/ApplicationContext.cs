using Microsoft.EntityFrameworkCore;
using Telerik_ForumTeamProject.Models.Entities;
using System.Collections.Generic;

namespace Telerik_ForumTeamProject.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get;set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User-Post relationship
            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserID)
                .OnDelete(DeleteBehavior.Restrict); // Use Restrict to avoid cycles

            // Post-Comment relationship
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostID)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete comments when post is deleted


            // User-Comment relationship
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete comments when user is deleted
            
            // Comment-Reply relationship
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(r => r.Replies)
                .HasForeignKey(c => c.ParentCommentID)
                .OnDelete(DeleteBehavior.Restrict); 

            string plainPassword = "123456778";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);
            List<User> users = new List<User>()
            {
                new User
                {
                    ID = 1,
                    UserName = "Kosio_Peev",
                    Password = hashedPassword,
                    FirstName = "Konstantin",
                    LastName = "Peev",
                    Email = "konstantin.i.peev@gmail.com",
                    IsBlocked = false,
                    IsAdmin = true,
                    Role = "Admin"
                }
            };

            List<Post> posts = new List<Post>()
            {
                new Post
                {
                    Id = 1,
                    Title = "This is my first post!!",
                    Content = "Wow this is the first post I have written",
                    UserID = users[0].ID,
                    Created = DateTime.Now
                    
                }
            };
            List<Comment> comments = new List<Comment>()
            {
                new Comment
                {
                    Id = 1,
                    Content = "I am just commenting because why not",
                    UserID = users[0].ID, 
                    PostID = posts[0].Id,
                    Created = DateTime.Now
                },
                new Comment
                {
                    Id = 2,
                    Content = "I am a reply and just testing the seeding",
                    UserID = users[0].ID,
                    PostID = posts[0].Id,
                    Created = DateTime.Now,
                    ParentCommentID = 1,
                }
            };



            List<Tag> tags = new List<Tag>()
            {
                new Tag
                {
                    ID = 1,                    
                    Description = "TestTag",
                   
                }
            };

           
            modelBuilder.Entity<User>().HasData(users);
            modelBuilder.Entity<Post>().HasData(posts);
            modelBuilder.Entity<Comment>().HasData(comments);
            modelBuilder.Entity<Tag>().HasData(tags);

            modelBuilder.Entity<User>().HasDiscriminator<string>("Discriminator").HasValue<User>("User");
         
        }
    }
}
