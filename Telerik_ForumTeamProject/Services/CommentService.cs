using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.ResponseDTO;
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

        public ICollection<Comment> GetAllPostComments(int postId)
        {
            return this.commentRepository.GetAllPostComments(postId);
        }

        public PagedResult<Comment> GetPagedReplies(int parentCommentId, int page, int pageSize)
        {
            ICollection<Comment> replies = this.commentRepository.GetPagedReplies(parentCommentId, page, pageSize);
            int totalReplies = this.GetRepliesCount(parentCommentId);

            PaginationMetadata paginationMetadata = new PaginationMetadata()
            {
                TotalCount = totalReplies,
                PageSize = pageSize,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalReplies / (double)pageSize)
            };

            if(page > paginationMetadata.TotalPages)
            {
                throw new PageNotFoundException("Page not found!");
            }

            return new PagedResult<Comment>
            {
                Items = replies,
                Metadata = paginationMetadata
            };
        }
        public Comment GetComment(int id)
        {
            return this.commentRepository.GetCommentById(id);
        }
        public int GetRepliesCount(int parentCommentId)
        {
            return this.commentRepository.GetRepliesCount(parentCommentId);
        }

        public Comment CreateComment(Comment comment, User user)
        {
            if (user.IsBlocked)
            {
                throw new AuthorisationExcpetion("Blocked users can't create comments");
            }
            comment.UserID = user.ID;
            Comment result = this.commentRepository.CreateComment(comment);
            return result;
        }
        public Comment CreateReply(Comment reply, int parentCommentId, User user)
        {
            if (user.IsBlocked)
            {
                throw new AuthorisationExcpetion("Blocked users can't reply to comments");
            }
            reply.UserID = user.ID;
            Comment result = this.commentRepository.CreateReply(reply, parentCommentId);
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
