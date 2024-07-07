﻿using System.Transactions;
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

        public Tag Create(string desc)
        {
            Tag tag = new Tag()
            {
                Description = desc,
            };
            Tag createdTag = tagRepository.Create(tag);
            return createdTag;
        }

        public List<Tag> GetTagsByDesc(string desc)
        {
            return this.tagRepository.GetTagByDesc(desc);
        }

        public bool RemoveTags(User user, Post post, string desc)
        {
            if(user.ID != post.UserID && !user.IsAdmin)
            {
                throw new AuthorisationExcpetion("You can't edit tags of other users");
            }

            if(post.Tags.Any(tagPost => desc == tagPost.Description))
            {
                Tag tag = tagRepository.GetTagByDesc(desc).FirstOrDefault(t => t.Description == desc);
                post.Tags.Remove(tag);
                tag.Posts.Remove(post);
                return true;
            }
            return false;
        }

        public Tag UpdateTags(User user, Post post, string desc)
        {
            if (user.ID != post.UserID && !user.IsAdmin)
            {
                throw new AuthorisationExcpetion("You can't edit tags of other users");
            }
            if(tagRepository.TagExists(desc) 
                && !post.Tags.Any(tagToChecl => tagToChecl.Description == desc))
            {
                
                return this.tagRepository.UpdateTags(post, desc);
            }
            else if (!tagRepository.TagExists(desc))
            {
                Tag tag = Create(desc);
                this.tagRepository.UpdateTags(post, desc);
                return this.tagRepository.UpdateTags(post, desc);
                
            }
            else
            {
                throw new DuplicateEntityException("Such a tag on this post already exists!");
            }
        }
    }
}
