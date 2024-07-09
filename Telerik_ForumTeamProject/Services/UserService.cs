using Microsoft.AspNetCore.Server.IIS.Core;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public User GetByInformation(string information)
        {

            return userRepository.GetByInformation(information);
        }

        public User CreateUser(User user)
        {
            if(this.userRepository.UserExists(user.UserName) || this.userRepository.UserExistsEmail(user.Email))
            {
                throw new DuplicateEntityException("User with such username or email exists");
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
    }
}
