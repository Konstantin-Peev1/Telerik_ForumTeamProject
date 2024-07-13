using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Services;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : BaseController
    {
        private readonly ICommentService commentService;
        private readonly IPostService postService;
        private readonly ModelMapper modelMapper;
      //  private readonly AuthManager authManager;

        public CommentController(ICommentService commentService, IPostService postService, AuthManager authManager, ModelMapper modelMapper) :base(authManager)
        {
            this.commentService = commentService;
           // this.authManager = authManager;
            this.modelMapper = modelMapper;
            this.postService = postService;
        }

        /*        [HttpGet("{id}")]
                public IActionResult GetById(int id)
                {
                    Comment comment = commentService.GetComment(id);
                    return this.Ok(comment);
                }*/

        [HttpGet("")]
        [Authorize()]
        public IActionResult GetAllPostComments(int postId)
        {
            User user = GetCurrentUser();
            Post post = postService.GetPost(postId);
            List<CommentReplyResponseDTO> response = this.modelMapper.Map(post.Comments);

            return this.Ok(response);
        }

        [HttpGet("replies")]
        [Authorize()]
        public IActionResult GetReplies(int parentCommentId, int skip = 0, int take = 5)
        {
            User currentUser = GetCurrentUser();
            List<Comment> replies = commentService.GetReplies(parentCommentId, skip, take);
            List<ReplyResponseDTO> response = modelMapper.MapReplyResponse(replies);

            return Ok(response);
        }

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
