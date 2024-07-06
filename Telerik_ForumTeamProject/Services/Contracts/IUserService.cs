using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Services.Contracts
{
    public interface IUserService
    {
        public User GetByInformation(string information, User user);
        public User CreateUser(User user);
        public User UpdateUser(User user, User userToUpdate, int id);

    }
}
