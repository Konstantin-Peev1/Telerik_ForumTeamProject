using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Repositories.Contracts
{
    public interface ILikeRepository
    {
        Like Create(User user, Post post, Like like);
        Like Remove(User user, Post post, Like like);
    }
}
