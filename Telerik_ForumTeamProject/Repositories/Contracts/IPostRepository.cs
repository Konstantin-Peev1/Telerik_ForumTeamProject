using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;

namespace Telerik_ForumTeamProject.Repositories.Contracts
{
    public interface IPostRepository
    {
        ICollection<Post> GetTop10Commented();
        ICollection<Post> GetTop10Recent();
        Post GetPost(int id);
        Post CreatePost(Post post);
        ICollection<Post> FilterBy(PostQueryParamteres filterParameters);
        Post UpdatePost(Post post, Post updatedPost);
        bool DeletePost(Post post);

        ICollection<Post> GetPagedPosts(int page, int pageSize);
        int GetPostsCount();






    }
}
