using System.Net;

namespace Telerik_ForumTeamProject.Models.ResponseDTO
{
    public class CommentReplyResponseDTO
    {
        public string Content { get; set; }
        public string UserName {  get; set; }
        public string Created { get; set; }
        public List<CommentReplyResponseDTO> Replies { get; set; }
        }
}
