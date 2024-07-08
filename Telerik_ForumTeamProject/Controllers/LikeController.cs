using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Controllers
{
    [Route("api/likes")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly ILikeService likeService;
        private readonly ModelMapper mapper;
        private readonly AuthManager authManager;

        public LikeController(ILikeService likeService, ModelMapper mapper, AuthManager authManager)
        {
            this.likeService = likeService;
            this.mapper = mapper;
            this.authManager = authManager;
        }

        [HttpPost("{id}")]

        public IActionResult AddRemoveLike([FromHeader] string credentials, int id)
        {
            User user = this.authManager.TryGetUser(credentials);
            Like like = this.likeService.Create(id, user);
            return Ok(like);
        }
       
    }
}
