using Telerik_ForumTeamProject.Models.ResponseDTO;

namespace Telerik_ForumTeamProject.ViewModels
{
    public class HomeViewModel
    {
        public List<PostUploadResponseDTO> LatestPosts { get; set; }
        public List<PostUploadResponseDTO> MostCommentedPosts { get; set; }
    }
}
