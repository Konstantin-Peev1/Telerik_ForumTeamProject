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
        bool UserExistsEmail(string email);
        User GetUserByID(int id);
        User BlockUser(User user);
        User MakeAdmin(User user);
        User UnBlockUser(User user);
        User GetByInformationUsername(string information);


    }
}
