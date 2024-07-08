using Telerik_ForumTeamProject.Data;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;

namespace Telerik_ForumTeamProject.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly ApplicationContext applicationContext;

        public LikeRepository(ApplicationContext applicationContext)
        {
            this.applicationContext = applicationContext;
        }
        public Like Create(User user, Post post, Like like)
        {
            user.Likes ??= new List<Like>();
            post.Likes ??= new List<Like>();

            user.Likes.Add(like);
            post.Likes.Add(like);
            applicationContext.SaveChanges();
            return like;
        }

        public Like Remove(User user, Post post, Like like)
        {
            user.Likes.Remove(like);
            post.Likes.Remove(like);
            return like;
        }
    }
}
