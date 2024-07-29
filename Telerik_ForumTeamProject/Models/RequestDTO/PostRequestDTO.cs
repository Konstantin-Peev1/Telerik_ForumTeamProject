using System.ComponentModel.DataAnnotations;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.ResponseDTO;

namespace Telerik_ForumTeamProject.Models.RequestDTO
{
    public class PostRequestDTO
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "The {0} field is required and must not be an empty string.")]
        [MaxLength(64, ErrorMessage = "The {0} field must be less than {1} characters.")]
        [MinLength(16, ErrorMessage = "The {0} field must be at least {1} character.")]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The {0} field is required and must not be an empty string.")]
        [MaxLength(8192, ErrorMessage = "The {0} field must be less than {1} characters.")]
        [MinLength(32, ErrorMessage = "The {0} field must be at least {1} character.")]
        public string Content { get; set; }
        public List<string> TagDescriptions { get; set; } = new List<string>();

    }
}
