using System.Collections.Generic;
using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Models.ViewModels
{
    public class HomeViewModel
    {
        public bool IsAuthenticated { get; set; }
        public List<Post> TopCommentedPosts { get; set; }
        public List<Post> RecentPosts { get; set; }
        public string ViewOption { get; set; }
    }
}
