using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Telerik_ForumTeamProject.Data;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;

namespace Telerik_ForumTeamProject.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext applicationConetxt;

        public UserRepository(ApplicationContext applicationConetxt)
        {
            this.applicationConetxt = applicationConetxt;
        }

        public User GetByInformation(string information)
        {
            return this.GetUser()
                .FirstOrDefault(x => x.FirstName == information 
                || x.UserName == information 
                || x.Email == information) 
                ?? throw new EntityNotFoundException("No user with such information found");
        }
        public User GetByInformationUsername(string information)
        {
            return this.GetUser()
                .FirstOrDefault(x => 
                x.UserName == information
                || x.Email == information)
                ?? throw new EntityNotFoundException("No user with such information found");
        }

        public User CreateUser(User user)
        {
            this.applicationConetxt.Add(user);
            this.applicationConetxt.SaveChanges();
            return user;
        }
        public User UpdateInformation(User user, User userToUpdate)
        {
            user.FirstName = userToUpdate.FirstName;
            user.LastName = userToUpdate.LastName;
            user.Password = userToUpdate.Password;
            user.UserName = userToUpdate.UserName;
            this.applicationConetxt.SaveChanges();
            return user;
        }

        public User UpdateProfilePicture(User user, string profilePictureUrl)
        {
            user.ProfilePictureUrl = profilePictureUrl;
            this.applicationConetxt.SaveChanges();
            return user;
        }

        public bool UserExists(string username)
        {
            return GetUser().Any(user => user.UserName == username);
        }
        public bool UserExistsEmail(string email)
        {
            return GetUser().Any(user => user.Email == email);

        }

        public User GetUserByID(int id)
        {
            return GetUser().FirstOrDefault(u => u.ID == id) ?? throw new EntityNotFoundException("No such user");
        }

        public IList<User> GetUser()
        {
            return this.applicationConetxt.Users
                .Include(x => x.Posts)
                .Include(x => x.Comments)
                .Include(x => x.ChatRooms)
                .ToList();
                
        }

        public IQueryable<User> SearchUsers(string searchTerm)
        {

            var users = applicationConetxt.Users
                .Where(u => u.UserName.Contains(searchTerm) || u.Email.Contains(searchTerm));

            if (!users.Any())
            {
                throw new EntityNotFoundException($"No user with '{searchTerm}' has been found!");
            }

            return users;

        }

        public User BlockUser(User user)
        {
            user.IsBlocked = true;
            this.applicationConetxt.SaveChanges();
            return user;
        }

        public User MakeAdmin(User user)
        {
            user.IsAdmin = true;
            user.Role = "Admin";
            this.applicationConetxt.SaveChanges();
            return user;
        }

        public User MakeUser(User user)
        {
            user.IsAdmin = false;
            user.Role = "User";
            this.applicationConetxt.SaveChanges();
            return user;
        }

        public User UnBlockUser(User user)
        {
            user.IsBlocked = false;
            this.applicationConetxt.SaveChanges();
            return user;
        }

      
    }
}
