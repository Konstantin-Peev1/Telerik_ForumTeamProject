using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
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


            var userInfo = this.userService.GetByInformation(information, user);
            var userToDisplay = modelMapper.Map(userInfo);
            return this.Ok(userToDisplay);
             //trycatch later -> think of where to put them 
        }

        [HttpPost("")]
        public IActionResult Create([FromBody] UserRequestDTO user)
        {
            var createdUser = this.userService.CreateUser(this.modelMapper.Map(user));
            var createdUserResponse = this.modelMapper.Map(createdUser);

            return this.StatusCode(StatusCodes.Status201Created, createdUserResponse);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromHeader] string credentials, [FromBody] UserRequestDTO userRequest)
        {
            User user = this.authManager.TryGetUser(credentials);
            User userUpdateInfo = this.modelMapper.Map(userRequest);
            var updatedUser = this.userService.UpdateUser(user, userUpdateInfo, id);
            var userToReturn = this.modelMapper.Map(updatedUser);
            return this.Ok(userToReturn);
        }


    }
}
