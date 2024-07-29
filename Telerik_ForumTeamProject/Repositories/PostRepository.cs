using Microsoft.EntityFrameworkCore;
using Telerik_ForumTeamProject.Data;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Repositories.Contracts;

namespace Telerik_ForumTeamProject.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationContext applicationContext;
        public PostRepository(ApplicationContext applicationContext)
        {
            this.applicationContext = applicationContext;
        }

        public ICollection<Post> GetTop10Commented()
        {
            return GetPosts()
           .OrderByDescending(x => x.Comments.Count)
           .Take(10)
           .ToList();
        }

        public ICollection<Post> GetTop10Recent()
        {
            return GetPosts()
            .OrderByDescending(x => x.Created)
            .Take(10)
            .ToList();
        }

        public Post GetPost(int id)
        {
            return GetPosts().FirstOrDefault(x => x.Id == id) ?? throw new EntityNotFoundException("No such post exists in our system!");
        }

        public Post CreatePost(Post post)
        {
            this.applicationContext.Add(post);
            post.Tags = new List<Tag>();
            this.applicationContext.SaveChanges();
            return post;
        }
        public ICollection<Post> FilterBy(PostQueryParamteres filterParameters)
        {
            IQueryable<Post> results = this.GetPosts();
            results = FilterByTitle(results, filterParameters.Title);
            results = FilterByUser(results, filterParameters.UserName);
            results = FilterByTag(results, filterParameters.Tag);
            results = FilterByMinLikes(results, filterParameters.MinLikes);
            results = FilterByMaxLikes(results, filterParameters.MaxLikes);
            results = SortBy(results, filterParameters.SortBy);
            var resultsFixed = results.ToList();
            resultsFixed = Order(resultsFixed, filterParameters.SortOrder).ToList();


            return resultsFixed;

        }

        public ICollection<Post> FilterBy(int page, int pageSize, PostQueryParamteres filterParameters)
        {
            IQueryable<Post> results = this.GetPosts();
            results = FilterByTitle(results, filterParameters.Title);
            results = FilterByUser(results, filterParameters.UserName);
            results = FilterByTag(results, filterParameters.Tag);
            results = FilterByMinLikes(results, filterParameters.MinLikes);
            results = FilterByMaxLikes(results, filterParameters.MaxLikes);
            results = SortBy(results, filterParameters.SortBy);
            var resultsFixed = results.ToList();
            resultsFixed = Order(resultsFixed, filterParameters.SortOrder).ToList();
            

            return GetPagedPosts(page, pageSize, resultsFixed);

        }

        public Post UpdatePost(Post post, Post updatedPost)
        {
            post.Title = updatedPost.Title;
            post.Content = updatedPost.Content;
            post.LastModified = DateTime.UtcNow;
            // post.Tags = updatedPost.Tags;
            this.applicationContext.SaveChanges();
            return post;
        }


        private ICollection<Post> GetPagedPosts(int page, int pageSize, ICollection<Post> filteredPosts)
        {
            int skip = (page - 1) * pageSize;
            //var filteredPosts = GetPosts();
            return filteredPosts
                
                .Skip(skip)
                .Take(pageSize)
                .ToList();
        }

        public int GetPostsCount()
        {
            return applicationContext.Posts.Count();
        }
        public bool DeletePost(Post post)
        {
            this.applicationContext.Posts.Remove(post);
            return this.applicationContext.SaveChanges() > 0;
        }

        private static IQueryable<Post> FilterByTitle(IQueryable<Post> posts, string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                return posts.Where(post => post.Title.Contains(title));
            }
            return posts;
        }

        private static IQueryable<Post> FilterByMinLikes(IQueryable<Post> posts, int? minLikes)
        {

            if (minLikes.HasValue && minLikes.Value > 0)
            {
                return posts.Where(post => post.Likes.Count >= minLikes.Value);
            }
            return posts;
        }
        private static IQueryable<Post> FilterByMaxLikes(IQueryable<Post> posts, int? maxLikes)
        {
            if (maxLikes.HasValue && maxLikes.Value > 0)
            {
                return posts.Where(post => post.Likes.Count <= maxLikes.Value);
            }
            return posts;
        }
        private static IQueryable<Post> FilterByUser(IQueryable<Post> posts, string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                return posts.Where(post => post.User.UserName.Contains(username));
            }
            return posts;
        }

        private static IQueryable<Post> FilterByTag(IQueryable<Post> posts, string tagdesc)
        {
            if (!string.IsNullOrEmpty(tagdesc))
            {
                return posts
                    .Where(post => post.Tags.Any(tag => tag.Description.Contains(tagdesc)));
            }
            return posts;
        }

        private static IQueryable<Post> SortBy(IQueryable<Post> posts, string sortCriteria)
        {
            switch (sortCriteria)
            {
                case "Title":
                    return posts.OrderBy(posts => posts.Title);
                case "Likes":
                    return posts.OrderBy(posts => posts.Likes.Count);
                case "User":
                    return posts.OrderBy(posts => posts.User.UserName);
                case "Tag":
                    return posts.OrderBy(posts => posts.Tags.OrderBy(tag => tag.Description));
                default:
                    return posts.OrderByDescending(x => x.Created);
            }
        }

        private static IEnumerable<Post> Order(IList<Post> posts, string sortOrder)
        {
            return (sortOrder == "desc") ? posts.Reverse() : posts;
        }

        private IQueryable<Post> GetPosts()
        {
            return this.applicationContext.Posts
                .Include(post => post.User)
                .Include(post => post.Tags)
                //.Include(post => post.Replies)
                //.ThenInclude(reply => reply.User)
                .Include(post => post.Comments)
                    .ThenInclude(comment => comment.User)
                .Include(post => post.Likes)
                    .ThenInclude(likes => likes.User);

        }
    }
}
