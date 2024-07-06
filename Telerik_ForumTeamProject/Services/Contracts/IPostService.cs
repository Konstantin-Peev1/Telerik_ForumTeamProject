using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;

namespace Telerik_ForumTeamProject.Services.Contracts
{
    public interface IPostService
    {
        List<Post> GetTop10Commented();
        List<Post> GetTop10Recent();
        Post GetPost(int id);
        Post CreatePost(Post post, User user);
        List<Post> FilterBy(PostQueryParamteres filterParameters);
        Post UpdatePost(int id, Post updatedPost, User user);
        bool DeletePost(int id, User user);
 
    }
}
