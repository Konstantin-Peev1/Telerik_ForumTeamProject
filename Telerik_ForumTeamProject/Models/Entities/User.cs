using System.ComponentModel.DataAnnotations;
using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Models.Entities
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        
        public string UserName { get; set; }

       
        public string FirstName { get; set; }

      
        public string LastName { get; set; }
        public string Email { get; set; } 
        public string Password { get; set; }
      
        public bool IsAdmin { get; set; } 

        public string Role { get; set; }
        public bool IsBlocked { get; set; }

        public List<Post> Posts { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Like> Likes { get;set; }
        public string? ProfilePictureUrl { get; set; }


    }
}
