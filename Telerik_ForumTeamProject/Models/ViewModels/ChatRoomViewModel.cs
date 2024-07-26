using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Models.ViewModels
{
    public class ChatRoomViewModel
    {
        public ChatRoom ChatRoom { get; set; }
        public User CurrentUser { get; set; }
        public bool IsCreator { get; set; }
        public bool IsAdmin { get; set; }
    }
}
