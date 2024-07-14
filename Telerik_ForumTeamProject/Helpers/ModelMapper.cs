using Microsoft.Extensions.Hosting;
using System.Xml.Linq;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.AspNetCore.SignalR;
namespace Telerik_ForumTeamProject.Helpers
{
    public class ModelMapper
    {
        public UserResponseDTO Map(User user)
        {
            UserResponseDTO response = new UserResponseDTO();
            response.FirstName = user.FirstName;
            response.LastName = user.LastName;
            response.Email = user.Email;
            if (user.Posts != null) { response.Posts = Map(user.Posts); }
            return response;
        }

        public User Map(UserRequestDTO userRequest)
        {
            return new User()
            {
                FirstName = userRequest.FirstName,
                LastName = userRequest.LastName,
                Email = userRequest.Email,
                Password = userRequest.Password,
                UserName = userRequest.UserName,
                Role = "User",
            };
        }

        public LikeResponseDTO Map(Like like)
        {
            return new LikeResponseDTO()
            {
                UserName = like.User.UserName,
            };
        }

        public Post Map(PostRequestDTO postRequestDTO)
        {
            return new Post()
            {
                Title = postRequestDTO.Title,
                Content = postRequestDTO.Content,
                Created = DateTime.Now,
            };
        }

        public TagResponseDTO Map(Tag tag)
        {
            return new TagResponseDTO()
            {
                Description = tag.Description,
                Posts = tag.Posts.Select(post => Map(post)).ToList(),
            };
        }



        public PostUploadResponseDTO Map(Post post)
        {
            PostUploadResponseDTO postResponse = new PostUploadResponseDTO();

            postResponse.Title = post.Title;
            postResponse.PostDate = DateTimeFormatter.FormatToStandard(post.Created);
            postResponse.Content = post.Content;
            postResponse.UserName = post.User.UserName;
            postResponse.Likes = post.Likes?.Count() ?? 0;
            postResponse.Comments = Map(post.Comments) ?? new List<CommentReplyResponseDTO>();
            //postResponse.Replies = Map(post.Replies) ?? new List<CommentReplyResponseDTO>();
            postResponse.Tags = post.Tags?.Select(tag => tag.Description).ToList() ?? new List<string>();
            postResponse.DateCreated = post.Created;
            postResponse.Id = post.Id;
            return postResponse;
            /*return new PostUploadResponseDTO()
            {
                Title = post.Title,
                PostDate = post.Created,
                Content = post.Content,
                UserName = post.User.UserName,
                Likes = post.Likes?.Count() ?? 0,
                Comments = Map(post.Comments) ?? new List<CommentReplyResponseDTO>(),
                Replies = Map(post.Replies) ?? new List<CommentReplyResponseDTO>(),
                Tags = post.Tags.Select(tag => tag.Description).ToList() ?? new List<string>(),
            };*/
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
                            }).ToList() ?? new List<CommentReplyResponseDTO>();
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

        public Comment Map(CommentRequestDTO comment, int postId)
        {
            return new Comment()
            {
                PostID = postId,
                Content = comment.Content,
                Created = DateTime.Now
            };
        }

        public Comment MapCreateReply(CommentRequestDTO comment)
        {
            return new Comment()
            {
                
                Content = comment.Content,
                Created = DateTime.Now
            };
        }

        public Comment MapUpdateComment(CommentRequestDTO comment, int commentId)
        {
            return new Comment()
            {
                Id = commentId,
                Content = comment.Content,
                Created = DateTime.Now
            };
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
            if (posts.Count != 0)
            {
                return posts.Select(post => new PostResponseDTO
                {
                    Title = post.Title
                }).ToList();
            }
            return new List<PostResponseDTO>();
        }

    }   
}
