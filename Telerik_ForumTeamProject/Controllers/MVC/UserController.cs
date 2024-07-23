using Microsoft.AspNetCore.Mvc;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Services;
using Telerik_ForumTeamProject.Services.Contracts;
using Telerik_ForumTeamProject.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Telerik_ForumTeamProject.Models.ViewModels;
using System.Linq;

namespace Telerik_ForumTeamProject.Controllers.MVC
{
    public class UserController : Controller
    {
        private readonly IUserService userService;
        private readonly ModelMapper modelMapper;
        private readonly AuthManager authManager;
        private readonly ILogger<UserController> logger;

        public UserController(IUserService userService, ModelMapper modelMapper, AuthManager authManager, ILogger<UserController> logger)
        {
            this.userService = userService;
            this.modelMapper = modelMapper;
            this.authManager = authManager;
            this.logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            var model = new LogInRequestDTO();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(LogInRequestDTO loginRequest)
        {
            if (loginRequest == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login request");
                return View(new LogInRequestDTO());
            }

            logger.LogInformation("Received login request with UserName: {UserName}", loginRequest.UserName);

            try
            {
                var user = authManager.Authenticate(loginRequest.UserName, loginRequest.Password);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(loginRequest);
                }

                var token = authManager.Generate(user);
                var sessionId = GenerateSessionId();

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Set to true if using HTTPS
                    Expires = DateTime.UtcNow.AddMinutes(10) // Match the token expiration
                };

                Response.Cookies.Append("AuthToken", token, cookieOptions);
                Response.Cookies.Append("UserId", user.UserName.ToString(), cookieOptions);

                // Store the sessionId in TempData
                TempData["SessionId"] = sessionId;
                TempData["IsAuthenticated"] = true;
                return RedirectToAction("Index", "Home");
            }
            catch (AuthorisationExcpetion ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(loginRequest);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new UserRequestDTO());
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register(UserRequestDTO registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(registerRequest);
            }

            try
            {
                var user = modelMapper.Map(registerRequest);
                user.Password = authManager.HashPassword(user.Password);
                var createdUser = userService.CreateUser(user);
                var token = authManager.Generate(user);
                var sessionId = GenerateSessionId();

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Set to true if using HTTPS
                    Expires = DateTime.UtcNow.AddMinutes(10) // Match the token expiration
                };

                Response.Cookies.Append("AuthToken", token, cookieOptions);
                Response.Cookies.Append("SessionId", sessionId, cookieOptions);

                return RedirectToAction("Index", "Home");
            }
            catch (DuplicateEntityException ex)
            {
                if (userService.UserExists(registerRequest.UserName))
                {
                    ModelState.AddModelError("UserName", "Username is already taken.");
                }

                if (userService.UserExistsEmail(registerRequest.Email))
                {
                    ModelState.AddModelError("Email", "Email is already taken.");
                }

                return View(registerRequest);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while registering the user.");
                return View(registerRequest);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("UserId");

            TempData.Clear();
            TempData["IsAuthenticated"] = false;

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Search(string query)
        {
            var users = userService.SearchUsers(query);
            var model = new UserSearchViewModel
            {
                Query = query,
                Users = users.ToList()
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            // Placeholder for user details
            return View();
        }

        private string GenerateSessionId()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var byteArray = new byte[16];
                rng.GetBytes(byteArray);
                return BitConverter.ToString(byteArray).Replace("-", "").ToLower();
            }
        }
    }
}
