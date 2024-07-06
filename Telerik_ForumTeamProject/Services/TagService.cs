using System.Transactions;
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

        public bool RemoveTags(Post post, Tag tag)
        {
            if(post.Tags.Any(tagPost => tagPost.Description == tag.Description))
            {
                post.Tags.Remove(tag);
                return true;
            }
            return false;
        }




    }
}
