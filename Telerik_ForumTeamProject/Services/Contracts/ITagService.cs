using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Services.Contracts
{
    public interface ITagService
    {
        bool RemoveTags(User user, Post post, string desc);
        Tag UpdateTags(User user, Post post, string desc);
        Tag Create(string desc);

        ICollection<Tag> GetTagsByDesc(string desc);   

    }
}
