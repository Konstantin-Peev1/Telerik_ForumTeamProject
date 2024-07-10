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
        public Comment CreateReply(Comment reply, int parentCommentId)
        {
            Comment parentComment = this.GetCommentById(parentCommentId);
            if(parentComment == null)
            {
                throw new EntityNotFoundException("Parent comment does not exist.");
            }
            reply.ParentCommentID = parentCommentId;
            this.applicationContext.Comments.Add(reply);
            this.applicationContext.SaveChanges();
            return reply;
        }

        public Comment UpdateComment(int id, Comment comment)
        {
            Comment commentToUpdate = this.GetCommentById(id);

            if(commentToUpdate == null)
            {
                throw new EntityNotFoundException("Comment does not exist.");
            }

            commentToUpdate.Content = comment.Content;
            this.applicationContext.SaveChanges();
            return commentToUpdate;
        }

        public bool DeleteComment(int id, Comment comment) 
        {
            comment = this.GetCommentById(id);

            if (comment == null)
            {
                throw new EntityNotFoundException("Comment does not exist.");
            }

            this.applicationContext.Comments.Remove(comment);
            return this.applicationContext.SaveChanges() > 0;
            
        }


        private IQueryable<Comment> GetComments()
        {
            return this.applicationContext.Comments;
        }
    }
}
