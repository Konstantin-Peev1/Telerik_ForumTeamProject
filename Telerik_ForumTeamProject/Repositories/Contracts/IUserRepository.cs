using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Repositories.Contracts
{
    public interface IUserRepository
    {
        User GetByInformation(string information);
        User UpdateInformation(User user, User userToUpdate);
        User CreateUser (User user);
        bool UserExists(string username);
        public bool UserExistsEmail(string email);
        public User GetUserByID(int id);


    }
}
