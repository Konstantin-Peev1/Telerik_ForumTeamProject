using Microsoft.EntityFrameworkCore;
using Telerik_ForumTeamProject.Data;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;

namespace Telerik_ForumTeamProject.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationContext applicationContext;

        public CommentRepository(ApplicationContext applicationContext)
        {
            this.applicationContext = applicationContext;
        }

        public List<Comment> GetAllComments()
        {
            return GetComments().ToList();
        }

        public Comment GetCommentById(int id)
        {
            Comment comment = GetComments().FirstOrDefault(x => x.Id == id);

            return comment ?? throw new EntityNotFoundException("Comment does not exist.");
        }

        public Comment CreateComment(Comment comment)
        {
            this.applicationContext.Comments.Add(comment);
            this.applicationContext.SaveChanges();
            return comment;
        }

        public Comment UpdateComment(int id, Comment comment)
        {
            Comment commentToUpdate = this.GetCommentById(id);

            commentToUpdate.Content = comment.Content;
            this.applicationContext.SaveChanges();
            return commentToUpdate;
        }

        public bool DeleteComment(int id, Comment comment) 
        {
            comment = this.GetCommentById(id);
            this.applicationContext.Comments.Remove(comment);
            return this.applicationContext.SaveChanges() > 0;
            
        }

        private IQueryable<Comment> GetComments()
        {
            return this.applicationContext.Comments;
        }
    }
}
