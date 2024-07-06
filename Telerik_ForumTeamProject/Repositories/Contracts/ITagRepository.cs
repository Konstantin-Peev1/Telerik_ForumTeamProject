using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Repositories.Contracts
{
    public interface ITagRepository
    {
        Tag Create(Tag tag);
        bool TagExists(string description);
        bool RemoveTags(Post post, Tag tag);
        Post UpdateTags(Post post, Tag tag);
    }
}
