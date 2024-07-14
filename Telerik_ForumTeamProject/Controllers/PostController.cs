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

        public PostController(IPostService postService, ModelMapper modelMapper, AuthManager authManager) :base(authManager)
        {
            this.postService = postService;
            this.modelMapper = modelMapper;
        }

        /// <summary>
        /// Gets the 10 latest posts.
        /// </summary>
        /// <returns>A list of PostUploadResponseDTO objects representing the latest posts.</returns>
        [AllowAnonymous]
        [HttpGet("latest")]
        public IActionResult GetLatest10()
        {
            var posts = this.postService.GetTop10Recent().ToList();
            List<PostUploadResponseDTO> postsToShow = posts.Select(post => this.modelMapper.Map(post)).ToList();
            return Ok(postsToShow);
        }

        /// <summary>
        /// Gets the 10 most commented posts.
        /// </summary>
        /// <returns>A list of PostUploadResponseDTO objects representing the most commented posts.</returns>
        [AllowAnonymous]
        [HttpGet("most-commented")]
        public IActionResult GetMostCommented10()
        {
            var posts = this.postService.GetTop10Commented().ToList();
            List<PostUploadResponseDTO> postsToShow = posts.Select(post => this.modelMapper.Map(post)).ToList();
            return Ok(postsToShow);
        }
        /// <summary>
        /// Gets a filtered list of posts based on the provided query parameters.
        /// </summary>
        /// <param name="parameters">Query parameters for filtering (optional).</param>
        /// <returns>A list of PostUploadResponseDTO objects representing the filtered posts.</returns>
        [HttpGet("filtered-posts")]
        [Authorize]
        public IActionResult Get([FromQuery] PostQueryParamteres paramteres)
        {
            User user = GetCurrentUser();
            var postsToUpload = this.postService.FilterBy(paramteres).Select(post => this.modelMapper.Map(post)).ToList();
            return Ok(postsToUpload);
        }

        /// <summary>
        /// Gets a post by its ID. (Admin access only)
        /// </summary>
        /// <param name="credentials">Authorization credentials in the header.</param>
        /// <param name="id">The ID of the post to retrieve.</param>
        /// <returns>A PostUploadResponseDTO object representing the post if found, or a 404 Not Found response if not found.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById([FromHeader] string credentials, int id)
        {
            User user = GetCurrentUser();
            Post post = this.postService.GetPost(id);
            var postToShow = this.modelMapper.Map(post);

            return Ok(postToShow);
        }

        /// <summary>
        /// Creates a new post.
        /// </summary>
        /// <param name="postRequestDTO">The data for the new post.</param>
        /// <returns>A 201 Created response with the newly created PostUploadResponseDTO object.</returns>
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

        /// <summary>
        /// Updates an existing post by ID.
        /// </summary>
        /// <param name="postRequestDTO">The updated post data.</param>
        /// <param name="id">The ID of the post to update.</param>
        /// <returns>An updated PostUploadResponseDTO object representing the modified post.</returns>
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
        /// <summary>
        /// Deletes a post by ID.
        /// </summary>
        /// <param name="id">The ID of the post to delete.</param>
        /// <returns>A boolean indicating whether the post was successfully deleted.</returns>
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
