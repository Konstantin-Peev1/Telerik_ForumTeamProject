using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Repositories.Contracts;

namespace Telerik_ForumTeamProject.Helpers
{
    public class AuthManager
    {
        private const string InvalidCredentialsErrorMessage = "Invalid credentials!";
        private readonly IUserRepository userRepository;
        private readonly IConfiguration configuration;
        public AuthManager(IUserRepository userRepository, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.userRepository = userRepository;
        }

        public User Authenticate(string username, string password)
        {
            var user = userRepository.GetByInformationUsername(username);

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
                var user = userRepository.GetByInformationUsername(username);

                // Check if user exists and verify password
                if (user == null || password != user.Password)
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

        public string Generate(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Hash, user.Password),
                new Claim("isAdmin", user.IsAdmin.ToString())
            };

            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"], configuration["Jwt:Audience"],
                claims, 
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



   
    }
}