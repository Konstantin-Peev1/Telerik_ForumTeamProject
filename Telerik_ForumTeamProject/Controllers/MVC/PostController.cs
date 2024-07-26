﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Xml.Linq;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Models.ViewModels;
using Telerik_ForumTeamProject.Services;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Controllers.MVC
{
    [Authorize]
    public class PostController : BaseControllerMVC
    {
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly ModelMapper _modelMapper;
        private readonly AuthManager _authManager;
        private readonly ILikeService _likeService;
        public PostController(IPostService postService, ICommentService commentService, ModelMapper modelMapper, AuthManager authManager, ILikeService likeService) : base(authManager)
        {
            _postService = postService;
            _commentService = commentService;
            _modelMapper = modelMapper;
            _likeService = likeService;
        }

        [HttpPost]
        public IActionResult ToggleLike([FromBody] int postId)
        {
            User user = GetCurrentUser();
            var post = _postService.GetPost(postId);

            if (post == null)
            {
                return Json(new { success = false, message = "Post not found" });
            }

            var existingLike = post.Likes.FirstOrDefault(like => like.UserId == user.ID);

            if (existingLike != null)
            {
                _likeService.Delete(postId, user, existingLike);
            }
            else
            {
                _likeService.Create(postId, user);
            }

            var likeCount = post.Likes.Count;

            return Json(new { success = true, likeCount });
        }

        public IActionResult Index(PostQueryParamteres filterParams, int page = 1, int pageSize = 10)
        {
            User currentUser = GetCurrentUser();
            PagedResult<Post> pagedPosts = _postService.GetPagedPosts(page, pageSize, filterParams);

            var postViewModels = pagedPosts.Items.Select(post => new PostViewModel
            {
                id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Likes = post.Likes?.Count ?? 0,
                PostDate = DateTimeFormatter.FormatToStandard(post.Created),
                Comments = post.Comments?.Select(comment => new CommentReplyResponseDTO
                {
                    Content = comment?.Content ?? "No content",
                }).ToList() ?? new List<CommentReplyResponseDTO>(),
                UserName = post.User.UserName,
                userProfilePictureURL = post.User.ProfilePictureUrl,
                Tags = post.Tags?.Select(tag => tag.Description).ToList() ?? new List<string>(),
                IsCreator = post.UserID == currentUser.ID,
                IsAdmin = currentUser.IsAdmin,
                LastModified = post.LastModified,
            }).ToList();

            var pagedPostViewModel = new PagedPostViewModel
            {
                Posts = postViewModels,
                PaginationMetadata = pagedPosts.Metadata
            };

            return View(pagedPostViewModel);
        }

        [HttpGet]
        public IActionResult GetPost(int id, int? commentId = null)
        {
            var post = _postService.GetPost(id);
            var comments = _commentService.GetAllPostComments(id);

            var viewModel = new PostViewModel
            {
                Title = post.Title,
                Content = post.Content,
                Likes = post.Likes?.Count ?? 0,
                PostDate = DateTimeFormatter.FormatToStandard(post.Created),
                Comments = comments?.Select(comment => new CommentReplyResponseDTO
                {
                    Id = comment.Id,
                    Content = comment?.Content ?? "No content",
                    UserName = comment.User.UserName,
                    Created = DateTimeFormatter.FormatToStandard(comment.Created),
                    Replies = new List<CommentReplyResponseDTO>() 

                }).ToList() ?? new List<CommentReplyResponseDTO>(),
                UserName = post.User.UserName,
                userProfilePictureURL = post.User.ProfilePictureUrl,
                Tags = post.Tags?.Select(tag => tag.Description).ToList() ?? new List<string>(),
                Replies = null
            };

            ViewData["PostId"] = id;

            return View(viewModel);
        }

        public IActionResult GetReplies(int commentId, int page = 1, int pageSize = 5)
        {
            var pagedReplies = _commentService.GetPagedReplies(commentId, page, pageSize);

            var replies = pagedReplies.Items.Select(reply => new RepliesViewModel
            {
                Content = reply.Content,
                UserName = reply.User.UserName,
                Created = DateTimeFormatter.FormatToStandard(reply.Created),
            }).ToList();

            var pagedRepliesViewModel = new PagedRepliesViewModel
            {
                ParentCommentId = commentId,
                Replies = replies,
                Metadata = pagedReplies.Metadata
            };

            return PartialView("_RepliesPartial", pagedRepliesViewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var postModel = new PostRequestDTO();
            return View(postModel);
        }

        [HttpPost]
        
        public IActionResult Create(PostRequestDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = GetCurrentUser();


                try
                {
                    var post = _modelMapper.Map(model);
                    _postService.CreatePost(post, user);
                    return RedirectToAction("Index", "Post");
                }
                catch (AuthorisationExcpetion ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
           

          

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var post = _postService.GetPost(id);
            if (post == null)
            {
                return NotFound();
            }

            var model = new PostRequestDTO
            {
                Title = post.Title,
                Content = post.Content,
                
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, PostRequestDTO model)
        {
            if (ModelState.IsValid)
            {
                User user = GetCurrentUser();
                Post post = _modelMapper.Map(model);
                Post updatedPost = _postService.UpdatePost(id, post, user);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            User user = GetCurrentUser();
            var post = _postService.GetPost(id);
            if (post == null)
            {
                return NotFound();
            }

            _postService.DeletePost(post.Id, user);

            return Ok(new { success = true });
        }
    }
}
