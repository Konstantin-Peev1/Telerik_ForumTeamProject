using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Models.ResponseDTO
{
    public class TagResponseDTO
    {
        public string Description { get; set; } 
        public List<PostUploadResponseDTO> Posts { get; set; } 
    }
}
