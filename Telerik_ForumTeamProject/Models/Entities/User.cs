using System.ComponentModel.DataAnnotations;
using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Models.Entities
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The {0} field is required and must not be an empty string.")]
        
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The {0} field is required and must not be an empty string.")]
        [MaxLength(32, ErrorMessage = "The {0} field must be less than {1} characters.")]
        [MinLength(4, ErrorMessage = "The {0} field must be at least {1} character.")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The {0} field is required and must not be an empty string.")]
        [MaxLength(32, ErrorMessage = "The {0} field must be less than {1} characters.")]
        [MinLength(4, ErrorMessage = "The {0} field must be at least {1} character.")]
        public string LastName { get; set; }
        public string Email { get; set; } 
        public string Password { get; set; }
      
        public bool IsAdmin { get; set; } 
        public bool IsBlocked { get; set; }

        public List<Post> Posts { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Reply> Replies { get; set; }
     

    }
}
