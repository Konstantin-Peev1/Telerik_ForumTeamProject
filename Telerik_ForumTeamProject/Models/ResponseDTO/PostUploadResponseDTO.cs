using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Models.ResponseDTO
{
    public class PostUploadResponseDTO
    {
        public string Title { get; set; }
        public string Content { get; set; } 
        public int Likes { get; set; }
        public DateTime PostDate { get; set; }  
        public List<CommentReplyResponseDTO> Comments { get; set; }
        public List<CommentReplyResponseDTO> Replies { get; set; }
        public string UserName { get; set; }

        //maybe tags

    }
}
