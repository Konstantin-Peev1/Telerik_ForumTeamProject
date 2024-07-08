using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
        }

        public List<Comment> GetComments()
        {
            return this.commentRepository.GetAllComments();
        }

        public Comment GetComment(int id)
        {
            return this.commentRepository.GetCommentById(id);
        }

        public Comment CreateComment(Comment comment, User user)
        {
            comment.UserID = user.ID;
            Comment result = this.commentRepository.CreateComment(comment);
            return result;
        }

        public Comment UpdateComment(int id, Comment comment, User user)
        {
            if (comment.UserID != user.ID && !user.IsAdmin)
            {
                throw new AuthorisationExcpetion("You do not have permission to edit this comment!");
            }
            return this.commentRepository.UpdateComment(id, comment);
        }

        public bool DeleteComment(int id, User user)
        {
            Comment commentToDelete = this.commentRepository.GetCommentById(id);
            if (commentToDelete.UserID != user.ID && !user.IsAdmin)
            {
                throw new AuthorisationExcpetion("You do not have permission to delete this comment!");
            }
            return this.commentRepository.DeleteComment(id, commentToDelete);
        }
    }
}
