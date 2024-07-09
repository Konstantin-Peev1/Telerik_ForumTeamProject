using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
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
      //  private readonly AuthManager authManager;
        private readonly IPostService postService;
    
        public TagController(ITagService tagService, ModelMapper modelMapper, AuthManager authManager, IPostService postService) : base(authManager)
        {
            this.tagService = tagService;
            this.modelMapper = modelMapper;
           // this.authManager = authManager;
            this.postService = postService;
        }

        [HttpGet("")]
        [Authorize]
        public IActionResult Get([FromQuery] string desc)
        {
            User user = GetCurrentUser();
            List<Tag> tags = this.tagService.GetTagsByDesc(desc);
            List<TagResponseDTO> tagsToReturn = tags.Select(tag => this.modelMapper.Map(tag)).ToList();
            return Ok(tagsToReturn);
        }

        [HttpPut("{id}")]
        [Authorize]

        public IActionResult UpdateTags(int id, [FromBody] string desc)
        {
            User user = GetCurrentUser();
            Post post = this.postService.GetPost(id);
            Tag tag = this.tagService.UpdateTags(user, post, desc);
            PostUploadResponseDTO response = this.modelMapper.Map(post);

            return Ok(response);

        }

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
