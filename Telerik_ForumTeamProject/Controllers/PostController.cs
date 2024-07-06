using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Controllers
{
    [Route("api/post")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService postService;
        private readonly ModelMapper modelMapper;
        private readonly AuthManager authManager;

        public PostController(IPostService postService, ModelMapper modelMapper, AuthManager authManager)
        {
            this.postService = postService;
            this.modelMapper = modelMapper;
            this.authManager = authManager;
        }


        [HttpGet("latest")]

        public IActionResult GetLatest10()
        {
            var posts = this.postService.GetTop10Recent().ToList();
            List<PostUploadResponseDTO> postsToShow = posts.Select(post => this.modelMapper.Map(post)).ToList();
            return Ok(postsToShow);
        }

        [HttpGet("most-commented")]

        public IActionResult GetMostCommented10()
        {
            var posts = this.postService.GetTop10Commented().ToList();
            List<PostUploadResponseDTO> postsToShow = posts.Select(post => this.modelMapper.Map(post)).ToList();
            return Ok(postsToShow);
        }

        [HttpGet("filtered-posts")]
        public IActionResult Get([FromHeader] string credentials, [FromQuery] PostQueryParamteres paramteres)
        {
            User user = this.authManager.TryGetUser(credentials);
            var postsToUpload = this.postService.FilterBy(paramteres).Select(post => this.modelMapper.Map(post)).ToList();
            return Ok(postsToUpload);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromHeader] string credentials, int id)
        {
            User user = this.authManager.TryGetUser(credentials);
            Post post = this.postService.GetPost(id);
            var postToShow = this.modelMapper.Map(post);

            return Ok(postToShow);
        }

        [HttpPost("")]
        public IActionResult CreatePost([FromHeader] string credentials, [FromBody] PostRequestDTO postRequestDTO)
        {
            User user = this.authManager.TryGetUser(credentials);
            Post post = this.modelMapper.Map(postRequestDTO);
            Post createdPost = this.postService.CreatePost(post, user);
            var showPost = this.modelMapper.Map(createdPost);
            return this.StatusCode(StatusCodes.Status201Created, showPost);
        }

        [HttpPut("id")]

        public IActionResult UpdatePost([FromHeader] string credentials, [FromBody] PostRequestDTO postRequestDTO, int id)
        {
            User user = this.authManager.TryGetUser(credentials);
            Post post = this.modelMapper.Map(postRequestDTO);
            Post updatedPost = this.postService.UpdatePost(id, post, user);
            var showPost = this.modelMapper.Map(updatedPost);
            return this.Ok(showPost);

        }

        [HttpDelete("id")]

        public IActionResult DeletePost([FromHeader] string credentials, int id)
        {
            User user = this.authManager.TryGetUser(credentials);
            bool postDeleted = this.postService.DeletePost(id, user);
            return this.Ok(postDeleted);
        }

    }
}
