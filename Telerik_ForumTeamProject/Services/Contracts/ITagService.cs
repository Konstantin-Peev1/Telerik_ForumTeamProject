using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Services.Contracts
{
    public interface ITagService
    {
        Tag Create(Tag tag);
        bool RemoveTags(Post post, Tag tag);

    }
}
