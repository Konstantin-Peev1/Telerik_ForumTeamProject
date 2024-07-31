using Moq;
using System.Collections.Generic;
using System.Linq;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;

namespace Telerik_ForumTeamProject.Test
{
    public class MockLikeRepository
    {
        private List<Like> sampleLikes;
        private List<Post> samplePosts;
        private List<User> sampleUsers;

        public Mock<ILikeRepository> GetMockRepository()
        {
            var mockRepository = new Mock<ILikeRepository>();

            sampleUsers = new List<User>
            {
                new User
                {
                    ID = 1,
                    UserName = "User1",
                    Password = "password1",
                    FirstName = "FirstName1",
                    LastName = "LastName1",
                    Email = "user1@example.com",
                    IsAdmin = false,
                    IsBlocked = false
                },
                new User
                {
                    ID = 2,
                    UserName = "User2",
                    Password = "password2",
                    FirstName = "FirstName2",
                    LastName = "LastName2",
                    Email = "user2@example.com",
                    IsAdmin = false,
                    IsBlocked = false
                }
            };

            samplePosts = new List<Post>
            {
                new Post
                {
                    Id = 1,
                    Title = "First Post",
                    Content = "Content of the first post",
                    UserID = 1,
                    Created = DateTime.Now.AddDays(-10),
                    LastModified = DateTime.Now.AddDays(-5),
                    Comments = new List<Comment>(),
                    Tags = new List<Tag>(),
                    Likes = new List<Like>()
                },
                new Post
                {
                    Id = 2,
                    Title = "Second Post",
                    Content = "Content of the second post",
                    UserID = 2,
                    Created = DateTime.Now.AddDays(-8),
                    LastModified = DateTime.Now.AddDays(-3),
                    Comments = new List<Comment>(),
                    Tags = new List<Tag>(),
                    Likes = new List<Like>()
                }
            };

            sampleLikes = new List<Like>
            {
                new Like
                {
                    Id = 1,
                    UserId = 1,
                    PostID = 1
                },
                new Like
                {
                    Id = 2,
                    UserId = 2,
                    PostID = 2
                }
            };

            // Setup for Create
            mockRepository.Setup(x => x.Create(It.IsAny<User>(), It.IsAny<Post>(), It.IsAny<Like>()))
                .Callback((User user, Post post, Like like) =>
                {
                    like.Id = sampleLikes.Max(l => l.Id) + 1;
                    sampleLikes.Add(like);
                    post.Likes.Add(like);
                })
                .Returns((User user, Post post, Like like) => like);

            // Setup for Remove
            mockRepository.Setup(x => x.Remove(It.IsAny<User>(), It.IsAny<Post>(), It.IsAny<Like>()))
                .Callback((User user, Post post, Like like) =>
                {
                    sampleLikes.Remove(like);
                    post.Likes.Remove(like);
                })
                .Returns((User user, Post post, Like like) => like);

            return mockRepository;
        }
    }
}
