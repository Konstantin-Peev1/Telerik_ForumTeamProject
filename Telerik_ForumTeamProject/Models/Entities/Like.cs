using System.ComponentModel.DataAnnotations;

namespace Telerik_ForumTeamProject.Models.Entities
{
    public class Like
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }  
        public int PostID { get; set; }
        public Post Post { get; set; }
    }
}
