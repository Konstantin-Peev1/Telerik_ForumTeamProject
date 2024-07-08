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
        private readonly ModelMapper modelMapper;
        private readonly AuthManager authManager;

        public CommentController(ICommentService commentService, AuthManager authManager, ModelMapper modelMapper)
        {
            this.commentService = commentService;
            this.authManager = authManager;
            this.modelMapper = modelMapper;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            Comment comment = commentService.GetComment(id);

            return this.Ok(comment);
        }

        [HttpGet]
        public IActionResult GetAll([FromHeader] string credentials) 
        {
            User user = this.authManager.TryGetUser(credentials);
            List<Comment> comments = this.commentService.GetComments();
            List<CommentReplyResponseDTO> response = this.modelMapper.Map(comments);
            return this.Ok(response);
        }

        [HttpPost]
        public IActionResult CreateComment([FromHeader] string credentials,[FromBody] CommentRequestDTO commentRequestDTO) 
        {
            User user = this.authManager.TryGetUser(credentials);
            Comment comment = this.modelMapper.Map(commentRequestDTO);
            Comment commentToCreate = this.commentService.CreateComment(comment, user);
            CommentReplyResponseDTO response = this.modelMapper.Map(commentToCreate);
            return this.StatusCode(StatusCodes.Status201Created, response);
        }

    }
}
