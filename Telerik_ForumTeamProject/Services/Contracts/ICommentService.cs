using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Services.Contracts
{
    public interface ICommentService
    {
        public List<Comment> GetComments();
        Comment GetComment(int id);
        Comment CreateComment(Comment comment, User user);
        bool DeleteComment(int id, User user);
        Comment UpdateComment(int id, Comment comment, User user);
    }
}