using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Repositories.Contracts
{
    public interface ICommentRepository
    {
        Comment CreateComment(Comment comment);
        List<Comment> GetAllComments();
        Comment GetCommentById(int id);
        Comment UpdateComment(int id, Comment comment);
    }
}