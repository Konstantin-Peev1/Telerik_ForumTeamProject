using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Telerik_ForumTeamProject.Models.Entities
{
    public class Comment : PostAddition
    {
        

        public int? ReplyID { get; set; }
        public Reply Reply { get; set; }    
      
    }
}
