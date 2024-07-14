using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Services.Contracts
{
    public interface ICommentService
    {
        List<Comment> GetComments();
        List<Comment> GetReplies(int parentCommentId, int skip, int take);
        Comment GetComment(int id);
        Comment CreateComment(Comment comment, User user);
        Comment CreateReply(Comment reply, int parentCommentId, User user);
        bool DeleteComment(int id, User user);
        Comment UpdateComment(int id, Comment comment, User user);
        List<Comment> GetCommentsByPostId(int postId);
    }
}