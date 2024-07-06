using Telerik_ForumTeamProject.Data;
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
        public Post UpdateTags(Post post, Tag tag)
        {
            post.Tags.Add(tag);
            this.applicationContext.SaveChanges();
            return post;
        }
        public bool RemoveTags(Post post, Tag tag)
        {
            post.Tags.Remove(tag);
            
            return this.applicationContext.SaveChanges() > 0;
        }
    }
}
