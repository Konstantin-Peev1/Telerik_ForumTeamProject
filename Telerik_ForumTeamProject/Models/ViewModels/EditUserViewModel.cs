using System.ComponentModel.DataAnnotations;

namespace Telerik_ForumTeamProject.Models.ViewModels
{
    public class EditUserViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}
