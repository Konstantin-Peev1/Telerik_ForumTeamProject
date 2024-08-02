using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Security.Claims;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Hubs;
using Telerik_ForumTeamProject.Models;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.ViewModels;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Controllers.MVC
{


    [Authorize]
    public class ChatController : BaseControllerMVC
    {
        private readonly IChatService _chatService;
        

        public ChatController(IChatService chatService, AuthManager auth) :base(auth)
        {
            _chatService = chatService;
         
        }

        public IActionResult Index()
        {
            var activeChats = _chatService.GetActiveChats();
            _chatService.DeleteChatRoomIFOld();
            return View(activeChats);
        }

        public IActionResult Room(int id)
        {
            var chatRoom = _chatService.GetChatRoom(id);
            if (chatRoom == null)
            {
                return NotFound();
            }

            
            User currentUser = GetCurrentUser();

            var viewModel = new ChatRoomViewModel
            {
                ChatRoom = chatRoom,
                CurrentUser = currentUser
            };

            return View(viewModel);
        }

        [HttpPost]
        
        public IActionResult SendMessage(int chatRoomId, string message)
        {
            User user = GetCurrentUser();
            var userId = user.ID;
            var userName = user.UserName;
            _chatService.AddMessage(chatRoomId, userId, userName, message);
            return RedirectToAction("Room", new { id = chatRoomId });
        }


        

        [HttpPost]
        public IActionResult CreateChatRoom(string name) // New action method for creating chat rooms
        {
            User user = GetCurrentUser();
            if(user.ChatRooms.Count() != 0)
            {
                throw new AuthorisationExcpetion("No more chatRooms");
            }
            if (!string.IsNullOrEmpty(name))
            {
                _chatService.CreateChatRoom(name, user);
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        
        public IActionResult DeleteChatRoom(int chatRoomId)
        {
            User user = GetCurrentUser();
            try
            {
                _chatService.DeleteChatRoom(chatRoomId, user);
                return RedirectToAction("Index");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

    }
}
