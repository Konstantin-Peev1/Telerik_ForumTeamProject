using Microsoft.Extensions.Hosting;
using System.Xml.Linq;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Telerik_ForumTeamProject.Helpers
{
    public class ModelMapper
    {
        public UserResponseDTO Map(User user)
        {
            UserResponseDTO response = new UserResponseDTO()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Posts = user.Posts != null ? Map(user.Posts) : new List<PostResponseDTO>()
            };
            return response;
        }

        public User Map(UserRequestDTO userRequest)
        {
            User request = new User()
            {
                FirstName = userRequest.FirstName,
                LastName = userRequest.LastName,
                Email = userRequest.Email,
                Password = userRequest.Password,
                UserName = userRequest.UserName,
                Role = "User",
            };
            return request;
        }

        public LikeResponseDTO Map(Like like)
        {
            LikeResponseDTO response = new LikeResponseDTO()
            {
                UserName = like.User.UserName
            };
            return response;
        }

        public Post Map(PostRequestDTO postRequestDTO)
        {
            Post request = new Post()
            {
                Title = postRequestDTO.Title,
                Content = postRequestDTO.Content,
                Created = DateTime.Now,
            };
            return request;
        }

        public PostUploadResponseDTO Map(Post post)
        {
            PostUploadResponseDTO response = new PostUploadResponseDTO()
            {
                Title = post.Title,
                PostDate = DateTimeFormatter.FormatToStandard(post.Created),
                Content = post.Content,
                UserName = post.User.UserName,
                Likes = post.Likes?.Count() ?? 0,
                Comments = Map(post.Comments) ?? new List<CommentReplyResponseDTO>(),
                Tags = post.Tags?.Select(tag => tag.Description).ToList() ?? new List<string>()
            };
            return response;
        }

        public TagResponseDTO Map(Tag tag)
        {
            TagResponseDTO response = new TagResponseDTO()
            {
                Description = tag.Description,
                Posts = tag.Posts.Select(post => Map(post)).ToList(),
            };
            return response;
        }

        public List<CommentReplyResponseDTO> Map(List<Comment> comments)
        {
            return comments?.Where(c => c.ParentCommentID == null)
                            .Select(c => new CommentReplyResponseDTO
                            {
                                Content = c.Content,
                                Created = DateTimeFormatter.FormatToStandard(c.Created),
                                UserName = c.User.UserName,
                                Replies = new List<CommentReplyResponseDTO>() // Empty list to indicate no replies
                            })
                            .ToList() ?? new List<CommentReplyResponseDTO>();
        }

        public List<ReplyResponseDTO> MapReplyResponse(List<Comment> reply)
        {
            return reply?.Select(c => new ReplyResponseDTO 
            {
                Content = c.Content,
                Created = DateTimeFormatter.FormatToStandard(c.Created),
                UserName = c.User.UserName,
            }).ToList() ?? new List<ReplyResponseDTO>();
        }

        private CommentReplyResponseDTO MapCommentWithReplies(Comment comment)
        {
            var dto = new CommentReplyResponseDTO
            {
                Content = comment.Content,
                Created = DateTimeFormatter.FormatToStandard(comment.Created),
                UserName = comment.User.UserName,
                Replies = comment.Replies?.Select(r => MapCommentWithReplies(r)).ToList() ?? new List<CommentReplyResponseDTO>()
            };
            return dto;
        }

        public Comment Map(CommentRequestDTO comment, int postId = 0, int commentId = 0)
        {
            // Map for Creating a new comment
            if (postId != 0 && commentId == 0) 
            {
                return new Comment()
                {
                    PostID = postId,
                    Content = comment.Content,
                    Created = DateTime.Now
                };
            }
            // Map for Updating a comment
            else if (postId == 0 && commentId != 0) 
            {
                return new Comment()
                {
                    Id = commentId,
                    Content = comment.Content,
                    Created = DateTime.Now
                };
            }
            // Map for creating a reply
            else
            {
                return new Comment() 
                {
                    Content = comment.Content,
                    Created = DateTime.Now
                };
            }
        }

        public CommentReplyResponseDTO Map(Comment comment)
        {
            return new CommentReplyResponseDTO
            {
                Content = comment.Content,
                Created = DateTimeFormatter.FormatToStandard(comment.Created),
                UserName = comment.User.UserName
            };
        }

        public List<PostResponseDTO> Map(List<Post> posts)
        {
            List<PostResponseDTO> response;
            response = posts.Count != 0 ? posts
                            .Select(p => new PostResponseDTO { Title = p.Title })
                            .ToList() : new List<PostResponseDTO>();
            return response;
        }
    }   
}
