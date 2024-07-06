using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Services.Contracts
{
    public interface ITagService
    {
        Tag Create(Tag tag);
        bool RemoveTags(User user, Post post, Tag tag);
        Tag UpdateTags(User user, Post post, Tag tag);

    }
}
