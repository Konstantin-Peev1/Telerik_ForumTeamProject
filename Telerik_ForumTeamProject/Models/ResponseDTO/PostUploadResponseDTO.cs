using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Models.ResponseDTO
{
    public class PostUploadResponseDTO
    {
        public string Title { get; set; }
        public string Content { get; set; } 
        public int Likes { get; set; }
        public string PostDate { get; set; }  
        public List<CommentReplyResponseDTO> Comments { get; set; }
        public string UserName { get; set; }
        public List<string> Tags { get; set; }

        //maybe tags

    }
}
