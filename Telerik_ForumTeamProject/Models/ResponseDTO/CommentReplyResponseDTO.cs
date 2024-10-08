﻿using System.Net;

namespace Telerik_ForumTeamProject.Models.ResponseDTO
{
    public class CommentReplyResponseDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserName {  get; set; }
        public string Created { get; set; }
        public List<CommentReplyResponseDTO> Replies { get; set; }
        public int? RepliesCount { get; set; }
        }
}
