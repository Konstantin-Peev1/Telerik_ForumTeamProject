using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Repositories.Contracts
{
    public interface IReplyRepository
    {
        Reply CreateReply(Reply reply);
        List<Reply> GetAllReplies();
        List<Reply> GetRepliesByUser(int userId);
        Reply GetReplyById(int id);
        Reply UpdateReply(int id, Reply reply);
    }
}