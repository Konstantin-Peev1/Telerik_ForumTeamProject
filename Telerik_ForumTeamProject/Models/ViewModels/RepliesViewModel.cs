namespace Telerik_ForumTeamProject.Models.ViewModels
{
    public class RepliesViewModel
    {
        public int id { get; set; }
        public int? ParentCommentID { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public string Created { get; set; }
    }
}
