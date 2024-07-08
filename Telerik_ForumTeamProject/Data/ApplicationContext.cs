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
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get;set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Reply> Replies { get; set; }

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

            // Comment-Reply relationship
            modelBuilder.Entity<Reply>()
                .HasOne(r => r.Post)
                .WithMany(p => p.Replies)
                .HasForeignKey(r => r.PostID)
                .OnDelete(DeleteBehavior.Restrict); // Use Restrict to avoid multiple cascade paths

         

            // User-Comment relationship
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete comments when user is deleted

            // User-Reply relationship
            modelBuilder.Entity<Reply>()
                .HasOne(r => r.User)
                .WithMany(u => u.Replies)
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Cascade);
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
                    IsAdmin = true
                }
            };
/*            List<Admin> admins = new List<Admin>()
            {
                new Admin
                {
                    ID = 1,
                    FirstName = "Admin",
                    LastName = "Adminov",
                    Email = "adminAdminov@gmail.com"
                }
            };*/
            List<Post> posts = new List<Post>()
            {
                new Post
                {
                    Id = 1,
                    Title = "This is my first post!!",
                    Content = "Wow this is the first post I have written",
                    UserID = users[0].ID,
                    Created = DateTime.Now,
                    
                }
            };
            List<Comment> comments = new List<Comment>()
            {
                new Comment
                {
                    Id = 1,
                    PostID = posts[0].Id,
                    
                    Content = "I am just commenting because why not",
                    UserID = users[0].ID,
                   
                }
            };
            List<Reply> replies = new List<Reply>()
            {
                new Reply
                {
                    Id = 1,
                    PostID = posts[0].Id,
                    Content = "I am just commenting because why not",
                    UserID = users[0].ID,
                   
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

            //modelBuilder.Entity<Admin>().HasData(admins);
            modelBuilder.Entity<User>().HasData(users);
            modelBuilder.Entity<Post>().HasData(posts);
            modelBuilder.Entity<Comment>().HasData(comments);
            modelBuilder.Entity<Reply>().HasData(replies);
            modelBuilder.Entity<Tag>().HasData(tags);

            modelBuilder.Entity<User>().HasDiscriminator<string>("Discriminator").HasValue<User>("User");
         
        }
    }
}
