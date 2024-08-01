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
    public class UserControllerAPI : BaseControllerAPI
    {
        private readonly IUserService userService;
        private readonly ModelMapper modelMapper;
        private readonly ICloudinaryService cloudinaryService;
        private readonly List<string> allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png" };

        public UserControllerAPI(IUserService userService, ModelMapper modelMapper, AuthManager authManager, ICloudinaryService cloudinaryService) : base(authManager)
        {
            this.userService = userService;
            this.modelMapper = modelMapper;
            this.cloudinaryService = cloudinaryService;
        }

        /// <summary>
        /// Retrieves user information based on provided search criteria.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible only to administrators.
        /// The search criteria can be a username, email, or first name.
        /// </remarks>
        /// <param name="information">The search criteria (username, email, or first name).</param>
        /// <returns>
        /// A UserResponseDTO containing the user's information if found, or a 404 Not Found response if not found.
        /// </returns>
        [HttpGet("")]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Get([FromQuery] string information)
        {
            try
            {
                User user = GetCurrentUser();
                var userInfo = this.userService.GetByInformation(information);
                var userToDisplay = modelMapper.Map(userInfo);
                return this.Ok(userToDisplay);
            }
            catch(EntityNotFoundException ex)
            {
                return this.NotFound(ex);
            }
           
             //trycatch later -> think of where to put them 
        }

        /// <summary>
        /// Updates user information.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible only to authenticated users.
        /// Users can update their own information.
        /// </remarks>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="userRequest">The updated user data in the request body.</param>
        /// <returns>A UserResponseDTO containing the updated user information.</returns>
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

        /// <summary>
        /// Updates user information.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible only to authenticated users.
        /// Users can update their own information.
        /// </remarks>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="userRequest">The updated user data in the request body.</param>
        /// <returns>A UserResponseDTO containing the updated user information.</returns>
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

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerRequest">User registration data.</param>
        /// <returns>
        /// A 201 Created response with a UserResponseDTO containing the new user's information if successful.
        /// A 409 Conflict response if a user with the same username or email already exists.
        /// </returns>
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

        /// <summary>
        /// Uploads a profile picture for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="file">The image file to upload.</param>
        /// <returns>
        /// An Ok (200) response with the updated profile picture URL if successful.
        /// A BadRequest (400) response with an error message if the file is invalid or there's an upload error.
        /// A NotFound (404) response if the user is not found.
        /// A 500 Internal Server Error response for unexpected errors.
        /// </returns>
        [HttpPost("{userId}/uploadProfilePicture")]
        [Authorize]
        public async Task<IActionResult> UploadProfilePicture(int userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            //Validate file extension
            var extension = Path.GetExtension(file.FileName);
            if (!this.allowedExtensions.Contains(extension))
            {
                return BadRequest("Invalid file format. Only .jpg, .jpeg, and .png are allowed.");
            }

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

        /// <summary>
        /// Blocks the user.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible only to administrators.
        /// </remarks>
        /// <param name="userName">The username of the user to block.</param>
        /// <returns>
        /// A UserResponseDTO containing the blocked user's information if found, or a 404 Not Found response if not found.
        /// </returns>
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

        /// <summary>
        /// Unblocks the user.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible only to administrators.
        /// </remarks>
        /// <param name="userName">The username of the user to unblock.</param>
        /// <returns>
        /// A UserResponseDTO containing the unblocked user's information if found, or a 404 Not Found response if not found.
        /// </returns>
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

        /// <summary>
        /// Makes the user an admin.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible only to administrators.
        /// </remarks>
        /// <param name="userName">The username of the user to make an admin.</param>
        /// <returns>
        /// A UserResponseDTO containing the new admin's information if found, or a 404 Not Found response if not found.
        /// </returns>
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

        [HttpPost("makeUser")]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult MakeUser([FromQuery] string userName)
        {
            User user = GetCurrentUser();
            var userInfo = this.userService.GetByInformation(userName);
            var newUser = this.userService.MakeAdmin(userInfo);
            var newUserToDisplay = modelMapper.Map(newUser);
            return this.Ok(newUserToDisplay);
        }

    }
}
