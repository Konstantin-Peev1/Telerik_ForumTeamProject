using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Controllers
{
    [Route("api/post")]
    [ApiController]
    public class PostController : BaseController
    {
        private readonly IPostService postService;
        private readonly ModelMapper modelMapper;
      //  private readonly AuthManager authManager;

        public PostController(IPostService postService, ModelMapper modelMapper, AuthManager authManager) :base(authManager)
        {
            this.postService = postService;
            this.modelMapper = modelMapper;
           // this.authManager = authManager;
        }

        [AllowAnonymous]
        [HttpGet("latest")]

        public IActionResult GetLatest10()
        {
            var posts = this.postService.GetTop10Recent().ToList();
            List<PostUploadResponseDTO> postsToShow = posts.Select(post => this.modelMapper.Map(post)).ToList();
            return Ok(postsToShow);
        }

        [AllowAnonymous]
        [HttpGet("most-commented")]

        public IActionResult GetMostCommented10()
        {
            var posts = this.postService.GetTop10Commented().ToList();
            List<PostUploadResponseDTO> postsToShow = posts.Select(post => this.modelMapper.Map(post)).ToList();
            return Ok(postsToShow);
        }

        [HttpGet("filtered-posts")]
        [Authorize]
        public IActionResult Get([FromQuery] PostQueryParamteres paramteres)
        {
            User user = GetCurrentUser();
            var postsToUpload = this.postService.FilterBy(paramteres).Select(post => this.modelMapper.Map(post)).ToList();
            return Ok(postsToUpload);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById([FromHeader] string credentials, int id)
        {
            User user = GetCurrentUser();
            Post post = this.postService.GetPost(id);
            var postToShow = this.modelMapper.Map(post);

            return Ok(postToShow);
        }

        [HttpPost("")]
        [Authorize]
        public IActionResult CreatePost([FromBody] PostRequestDTO postRequestDTO)
        {
            User user = GetCurrentUser();
            Post post = this.modelMapper.Map(postRequestDTO);
            Post createdPost = this.postService.CreatePost(post, user);
            var showPost = this.modelMapper.Map(createdPost);
            return this.StatusCode(StatusCodes.Status201Created, showPost);
        }

        [HttpPut("id")]
        [Authorize]
        public IActionResult UpdatePost([FromBody] PostRequestDTO postRequestDTO, int id)
        {
            User user = GetCurrentUser();
            Post post = this.modelMapper.Map(postRequestDTO);
            Post updatedPost = this.postService.UpdatePost(id, post, user);
            var showPost = this.modelMapper.Map(updatedPost);
            return this.Ok(showPost);

        }

        [HttpDelete("id")]
        [Authorize]
        public IActionResult DeletePost(int id)
        {
            User user = GetCurrentUser();
            bool postDeleted = this.postService.DeletePost(id, user);
            return this.Ok(postDeleted);
        }
       
    }
}
