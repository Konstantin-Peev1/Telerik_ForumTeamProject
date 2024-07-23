using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Models.ViewModels
{
    public class UserSearchViewModel
    {
        public string Query { get; set; }
        public List<User> Users { get; set; }
    }
}
