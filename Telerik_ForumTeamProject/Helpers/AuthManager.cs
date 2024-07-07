using BCrypt.Net;
using System;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;

namespace Telerik_ForumTeamProject.Helpers
{
    public class AuthManager
    {
        private const string InvalidCredentialsErrorMessage = "Invalid credentials!";
        private readonly IUserRepository userRepository;

        public AuthManager(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public User Authenticate(string username, string password)
        {
            var user = userRepository.GetByInformation(username);

            // Check if user exists and verify password
            if (user == null || !VerifyPassword(password, user.Password))
            {
                throw new AuthorisationExcpetion(InvalidCredentialsErrorMessage);
            }

            return user;
        }

        public User TryGetUser(string credentials)
        {
            string[] credentialsArray = credentials.Split(':');
            string username = credentialsArray[0];
            string password = credentialsArray[1];

            try
            {
                var user = userRepository.GetByInformation(username);

                // Check if user exists and verify password
                if (user == null || !VerifyPassword(password, user.Password))
                {
                    throw new AuthorisationExcpetion(InvalidCredentialsErrorMessage);
                }

                return user;
            }
            catch (EntityNotFoundException)
            {
                throw new AuthorisationExcpetion(InvalidCredentialsErrorMessage);
            }
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}