using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Services.Contracts
{
    public interface IUserService
    {
        User GetByInformation(string information);
        User CreateUser(User user);
        User UpdateUser(User user, User userToUpdate, int id);
        User UpdateProfilePicture(int userId, string profilePictureUrl);
        User BlockUser(User user);
        User UnBlockUser(User user);
        User MakeAdmin(User user);  

    }
}
