using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Exceptions;

namespace Telerik_ForumTeamProject.Test
{
    public class MockTagRepository
    {
        private List<Tag> sampleTags;
        private List<Post> samplePosts;

        public Mock<ITagRepository> GetMockRepository()
        {
            var mockRepository = new Mock<ITagRepository>();

            sampleTags = new List<Tag>
            {
                new Tag
                {
                    ID = 1,
                    Description = "Tag1",
                    Posts = new List<Post>()
                },
                new Tag
                {
                    ID = 2,
                    Description = "Tag2",
                    Posts = new List<Post>()
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
                    Tags = new List<Tag>()
                },
                new Post
                {
                    Id = 2,
                    Title = "Second Post",
                    Content = "Content of the second post",
                    UserID = 2,
                    Tags = new List<Tag>()
                }
            };

            // Setup for Create
            mockRepository.Setup(x => x.Create(It.IsAny<Tag>()))
                .Callback((Tag tag) =>
                {
                    tag.ID = sampleTags.Max(t => t.ID) + 1;
                    sampleTags.Add(tag);
                })
                .Returns((Tag tag) => tag);

            // Setup for GetTagByDesc
            mockRepository.Setup(x => x.GetTagByDesc(It.IsAny<string>()))
                .Returns((string desc) => sampleTags.Where(t => t.Description.Contains(desc)).ToList());

            // Setup for RemoveTags
            mockRepository.Setup(x => x.RemoveTags(It.IsAny<Post>(), It.IsAny<Tag>()))
                .Returns((Post post, Tag tag) =>
                {
                    if (post.Tags.Remove(tag))
                    {
                        tag.Posts.Remove(post);
                        post.Tags.Remove(tag);
                        return true;
                    }
                    return false;
                });

            // Setup for UpdateTags
            mockRepository.Setup(x => x.UpdateTags(It.IsAny<Post>(), It.IsAny<string>()))
                .Returns((Post post, string desc) =>
                {
                    var tag = sampleTags.FirstOrDefault(t => t.Description == desc) ?? new Tag { Description = desc };
                    tag.Posts = new List<Post>();
                    if (!post.Tags.Any(t => t.Description == desc))
                    {
                        post.Tags.Add(tag);
                        tag.Posts.Add(post);
                    }
                    return tag;
                });

            // Setup for TagExists
            mockRepository.Setup(x => x.TagExists(It.IsAny<string>()))
                .Returns((string desc) => sampleTags.Any(t => t.Description == desc));

            return mockRepository;
        }
    }
}
