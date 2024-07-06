using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Services.Contracts
{
    public interface IUserService
    {
        User GetByInformation(string information, User user);
        User CreateUser(User user);
        User UpdateUser(User user, User userToUpdate, int id);

    }
}
