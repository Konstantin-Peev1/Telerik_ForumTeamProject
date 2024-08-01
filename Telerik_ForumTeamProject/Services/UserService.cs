using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.IIS.Core;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly string defaultProfilePictureUrl;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            this.userRepository = userRepository;
            this.defaultProfilePictureUrl = configuration["CloudinarySettings:DefaultProfilePictureUrl"];
        }

        public User GetByInformation(string information)
        {

            return userRepository.GetByInformation(information);
        }
        public User GetByInformationUsername(string information)
        {
            return userRepository.GetByInformationUsername(information);
        }

        public User GetUserById(int id) 
        {
            return userRepository.GetUserByID(id);
        }
        public IEnumerable<User> SearchUsers(string searchTerm)
        {
            return this.userRepository.SearchUsers(searchTerm).ToList();
        }

        public User CreateUser(User user)
        {
            if(this.userRepository.UserExists(user.UserName) || this.userRepository.UserExistsEmail(user.Email))
            {
                throw new DuplicateEntityException("User with such username or email exists");
            }
            
            if (string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                user.ProfilePictureUrl = this.defaultProfilePictureUrl;
            }

            User createdUser = this.userRepository.CreateUser(user);
            return createdUser;
        }
        public User UpdateUser(User user, User userToUpdate, int id)
        {
            var validateUser = this.userRepository.GetUserByID(id);
            if(user.ID != validateUser.ID)
            {
                throw new AuthorisationExcpetion("You can't edit other users!");
            }
            if(user.UserName != userToUpdate.UserName && this.userRepository.UserExists(userToUpdate.UserName))
            {
                throw new DuplicateEntityException("User with such username exists");
            }
            return this.userRepository.UpdateInformation(user, userToUpdate);
        }

        public User UpdateProfilePicture(int userId, string profilePictureUrl)
        {
            User userToUpdate = this.userRepository.GetUserByID(userId);

            if(userToUpdate == null)
            {
                throw new EntityNotFoundException("Such user does not exist");
            }
            return this.userRepository.UpdateProfilePicture(userToUpdate, profilePictureUrl);

        }

        public bool UserExists(string username)
        {
            return this.userRepository.UserExists(username);
        }
        public bool UserExistsEmail(string email)
        {
            return this.userRepository.UserExistsEmail(email);

        }

        public User BlockUser(User user)
        {
            if (user.IsBlocked)
            {
                throw new DuplicateEntityException("User already blocked");
            }
            if(user.IsAdmin)
            {
                throw new AuthorisationExcpetion("You can't block admins");
            }
            return this.userRepository.BlockUser(user);
        }

        public User UnBlockUser(User user)
        {
            if (!user.IsBlocked)
            {
                throw new DuplicateEntityException("User is not blocked");
            }
            return this.userRepository.UnBlockUser(user);
        }

        public User MakeAdmin(User user)
        {
            if (user.IsAdmin)
            {
                throw new DuplicateEntityException("User already admin");
            }
            return this.userRepository.MakeAdmin(user);
        }

        public User MakeUser(User user)
        {
            return this.userRepository.MakeUser(user);
        }
    }
}
