using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Repositories.Contracts
{
    public interface ICommentRepository
    {
        Comment CreateComment(Comment comment);
        ICollection<Comment> GetAllPostComments(int postId);
        ICollection<Comment> GetPagedReplies(int parentCommentId, int page, int pageSize);
        int GetRepliesCount(int parentCommentId);
        Comment GetCommentById(int id);
        Comment UpdateComment(int id, Comment comment);
        public bool DeleteComment(int id, Comment comment);
        Comment CreateReply(Comment reply, int parentCommentId);
    }
}