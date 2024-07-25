using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Models.ResponseDTO;

namespace Telerik_ForumTeamProject.Services.Contracts
{
    public interface IPostService
    {
        ICollection<Post> GetTop10Commented();
        ICollection<Post> GetTop10Recent();
        Post GetPost(int id);
        Post CreatePost(Post post, User user);
        ICollection<Post> FilterBy(PostQueryParamteres filterParameters);
        Post UpdatePost(int id, Post updatedPost, User user);
        bool DeletePost(int id, User user);
        PagedResult<Post> GetPagedPosts(int page, int pageSize, PostQueryParamteres filterParams);
    }
}
