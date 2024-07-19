namespace Telerik_ForumTeamProject.Models.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int UserId {  get; set; }
        public User User { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }
        public int ChatRoomId { get; set; }
        public ChatRoom ChatRoom { get; set; }

    }
}
