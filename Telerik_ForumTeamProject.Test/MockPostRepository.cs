using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Exceptions;

namespace Telerik_ForumTeamProject.Test
{
    public class MockPostRepository
    {
        private List<Post> samplePosts;
        private List<User> sampleUsers;

        public Mock<IPostRepository> GetMockRepository()
        {
            var mockRepository = new Mock<IPostRepository>();

            sampleUsers = new List<User>
            {
                new User
                {
                    ID = 1,
                    UserName = "Kosio",
                    Password = "password",
                    FirstName = "string",
                    LastName = "string",
                    Email = "user1@example.com",
                    IsAdmin = true,
                    IsBlocked = false,
                    Posts = new List<Post>(),
                    Comments = new List<Comment>(),
                },
                new User
                {
                    ID = 2,
                    UserName = "Kosio1",
                    Password = "password1",
                    FirstName = "string1",
                    LastName = "string1",
                    Email = "user2@example.com",
                    IsAdmin = false,
                    IsBlocked = false,
                    Posts = new List<Post>(),
                    Comments = new List<Comment>(),
                },
                new User
                {
                    ID = 3,
                    UserName = "Kosio2",
                    Password = "password2",
                    FirstName = "string2",
                    LastName = "string2",
                    Email = "user3@example.com",
                    IsAdmin = false,
                    IsBlocked = true,
                    Posts = new List<Post>(),
                    Comments = new List<Comment>(),
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
                    Likes = new List<Like> { new Like { Id = 1 }, new Like { Id = 2 } },
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
                    Likes = new List<Like> { new Like { Id = 3 } },
                }
            };

            // Setup for CreatePost
            mockRepository.Setup(x => x.CreatePost(It.IsAny<Post>()))
                .Callback((Post post) =>
                {
                    post.Id = samplePosts.Max(p => p.Id) + 1;
                    samplePosts.Add(post);
                })
                .Returns((Post post) => post);

            // Setup for DeletePost
            mockRepository.Setup(x => x.DeletePost(It.IsAny<Post>()))
                .Callback((Post post) =>
                {
                    samplePosts.Remove(post);
                })
                .Returns((Post post) => samplePosts.Contains(post) == false);

            // Setup for GetPost
            mockRepository.Setup(x => x.GetPost(It.IsAny<int>()))
                .Returns((int id) => samplePosts.FirstOrDefault(post => post.Id == id));

            // Setup for FilterBy
            mockRepository.Setup(x => x.FilterBy(It.IsAny<PostQueryParamteres>()))
                .Returns((PostQueryParamteres filterParameters) =>
                    samplePosts.Where(post =>
                        (string.IsNullOrEmpty(filterParameters.Title) || post.Title.Contains(filterParameters.Title)) &&
                        (string.IsNullOrEmpty(filterParameters.UserName) || sampleUsers.Any(user => user.ID == post.UserID && user.UserName.Contains(filterParameters.UserName))) &&
                        (filterParameters.MinLikes <= post.Likes.Count && post.Likes.Count <= filterParameters.MaxLikes))
                    .ToList());

            // Setup for GetTop10Commented
            mockRepository.Setup(x => x.GetTop10Commented())
                .Returns(samplePosts.OrderByDescending(post => post.Comments.Count).Take(10).ToList());

            // Setup for GetTop10Recent
            mockRepository.Setup(x => x.GetTop10Recent())
                .Returns(samplePosts.OrderByDescending(post => post.Created).Take(10).ToList());

            // Setup for UpdatePost
            mockRepository.Setup(x => x.UpdatePost(It.IsAny<Post>(), It.IsAny<Post>()))
                .Callback((Post originalPost, Post updatedPost) =>
                {
                    var postToUpdate = samplePosts.FirstOrDefault(p => p.Id == originalPost.Id);
                    if (postToUpdate != null)
                    {
                        postToUpdate.Title = updatedPost.Title;
                        postToUpdate.Content = updatedPost.Content;
                        postToUpdate.LastModified = DateTime.Now;
                        // Update other fields as needed
                    }
                })
                .Returns((Post originalPost, Post updatedPost) => updatedPost);

            // Setup for GetPagedPosts
            mockRepository.Setup(x => x.FilterBy(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<PostQueryParamteres>()))
                .Returns((int page, int pageSize, PostQueryParamteres filterParameters) =>
                {
                    var filteredPosts = samplePosts.Where(post =>
                        (post.Title.Contains(filterParameters.Title)) ||
                        (string.IsNullOrEmpty(filterParameters.UserName) || sampleUsers.Any(user => user.ID == post.UserID || user.UserName.Contains(filterParameters.UserName))) &&
                        (filterParameters.MinLikes <= post.Likes.Count || post.Likes.Count <= filterParameters.MaxLikes))
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                    return filteredPosts;
                });

            // Setup for GetPostsCount
            mockRepository.Setup(x => x.GetPostsCount())
                .Returns(samplePosts.Count);

            // Throws if no such post
            mockRepository
              .Setup(repo => repo.GetPost(It.Is<int>(id => samplePosts.All(p => p.Id != id))))
              .Throws(new EntityNotFoundException("Post doesn't exist."));

            return mockRepository;
        }
    }
}
