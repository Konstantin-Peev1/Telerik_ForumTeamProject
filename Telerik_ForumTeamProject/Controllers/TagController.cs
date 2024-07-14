using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Services.Contracts;


namespace Telerik_ForumTeamProject.Controllers
{
    [Route("api/tag")]
    [ApiController]
    public class TagController : BaseController
    {
        private readonly ITagService tagService;
        private readonly ModelMapper modelMapper;
        private readonly IPostService postService;
    
        public TagController(ITagService tagService, ModelMapper modelMapper, AuthManager authManager, IPostService postService) : base(authManager)
        {
            this.tagService = tagService;
            this.modelMapper = modelMapper;
            this.postService = postService;
        }

        /// <summary>
        /// Retrieves a list of tags based on a search query.
        /// </summary>
        /// <remarks>
        /// If the `desc` parameter is provided, the API will return tags 
        /// whose description contains the specified text (case-insensitive).
        /// If `desc` is not provided, all tags are returned.
        /// This endpoint requires authentication.
        /// </remarks>
        /// <param name="desc">Optional: Search query for filtering tags by description.</param>
        /// <returns>
        /// An Ok (200) response with a list of `TagResponseDTO` objects representing the filtered tags.
        /// If no tags are found, an empty list is returned.
        /// </returns>
        [HttpGet("")]
        [Authorize]
        public IActionResult Get([FromQuery] string desc)
        {
            User user = GetCurrentUser();
            ICollection<Tag> tags = this.tagService.GetTagsByDesc(desc);
            List<TagResponseDTO> tagsToReturn = tags.Select(tag => this.modelMapper.Map(tag)).ToList();
            return Ok(tagsToReturn);
        }

        /// <summary>
        /// Updates the tags associated with a specific post.
        /// </summary>
        /// <remarks>
        /// This endpoint allows adding or updating a tag on a post. 
        /// If the tag does not exist, it will be created.
        /// The current user must be authorized to modify the post's tags.
        /// </remarks>
        /// <param name="id">The ID of the post to update.</param>
        /// <param name="desc">The description of the tag to add/update.</param>
        /// <returns>
        /// An Ok (200) response with an updated `PostUploadResponseDTO` 
        /// representing the post with the modified tags.
        /// If the post is not found or the user is unauthorized, 
        /// a corresponding error response (404 Not Found or 403 Forbidden) is returned.
        /// </returns>
        [HttpPut("{id}")]

        public IActionResult UpdateTags(int id, [FromBody] string desc)
        {
            User user = GetCurrentUser();
            Post post = this.postService.GetPost(id);
            Tag tag = this.tagService.UpdateTags(user, post, desc);
            PostUploadResponseDTO response = this.modelMapper.Map(post);

            return Ok(response);

        }

        /// <summary>
        /// Removes a tag from a specified post.
        /// </summary>
        /// <remarks>
        /// The current user must be authorized to modify the post's tags.
        /// </remarks>
        /// <param name="id">The ID of the post.</param>
        /// <param name="desc">The description of the tag to remove.</param>
        /// <returns>
        /// An Ok (200) response with a boolean value indicating whether the tag was successfully removed.
        /// If the post is not found or the user is unauthorized, 
        /// a corresponding error response (404 Not Found or 403 Forbidden) is returned.
        /// </returns>
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id, [FromBody] string desc)
        {
            User user = GetCurrentUser();
            Post post = this.postService.GetPost(id);
            bool deletedTag = this.tagService.RemoveTags(user, post, desc);
            
            return Ok(deletedTag);
        }
    }
}
