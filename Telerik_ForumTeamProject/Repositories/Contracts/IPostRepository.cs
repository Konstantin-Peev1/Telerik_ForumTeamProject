using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;

namespace Telerik_ForumTeamProject.Repositories.Contracts
{
    public interface IPostRepository
    {
        List<Post> GetTop10Commented();
        List<Post> GetTop10Recent();
        Post GetPost(int id);
        Post CreatePost(Post post);
        List<Post> FilterBy(PostQueryParamteres filterParameters);
        Post UpdatePost(Post post, Post updatedPost);
        bool DeletePost(Post post);
        List<Post> GetAll();








    }
}
