using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Controllers
{
    [Route("api/likes")]
    [ApiController]
    public class LikeController : BaseController
    {
        private readonly ILikeService likeService;
        private readonly ModelMapper mapper;
     //   private readonly AuthManager authManager;

        public LikeController(ILikeService likeService, ModelMapper mapper, AuthManager authManager) : base(authManager)
        {
            this.likeService = likeService;
            this.mapper = mapper;
          //  this.authManager = authManager;
        }

        [HttpPost("{id}")]
        [Authorize]
        public IActionResult AddRemoveLike(int id)
        {
            User user = GetCurrentUser();
            Like like = this.likeService.Create(id, user);
            LikeResponseDTO likeToReturn = mapper.Map(like);
            return Ok(likeToReturn);
        }
       
    }
}
