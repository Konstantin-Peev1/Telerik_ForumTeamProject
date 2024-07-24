using System.ComponentModel.DataAnnotations;
using Telerik_ForumTeamProject.Models.ResponseDTO;

namespace Telerik_ForumTeamProject.Models.ViewModels
{
    public class PostViewModel
    {
        public int id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The {0} field is required and must not be an empty string.")]
        [MaxLength(64, ErrorMessage = "The {0} field must be less than {1} characters.")]
        [MinLength(16, ErrorMessage = "The {0} field must be at least {1} character.")]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The {0} field is required and must not be an empty string.")]
        [MaxLength(8192, ErrorMessage = "The {0} field must be less than {1} characters.")]
        [MinLength(32, ErrorMessage = "The {0} field must be at least {1} character.")]
        public string Content { get; set; }
        public int Likes { get; set; }
        public string PostDate { get; set; }
        public List<CommentReplyResponseDTO> Comments { get; set; }
        public string UserName { get; set; }
        public List<string> Tags { get; set; }
        public List<CommentReplyResponseDTO> Replies { get; set; }
    }
}
