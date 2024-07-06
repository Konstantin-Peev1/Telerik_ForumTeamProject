using System.Reflection.Metadata;
using Telerik_ForumTeamProject.Data;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;

namespace Telerik_ForumTeamProject.Repositories
{
    public class ReplyRepository : IReplyRepository
    {
        private readonly ApplicationContext applicationContext;
        public ReplyRepository(ApplicationContext applicationContext)
        {
            this.applicationContext = applicationContext;
        }

        public List<Reply> GetAllReplies()
        {
            return GetReplies().ToList();
        }

        public List<Reply> GetRepliesByUser(int userId)
        {
            return GetReplies().Where(r => r.UserID == userId).ToList();
        }

        public Reply GetReplyById(int id)
        {
            Reply reply = GetReplies().FirstOrDefault(x => x.ID == id);

            return reply ?? throw new EntityNotFoundException("Reply not found.");
        }

        public Reply CreateReply(Reply reply)
        {
            applicationContext.Replies.Add(reply);
            applicationContext.SaveChanges();
            return reply;
        }

        public Reply UpdateReply(int id, Reply reply)
        {
            Reply replyToUpdate = this.GetReplyById(id);

            replyToUpdate.Description = reply.Description;
            this.applicationContext.SaveChanges();
            return replyToUpdate;
        }

        private IQueryable<Reply> GetReplies()
        {
            return this.applicationContext.Replies;
        }
    }
}
