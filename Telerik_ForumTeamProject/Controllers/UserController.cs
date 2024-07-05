using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ModelMapper modelMapper;
        private readonly AuthManager authManager;

        public UserController(IUserService userService, ModelMapper modelMapper, AuthManager authManager)
        {
            this.userService = userService;
            this.modelMapper = modelMapper;
            this.authManager = authManager;
        }

        [HttpGet("")]

        public IActionResult Get([FromHeader] string credentials, [FromQuery] string information)
        {
            User user = this.authManager.TryGetUser(credentials);


            var userToDisplay = this.userService.GetByInformation(information, user);

            return this.Ok(userToDisplay);
             //trycatch later -> think of where to put them 
        }

        
    }
}
