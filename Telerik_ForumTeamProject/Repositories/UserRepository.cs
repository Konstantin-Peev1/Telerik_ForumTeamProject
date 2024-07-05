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

        public User CreateUser(User user)
        {
            this.applicationConetxt.Add(user);
            this.applicationConetxt.SaveChanges();
            return user;
        }
        public User UpdateInformation(User user, User userToUpdate)
        {
            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.Password = user.Password;
            userToUpdate.UserName = user.UserName;
            this.applicationConetxt.SaveChanges();
            return userToUpdate;
        }

        public bool UserExists(string username)
        {
            return this.applicationConetxt.Users.Any(user => user.UserName == username);
        }
        public bool UserExistsEmail(string email)
        {
            return this.applicationConetxt.Users.Any(user => user.Email == email);

        }

        private IQueryable<User> GetUser()
        {
            return this.applicationConetxt.Users;
                

        }

         

   
    }
}
