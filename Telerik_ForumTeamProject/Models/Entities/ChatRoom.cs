namespace Telerik_ForumTeamProject.Models.Entities
{
    public class ChatRoom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ChatMessage> Messages { get; set; }
        
    }
}
