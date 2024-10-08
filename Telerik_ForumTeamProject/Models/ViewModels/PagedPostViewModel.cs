﻿using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Models.ResponseDTO;

namespace Telerik_ForumTeamProject.Models.ViewModels
{
    public class PagedPostViewModel
    {
        public List<PostViewModel> Posts { get; set; }
        public PaginationMetadata PaginationMetadata { get; set; }
        public PostQueryParamteres FilterParameters { get; set; }
    }
}
