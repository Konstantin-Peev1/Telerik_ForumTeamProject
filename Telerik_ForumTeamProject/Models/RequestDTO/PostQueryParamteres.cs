using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Models.RequestDTO
{
    public class PostQueryParamteres
    {
        public string? Title { get; set; }
        public string? Tag { get; set; }
        public string? UserName { get; set; }
        public int MaxLikes { get; set; }
        public int MinLikes { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
    }
}
