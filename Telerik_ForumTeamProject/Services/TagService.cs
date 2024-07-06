using System.Transactions;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        public Tag Create(Tag tag)
        {
            return tagRepository.Create(tag);
        }

        public bool RemoveTags(User user, Post post, Tag tag)
        {
            if(user.ID != post.UserID && !user.IsAdmin)
            {
                throw new AuthorisationExcpetion("You can't edit tags of other users");
            }

            if(post.Tags.Any(tagPost => tagPost.Description == tag.Description))
            {
                post.Tags.Remove(tag);
                tag.Posts.Remove(post);
                return true;
            }
            return false;
        }

        public Tag UpdateTags(User user, Post post, Tag tag)
        {
            if (user.ID != post.UserID && !user.IsAdmin)
            {
                throw new AuthorisationExcpetion("You can't edit tags of other users");
            }
            if(tagRepository.TagExists(tag.Description) 
                && !post.Tags.Any(tagToChecl => tagToChecl.Description == tag.Description))
            {
                post.Tags.Add(tag);
                tag.Posts.Add(post);
                return tag;
            }
            else if (!tagRepository.TagExists(tag.Description))
            {
                tagRepository.Create(tag);
                post.Tags.Add(tag);
                tag.Posts.Add(post);
                return tag;
            }
            else
            {
                throw new DuplicateEntityException("Such a tag on this post already exists!");
            }
        }
    }
}
