using System.ComponentModel.DataAnnotations;

namespace Telerik_ForumTeamProject.Models.Entities
{
    public class Tag
    {
        [Key]
        public int ID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The {0} field is required and must not be an empty string.")]
        [MaxLength(12, ErrorMessage = "The {0} field must be less than {1} characters.")]
        [MinLength(3, ErrorMessage = "The {0} field must be at least {1} character.")]
        public string Description { get; set; }
        public List<Post> Posts { get; set; }
    }
}
