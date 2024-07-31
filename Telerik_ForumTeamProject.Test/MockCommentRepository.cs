using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Exceptions;

namespace Telerik_ForumTeamProject.Test
{
    public class MockCommentRepository
    {
        private List<Comment> sampleComments;
        private List<User> sampleUsers;
        private List<Post> samplePosts;

        public Mock<ICommentRepository> GetMockRepository()
        {
            var mockRepository = new Mock<ICommentRepository>();
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
                    IsAdmin = true,
                    IsBlocked = false
                }
            };

            sampleComments = new List<Comment>
            {
                new Comment
                {
                    Id = 1,
                    Content = "First comment",
                    UserID = 1,
                    PostID = 1,
                    Created = DateTime.UtcNow.AddDays(-1),
                    Replies = new List<Comment>()
                },
                new Comment
                {
                    Id = 2,
                    Content = "Second comment",
                    UserID = 2,
                    PostID = samplePosts[0].Id,
                    Created = DateTime.UtcNow.AddDays(-2),
                    Replies = new List<Comment>
                    {
                       
                    }
                }
            };

            sampleComments[1].Replies.Add(new Comment
            {
                Id = 3,
                Content = "Reply to second comment",
                UserID = 1,
                User = sampleUsers[0],
                PostID = 1,
                ParentCommentID = sampleComments[1].Id,
                ParentComment = sampleComments[1]
                
            });

            // Setup for GetAllPostComments
            mockRepository.Setup(x => x.GetAllPostComments(It.IsAny<int>()))
                .Returns((int postId) => sampleComments.Where(c => c.PostID == postId).ToList());

            // Setup for GetPagedReplies
            mockRepository.Setup(x => x.GetPagedReplies(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns((int parentCommentId, int page, int pageSize) =>
                {
                    int skip = (page - 1) * pageSize;
                    var replies = sampleComments
                        .Where(c => c.ParentCommentID == parentCommentId)
                        .Skip(skip)
                        .Take(pageSize)
                        .Select(c =>
                        {
                            c.User = sampleUsers.FirstOrDefault(u => u.ID == c.UserID);
                            return c;
                        })
                        .ToList();
                    return replies;
                });

            // Setup for GetCommentById
            mockRepository.Setup(x => x.GetCommentById(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    var comment = sampleComments.FirstOrDefault(c => c.Id == id);
                    if (comment == null)
                    {
                        throw new EntityNotFoundException($"Comment with ID {id} not found.");
                    }
                    return comment;
                });

            // Setup for GetRepliesCount
            mockRepository.Setup(x => x.GetRepliesCount(It.IsAny<int>()))
                .Returns((int parentCommentId) => sampleComments.Count(c => c.ParentCommentID == parentCommentId));

            // Setup for CreateComment
            mockRepository.Setup(x => x.CreateComment(It.IsAny<Comment>()))
                .Callback((Comment comment) =>
                {
                    comment.Id = sampleComments.Max(c => c.Id) + 1;
                    sampleComments.Add(comment);
                })
                .Returns((Comment comment) => comment);

            // Setup for CreateReply
            mockRepository.Setup(x => x.CreateReply(It.IsAny<Comment>(), It.IsAny<int>()))
                .Callback((Comment reply, int parentCommentId) =>
                {
                    reply.Id = sampleComments.Max(c => c.Id) + 1;
                    reply.ParentCommentID = parentCommentId;
                    sampleComments.Add(reply);
                })
                .Returns((Comment reply, int parentCommentId) => reply);

            // Setup for UpdateComment
            mockRepository.Setup(x => x.UpdateComment(It.IsAny<int>(), It.IsAny<Comment>()))
                .Callback((int id, Comment comment) =>
                {
                    var commentToUpdate = sampleComments.FirstOrDefault(c => c.Id == id);
                    if (commentToUpdate != null)
                    {
                        commentToUpdate.Content = comment.Content;
                        
                    }
                })
                .Returns((int id, Comment comment) => comment);

            // Setup for DeleteComment
            mockRepository.Setup(x => x.DeleteComment(It.IsAny<int>(), It.IsAny<Comment>()))
                .Callback((int id, Comment comment) =>
                {
                    sampleComments.Remove(comment);
                })
                .Returns((int id, Comment comment) => true);

            return mockRepository;
        }
    }
}
