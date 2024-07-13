using Microsoft.EntityFrameworkCore;
using Telerik_ForumTeamProject.Data;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;

namespace Telerik_ForumTeamProject.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationContext applicationContext;

        public TagRepository(ApplicationContext applicationContext)
        {
            this.applicationContext = applicationContext;
        }

        public Tag Create(Tag tag)
        {
            this.applicationContext.Tags.Add(tag);
            this.applicationContext.SaveChanges();
            return tag; 
        }

        public bool TagExists(string description)
        {
            return this.applicationContext.Tags.Any(tag => tag.Description == description);
        }

        public Tag TagExistsFullDesc(string description)
        {
            return this.applicationContext.Tags.Include(tag => tag.Posts).FirstOrDefault(tag => tag.Description == description) ?? throw new EntityNotFoundException("No such tag");
        }
        public Tag UpdateTags(Post post,string desc)
        {
            post.Tags.Add(TagExistsFullDesc(desc));
            TagExistsFullDesc(desc).Posts.Add(post);
            this.applicationContext.SaveChanges();
            return TagExistsFullDesc(desc);
        }
        public bool RemoveTags(Post post, Tag tag)
        {
            post.Tags.Remove(tag);
            
            return this.applicationContext.SaveChanges() > 0;
        }

        public ICollection<Tag> GetTagByDesc(string description)
        {
            return this.applicationContext.Tags.Where(tag => tag.Description.Contains(description)).Include(tag => tag.Posts).ToList();
        }
    }
}
