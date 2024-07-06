using System.ComponentModel.DataAnnotations;

namespace Telerik_ForumTeamProject.Models.Entities
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }
          
        public int UserID {  get; set; }
        public User User { get; set; }

        public DateTime Created { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Reply> Replies { get;set; }
        public List<Like> Likes { get; set; }
        
    }
}
