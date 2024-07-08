using System.ComponentModel.DataAnnotations;

namespace Telerik_ForumTeamProject.Models.Entities
{
    public class Reply : PostAddition
    {    

        public List<Comment> Comments { get; set; }
       
    }
}
