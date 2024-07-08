using Microsoft.Extensions.Hosting;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository likeRepository;
        private readonly IPostService postService;

        public LikeService(ILikeRepository likeRepository, IPostService postService)
        {
            this.likeRepository = likeRepository;
            this.postService = postService;
        }

        public Like Create(int id, User user)
        {
            Post post = postService.GetPost(id);
            if (post.Likes.Any(like => like.UserId == user.ID))
            {
                Like likeToRemove = post.Likes.FirstOrDefault(like => like.UserId == user.ID);
                
                return Delete(id, user, likeToRemove); ;
            }
            Like like = new Like()
            {
                UserId = user.ID,
                PostID = post.Id,
            };
            return this.likeRepository.Create(user, post, like);

        }

        public Like Delete(int id, User user, Like like)
        {
            Post post = this.postService.GetPost(id);
            return this.likeRepository.Remove(user, post, like);
        }
    }
}
