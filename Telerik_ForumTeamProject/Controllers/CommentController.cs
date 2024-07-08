using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService commentService;
        private readonly IPostService postService;
        private readonly ModelMapper modelMapper;
        private readonly AuthManager authManager;

        public CommentController(ICommentService commentService, IPostService postService, AuthManager authManager, ModelMapper modelMapper)
        {
            this.commentService = commentService;
            this.authManager = authManager;
            this.modelMapper = modelMapper;
            this.postService = postService;
        }

/*        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            Comment comment = commentService.GetComment(id);
            return this.Ok(comment);
        }*/

        [HttpGet]
        public IActionResult GetAllPostComments([FromHeader] string credentials, int postId)
        {
            User user = this.authManager.TryGetUser(credentials);
            Post post = postService.GetPost(postId);
            List<CommentReplyResponseDTO> response = this.modelMapper.Map(post.Comments);

            return this.Ok(response);
        }


        [HttpPost]
        public IActionResult CreateComment([FromHeader] string credentials, int postId, [FromBody] CommentRequestDTO commentRequestDTO) 
        {
            User user = this.authManager.TryGetUser(credentials);
            Post post = postService.GetPost(postId);

            Comment comment = this.modelMapper.Map(commentRequestDTO, postId);
            Comment commentToCreate = this.commentService.CreateComment(comment, user);
            CommentReplyResponseDTO response = this.modelMapper.Map(commentToCreate);

            return this.StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPut]
        public IActionResult UpdateComment([FromHeader] string credentials, int commentId, [FromBody] CommentRequestDTO commentRequestDTO)
        {
            User user = this.authManager.TryGetUser(credentials);

            Comment comment = this.modelMapper.MapUpdateComment(commentRequestDTO, commentId);
            Comment commentToEdit = this.commentService.UpdateComment(commentId, comment, user);
            CommentReplyResponseDTO response = this.modelMapper.Map(commentToEdit);

            return Ok(response);
        }

        [HttpDelete]

        public IActionResult DeleteComment([FromHeader] string credentials, int commentId)
        {
            User user = this.authManager.TryGetUser(credentials);
            bool commentDeleted = this.commentService.DeleteComment(commentId, user);
            return Ok(commentDeleted);
        }

    }
}
