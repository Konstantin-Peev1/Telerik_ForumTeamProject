using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentControllerAPI : BaseControllerAPI
    {
        private readonly ICommentService commentService;
        private readonly IPostService postService;
        private readonly ModelMapper modelMapper;

        public CommentControllerAPI(ICommentService commentService, IPostService postService, AuthManager authManager, ModelMapper modelMapper)
            : base(authManager)
        {
            this.commentService = commentService;
            this.modelMapper = modelMapper;
            this.postService = postService;
        }

        /// <summary>
        /// Retrieves all comments for a specific post.
        /// </summary>
        /// <param name="postId">ID of the post.</param>
        /// <returns>List of comments with replies for the specified post.</returns>
        [HttpGet("")]
        [Authorize()]
        public IActionResult GetAllPostComments(int postId)
        {
            User user = GetCurrentUser();
            Post post = postService.GetPost(postId);
            List<CommentReplyResponseDTO> response = this.modelMapper.Map(post.Comments);

            return this.Ok(response);
        }

        /// <summary>
        /// Retrieves a paginated list of replies for a given parent comment.
        /// </summary>
        /// <param name="parentCommentId">The ID of the parent comment.</param>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A paginated list of replies with metadata.</returns>
        [HttpGet("replies")]
        [Authorize()]
        public IActionResult GetReplies(int parentCommentId, int page = 1, int pageSize = 10)
        {
            User currentUser = GetCurrentUser();
            PagedResult<Comment> pagedReplies = commentService.GetPagedReplies(parentCommentId, page, pageSize);


            List<ReplyResponseDTO> response = modelMapper.MapReplyResponse(pagedReplies.Items);

            return Ok(new { pagedReplies.Metadata, Data = response });   
        }

        /// <summary>
        /// Creates a new comment for a specific post.
        /// </summary>
        /// <param name="postId">ID of the post.</param>
        /// <param name="commentRequestDTO">Data transfer object for the comment request.</param>
        /// <returns>The created comment with a 201 status code.</returns>
        [HttpPost("")]
        [Authorize()]
        public IActionResult CreateComment(int postId, [FromBody] CommentRequestDTO commentRequestDTO)
        {
            User user = GetCurrentUser();
            Post post = postService.GetPost(postId);

            Comment comment = this.modelMapper.Map(commentRequestDTO, postId);
            Comment commentToCreate = this.commentService.CreateComment(comment, user);
            CommentReplyResponseDTO response = this.modelMapper.Map(commentToCreate);

            return this.StatusCode(StatusCodes.Status201Created, response);
        }

        /// <summary>
        /// Creates a reply to a specific comment.
        /// </summary>
        /// <param name="parentCommentId">ID of the parent comment.</param>
        /// <param name="replyRequestDTO">Data transfer object for the reply request.</param>
        /// <returns>The created reply with a 201 status code.</returns>
        [HttpPost("{parentCommentId}/reply")]
        [Authorize()]
        public IActionResult CreateReply(int parentCommentId, [FromBody] CommentRequestDTO replyRequestDTO)
        {
            User user = GetCurrentUser();

            Comment reply = modelMapper.Map(replyRequestDTO);
            Comment replyToCreate = commentService.CreateReply(reply, parentCommentId, user);
            CommentReplyResponseDTO response = modelMapper.Map(replyToCreate);

            return this.StatusCode(StatusCodes.Status201Created, response);
        }

        /// <summary>
        /// Updates a specific comment.
        /// </summary>
        /// <param name="commentId">ID of the comment to update.</param>
        /// <param name="commentRequestDTO">Data transfer object for the comment update request.</param>
        /// <returns>The updated comment.</returns>
        [HttpPut("")]
        [Authorize()]
        public IActionResult UpdateComment(int commentId, [FromBody] CommentRequestDTO commentRequestDTO)
        {
            User user = GetCurrentUser();

            Comment comment = this.modelMapper.Map(commentRequestDTO, commentId);
            Comment commentToEdit = this.commentService.UpdateComment(commentId, comment, user);
            CommentReplyResponseDTO response = this.modelMapper.Map(commentToEdit);

            return Ok(response);
        }

        /// <summary>
        /// Deletes a specific comment.
        /// </summary>
        /// <param name="commentId">ID of the comment to delete.</param>
        /// <returns>A boolean indicating whether the comment was deleted successfully.</returns>
        [HttpDelete("")]
        [Authorize()]
        public IActionResult DeleteComment(int commentId)
        {
            User user = GetCurrentUser();
            bool commentDeleted = this.commentService.DeleteComment(commentId, user);
            return Ok(commentDeleted);
        }

    }
}
