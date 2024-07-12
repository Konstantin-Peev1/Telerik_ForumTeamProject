using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Security.Claims;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Services;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService userService;
        private readonly ModelMapper modelMapper;
        private readonly ICloudinaryService cloudinaryService;
      //  private readonly AuthManager authManager;

        public UserController(IUserService userService, ModelMapper modelMapper, AuthManager authManager, ICloudinaryService cloudinaryService) : base(authManager)
        {
            this.userService = userService;
            this.modelMapper = modelMapper;
            this.cloudinaryService = cloudinaryService;
        }


        [HttpGet("")]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Get([FromQuery] string information)
        {
            User user = GetCurrentUser();
            var userInfo = this.userService.GetByInformation(information);
            var userToDisplay = modelMapper.Map(userInfo);
            return this.Ok(userToDisplay);
             //trycatch later -> think of where to put them 
        }



        [HttpPut("{id}")]
        [Authorize()]
        public IActionResult Update(int id, [FromBody] UserRequestDTO userRequest)
        {
            User user = GetCurrentUser();
            User userUpdateInfo = this.modelMapper.Map(userRequest);
            var updatedUser = this.userService.UpdateUser(user, userUpdateInfo, id);
            var userToReturn = this.modelMapper.Map(updatedUser);
            return this.Ok(userToReturn);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LogInRequestDTO loginRequest)
        {
            try
            {
                var user = authManager.Authenticate(loginRequest.UserName, loginRequest.Password);
                var token = authManager.Generate(user);
                return Ok(token);
            }
            catch (AuthorisationExcpetion ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(UserRequestDTO registerRequest)
        {
            try
            {
                var newUser = modelMapper.Map(registerRequest);
                // Hash the password before saving
                newUser.Password = authManager.HashPassword(newUser.Password);
                var createdUser = userService.CreateUser(newUser);
                var createdUserResponse = modelMapper.Map(createdUser);
                return StatusCode(StatusCodes.Status201Created, createdUserResponse);
            }
            catch (DuplicateEntityException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("{userId}/uploadProfilePicture")]
        [Authorize]
        public async Task<IActionResult> UploadProfilePicture(int userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Upload image to Cloudinary
            var uploadResult = await this.cloudinaryService.UploadImageAsync(file);

            if (uploadResult == null)
                return BadRequest("Error uploading image.");

            try
            {
                // Update user's profile picture URL
                var updatedUser = this.userService.UpdateProfilePicture(userId, uploadResult.Url);

                return Ok(new { updatedUser.ProfilePictureUrl });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("block")]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Block([FromQuery] string userName)
        {
            User user = GetCurrentUser();
            var userInfo = this.userService.GetByInformation(userName);
            var blockedUser = this.userService.BlockUser(userInfo);
            var userToDisplay = modelMapper.Map(blockedUser);
            return this.Ok(userToDisplay);
            //trycatch later -> think of where to put them 
        }

        [HttpPost("unblock")]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult UnBlock([FromQuery] string userName)
        {
            User user = GetCurrentUser();
            var userInfo = this.userService.GetByInformation(userName);
            var unblockedUser = this.userService.UnBlockUser(userInfo);
            var userToDisplay = modelMapper.Map(unblockedUser);
            return this.Ok(userToDisplay);
            //trycatch later -> think of where to put them 
        }

        [HttpPost("makeAdmin")]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult MakeAdmin([FromQuery] string userName)
        {
            User user = GetCurrentUser();
            var userInfo = this.userService.GetByInformation(userName);
            var newAdmin = this.userService.MakeAdmin(userInfo);
            var newAdminToDisplay = modelMapper.Map(newAdmin);
            return this.Ok(newAdminToDisplay);
            //trycatch later -> think of where to put them 
        }

    }
}
