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
    }
}
