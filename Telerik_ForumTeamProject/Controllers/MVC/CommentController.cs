using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Services;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Controllers.MVC
{
    [Authorize]
    public class CommentController : BaseControllerMVC
    {
        private readonly ICommentService commentService;
        private readonly ModelMapper modelMapper;
        public CommentController(ICommentService commentService, AuthManager authManager, ModelMapper modelMapper) : base(authManager)
        {
            this.commentService = commentService;
            this.modelMapper = modelMapper;
        }

        [HttpGet]
        public IActionResult EditComment(int id)
        {
            var comment = this.commentService.GetComment(id);
            if (comment == null || (comment.UserID != GetCurrentUser().ID && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            var model = new CommentRequestDTO { Content = comment.Content };
            ViewBag.CommentId = id;
            ViewBag.PostId = comment.PostID;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditComment(int id, CommentRequestDTO commentRequest)
        {
            if (commentRequest == null || string.IsNullOrWhiteSpace(commentRequest.Content))
            {
                ViewBag.CommentId = id;
                ViewBag.PostId = this.commentService.GetComment(id).PostID;
                return View(commentRequest);
            }

            var user = GetCurrentUser();
           // var comment = this.commentService.GetComment(id);

            var commentUpdated = this.modelMapper.Map(commentRequest, id);
            var newComment = this.commentService.UpdateComment(id, commentUpdated, user);

            return RedirectToAction("GetPost", "Post", new { id = newComment.PostID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteComment(int commentId)
        {
            var comment = this.commentService.GetComment(commentId);
         
            var postId = comment.PostID;
            this.commentService.DeleteComment(commentId, GetCurrentUser());
            return RedirectToAction("GetPost", "Post", new { id = postId });
        }
    }
}
