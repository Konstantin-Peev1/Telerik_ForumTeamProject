using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Telerik_ForumTeamProject.Data;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories;
using Telerik_ForumTeamProject.Exceptions;

namespace Telerik_ForumTeamProject.Tests.Repositories
{
    [TestClass]
    public class UserRepositoryTests
    {
        private DbContextOptions<ApplicationContext> _options;
        private ApplicationContext _context;
        private UserRepository _userRepository;

        [TestInitialize]
        public void Initialize()
        {
            var databaseName = $"TestDatabase_{Guid.NewGuid()}";
            _options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            _context = new ApplicationContext(_options);
            _userRepository = new UserRepository(_context);

            // Seed the database with initial data
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var users = new List<User>
            {
                new User { ID = 1, UserName = "testuser1", FirstName = "Test", LastName = "User", Email = "test1@example.com", Password = "password1", Role = "User" },
                new User { ID = 2, UserName = "testuser2", FirstName = "Test", LastName = "User", Email = "test2@example.com", Password = "password2", Role = "User" },
                new User { ID = 3, UserName = "testuser3", FirstName = "Test", LastName = "User", Email = "test3@example.com", Password = "password3", Role = "User" }
            };

            _context.Users.AddRange(users);
            _context.SaveChanges();
        }

        [TestMethod]
        public void GetByInformation_ShouldReturnUser_WhenInformationIsValid()
        {
            // Act
            var user = _userRepository.GetByInformation("testuser1");

            // Assert
            Assert.IsNotNull(user);
            Assert.AreEqual("testuser1", user.UserName);
        }

        [TestMethod]
        [ExpectedException(typeof(EntityNotFoundException))]
        public void GetByInformation_ShouldThrowException_WhenInformationIsInvalid()
        {
            // Act
            var user = _userRepository.GetByInformation("invaliduser");
        }

        [TestMethod]
        public void CreateUser_ShouldAddUser()
        {
            // Arrange
            var newUser = new User { UserName = "newuser", FirstName = "New", LastName = "User", Email = "newuser@example.com", Password = "newpassword", Role = "User" };

            // Act
            var createdUser = _userRepository.CreateUser(newUser);

            // Assert
            Assert.IsNotNull(createdUser);
            Assert.AreEqual("newuser", createdUser.UserName);
            Assert.AreEqual(4, _context.Users.Count());
        }

        [TestMethod]
        public void UpdateInformation_ShouldUpdateUser()
        {
            // Arrange
            var userToUpdate = _userRepository.GetUserByID(1);
            var updatedUser = new User { UserName = "updateduser", FirstName = "Updated", LastName = "User", Password = "updatedpassword" };

            // Act
            var result = _userRepository.UpdateInformation(userToUpdate, updatedUser);

            // Assert
            Assert.AreEqual("updateduser", result.UserName);
            Assert.AreEqual("Updated", result.FirstName);
            Assert.AreEqual("User", result.LastName);
            Assert.AreEqual("updatedpassword", result.Password);
        }

        [TestMethod]
        public void UpdateProfilePicture_ShouldUpdateProfilePicture()
        {
            // Arrange
            var user = _userRepository.GetUserByID(1);
            var newProfilePictureUrl = "http://newpicture.com/profile.jpg";

            // Act
            var result = _userRepository.UpdateProfilePicture(user, newProfilePictureUrl);

            // Assert
            Assert.AreEqual(newProfilePictureUrl, result.ProfilePictureUrl);
        }

        [TestMethod]
        public void UserExists_ShouldReturnTrue_WhenUserExists()
        {
            // Act
            var result = _userRepository.UserExists("testuser1");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UserExists_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Act
            var result = _userRepository.UserExists("nonexistentuser");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UserExistsEmail_ShouldReturnTrue_WhenEmailExists()
        {
            // Act
            var result = _userRepository.UserExistsEmail("test1@example.com");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UserExistsEmail_ShouldReturnFalse_WhenEmailDoesNotExist()
        {
            // Act
            var result = _userRepository.UserExistsEmail("nonexistentemail@example.com");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetUserByID_ShouldReturnUser_WhenIdIsValid()
        {
            // Act
            var user = _userRepository.GetUserByID(1);

            // Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(1, user.ID);
        }

        [TestMethod]
        [ExpectedException(typeof(EntityNotFoundException))]
        public void GetUserByID_ShouldThrowException_WhenIdIsInvalid()
        {
            // Act
            var user = _userRepository.GetUserByID(999);
        }

        [TestMethod]
        public void GetUser_ShouldReturnAllUsers()
        {
            // Act
            var users = _userRepository.GetUser();

            // Assert
            Assert.AreEqual(3, users.Count);
        }

        [TestMethod]
        public void SearchUsers_ShouldReturnMatchingUsers()
        {
            // Act
            var users = _userRepository.SearchUsers("testuser1").ToList();

            // Assert
            Assert.AreEqual(1, users.Count);
            Assert.AreEqual("testuser1", users[0].UserName);
        }

        [TestMethod]
        [ExpectedException(typeof(EntityNotFoundException))]
        public void SearchUsers_ShouldThrowException_WhenNoMatchingUsers()
        {
            // Act
            var users = _userRepository.SearchUsers("nonexistentuser").ToList();
        }

        [TestMethod]
        public void BlockUser_ShouldBlockUser()
        {
            // Arrange
            var user = _userRepository.GetUserByID(1);

            // Act
            var result = _userRepository.BlockUser(user);

            // Assert
            Assert.IsTrue(result.IsBlocked);
        }

        [TestMethod]
        public void MakeAdmin_ShouldMakeUserAdmin()
        {
            // Arrange
            var user = _userRepository.GetUserByID(1);

            // Act
            var result = _userRepository.MakeAdmin(user);

            // Assert
            Assert.IsTrue(result.IsAdmin);
            Assert.AreEqual("Admin", result.Role);
        }

        [TestMethod]
        public void MakeUser_ShouldRemoveAdminPrivileges()
        {
            // Arrange
            var user = _userRepository.GetUserByID(1);
            _userRepository.MakeAdmin(user); // First make the user an admin

            // Act
            var result = _userRepository.MakeUser(user);

            // Assert
            Assert.IsFalse(result.IsAdmin);
            Assert.AreEqual("User", result.Role);
        }

        [TestMethod]
        public void UnBlockUser_ShouldUnblockUser()
        {
            // Arrange
            var user = _userRepository.GetUserByID(1);
            _userRepository.BlockUser(user); // First block the user

            // Act
            var result = _userRepository.UnBlockUser(user);

            // Assert
            Assert.IsFalse(result.IsBlocked);
        }
    }
}
