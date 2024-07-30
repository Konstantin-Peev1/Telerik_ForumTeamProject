using Moq;
using System.Collections.Generic;
using System.Linq;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;

namespace Telerik_ForumTeamProject.Test
{
    public class MockUserRepository
    {
        private List<User> sampleUsers;

        public Mock<IUserRepository> GetMockRepository()
        {
            var mockRepository = new Mock<IUserRepository>();

            sampleUsers = new List<User>
            {
                new User
                {
                    ID = 1,
                    UserName = "Kosio",
                    Password = "password",
                    FirstName = "string",
                    LastName = "string",
                    Email = "user1@example.com",
                    IsAdmin = true,
                    Posts = new List<Post>(),
                    Comments = new List<Comment>(),
                },
                new User
                {
                    ID = 2,
                    UserName = "Kosio1",
                    Password = "password1",
                    FirstName = "string1",
                    LastName = "string1",
                    Email = "user2@example.com",
                    IsAdmin = false,
                    Posts = new List<Post>(),
                    Comments = new List<Comment>(),
                },
                new User
                {
                    ID = 3,
                    UserName = "Kosio2",
                    Password = "password2",
                    FirstName = "string2",
                    LastName = "string2",
                    Email = "user3@example.com",
                    IsAdmin = false,
                    Posts = new List<Post>(),
                    Comments = new List<Comment>(),
                }
            };

            // Setup for GetUser
            mockRepository.Setup(x => x.GetUser())
                .Returns(sampleUsers);

            // Setup for GetByInformationUsername
            mockRepository.Setup(x => x.GetByInformationUsername(It.IsAny<string>()))
                .Returns((string username) => sampleUsers.FirstOrDefault(user => user.UserName == username));

            // Setup for GetByInformation (username, email, firstname)
            mockRepository.Setup(x => x.GetByInformation(It.IsAny<string>()))
                .Returns((string info) => sampleUsers.FirstOrDefault(user => user.UserName == info || user.Email == info || user.FirstName == info));

            // Setup for GetUserByID
            mockRepository.Setup(x => x.GetUserByID(It.IsAny<int>()))
                .Returns((int id) => sampleUsers.FirstOrDefault(user => user.ID == id));

            // Setup for UserExists
            mockRepository.Setup(x => x.UserExists(It.IsAny<string>()))
                .Returns((string username) => sampleUsers.Any(user => user.UserName == username));

            // Setup for UserExistsEmail
            mockRepository.Setup(x => x.UserExistsEmail(It.IsAny<string>()))
                .Returns((string email) => sampleUsers.Any(user => user.Email == email));

            // Setup for CreateUser
            mockRepository.Setup(x => x.CreateUser(It.IsAny<User>()))
                .Callback((User user) =>
                {
                    user.ID = sampleUsers.Max(u => u.ID) + 1;
                    sampleUsers.Add(user);
                })
                .Returns((User user) => user);

            // Setup for UpdateInformation
            mockRepository.Setup(x => x.UpdateInformation(It.IsAny<User>(), It.IsAny<User>()))
                .Callback((User user, User updatedUser) =>
                {
                    var userToUpdate = sampleUsers.FirstOrDefault(u => u.ID == user.ID);
                    if (userToUpdate != null)
                    {
                        userToUpdate.FirstName = updatedUser.FirstName;
                        userToUpdate.LastName = updatedUser.LastName;
                        userToUpdate.Email = updatedUser.Email;
                        userToUpdate.Password = updatedUser.Password;
                        // Update other fields as needed
                    }
                })
                .Returns((User user, User updatedUser) => updatedUser);

            // Setup for UpdateProfilePicture
            mockRepository.Setup(x => x.UpdateProfilePicture(It.IsAny<User>(), It.IsAny<string>()))
                .Callback((User user, string profilePictureUrl) =>
                {
                    var userToUpdate = sampleUsers.FirstOrDefault(u => u.ID == user.ID);
                    if (userToUpdate != null)
                    {
                        userToUpdate.ProfilePictureUrl = profilePictureUrl;
                    }
                })
                .Returns((User user, string profilePictureUrl) => user);

            // Setup for BlockUser
            mockRepository.Setup(x => x.BlockUser(It.IsAny<User>()))
                .Callback((User user) =>
                {
                    var userToBlock = sampleUsers.FirstOrDefault(u => u.ID == user.ID);
                    if (userToBlock != null)
                    {
                        userToBlock.IsBlocked = true;
                    }
                })
                .Returns((User user) => user);

            // Setup for UnBlockUser
            mockRepository.Setup(x => x.UnBlockUser(It.IsAny<User>()))
                .Callback((User user) =>
                {
                    var userToUnblock = sampleUsers.FirstOrDefault(u => u.ID == user.ID);
                    if (userToUnblock != null)
                    {
                        userToUnblock.IsBlocked = false;
                    }
                })
                .Returns((User user) => user);

            // Setup for MakeAdmin
            mockRepository.Setup(x => x.MakeAdmin(It.IsAny<User>()))
                .Callback((User user) =>
                {
                    var userToMakeAdmin = sampleUsers.FirstOrDefault(u => u.ID == user.ID);
                    if (userToMakeAdmin != null)
                    {
                        userToMakeAdmin.IsAdmin = true;
                    }
                })
                .Returns((User user) => user);

            mockRepository.Setup(x => x.SearchUsers(It.IsAny<string>()))
               .Returns((string searchTerm) =>
                   sampleUsers.Where(user =>
                       user.UserName.Contains(searchTerm) ||
                       user.FirstName.Contains(searchTerm) ||
                       user.LastName.Contains(searchTerm) ||
                       user.Email.Contains(searchTerm))
                   .AsQueryable());
            // Throws if no such user
            mockRepository
              .Setup(repo => repo.GetUserByID(It.Is<int>(id => sampleUsers.All(u => u.ID != id))))
              .Throws(new EntityNotFoundException("User doesn't exist."));

            // Throws when no user with such username
            mockRepository
              .Setup(repo => repo.GetByInformationUsername(It.Is<string>(username => sampleUsers.All(u => u.UserName != username))))
              .Throws(new EntityNotFoundException("User doesn't exist."));

            return mockRepository;
        }
    }
}
