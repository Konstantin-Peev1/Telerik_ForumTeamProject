using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.ResponseDTO;

namespace Telerik_ForumTeamProject.Services.Contracts
{
    public interface ICommentService
    {
        ICollection<Comment> GetComments();
        PagedResult<Comment> GetPagedReplies(int parentCommentId, int page, int pageSize);
        Comment GetComment(int id);
        Comment CreateComment(Comment comment, User user);
        Comment CreateReply(Comment reply, int parentCommentId, User user);
        bool DeleteComment(int id, User user);
        Comment UpdateComment(int id, Comment comment, User user);
    }
}