using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Security.Claims;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Hubs;
using Telerik_ForumTeamProject.Models;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Controllers.MVC
{
   
    

    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private readonly AuthManager _authManager;

        public ChatController(IChatService chatService, AuthManager auth)
        {
            _chatService = chatService;
            _authManager = auth;
        }

        public IActionResult Index()
        {
            var activeChats = _chatService.GetActiveChats();
            return View(activeChats);
        }

        public IActionResult Room(int id)
        {
            var chatRoom = _chatService.GetChatRoom(id);
            if (chatRoom == null)
            {
                return NotFound();
            }

            // Retrieve current user
            User currentUser = GetCurrentUser();

            var viewModel = new ChatRoomViewModel
            {
                ChatRoom = chatRoom,
                CurrentUser = currentUser
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public IActionResult SendMessage(int chatRoomId, string message)
        {
            User user = GetCurrentUser();
            var userId = user.ID;
            var userName = user.UserName;
            _chatService.AddMessage(chatRoomId, userId, userName, message);
            return RedirectToAction("Room", new { id = chatRoomId });
        }


        protected User GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var userClaims = identity.Claims;
                var userName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value;
                var password = userClaims.First(o => o.Type == ClaimTypes.Hash)?.Value;
                string credentials = $"{userName}:{password}";
                return _authManager.TryGetUser(credentials);
            }
            throw new EntityNotFoundException("No such user");
        }

        [HttpPost]
        public IActionResult CreateChatRoom(string name) // New action method for creating chat rooms
        {
            if (!string.IsNullOrEmpty(name))
            {
                _chatService.CreateChatRoom(name);
            }
            return RedirectToAction("Index");
        }

     
    }
}
