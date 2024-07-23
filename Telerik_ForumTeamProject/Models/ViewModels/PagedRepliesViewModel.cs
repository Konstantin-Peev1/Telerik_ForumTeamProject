using Telerik_ForumTeamProject.Models.ResponseDTO;

namespace Telerik_ForumTeamProject.Models.ViewModels
{
    public class PagedRepliesViewModel
    {
        public int ParentCommentId { get; set; }    
        public List<RepliesViewModel> Replies { get; set; }
        public PaginationMetadata Metadata { get; set; }
    }
}
