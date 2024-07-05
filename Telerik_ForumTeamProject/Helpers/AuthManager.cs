using System.Text;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services.Contracts;

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

        public User TryGetUser(string credentials)
        {
            string[] credentialsArray = credentials.Split(':');
            string username = credentialsArray[0];
            string password = credentialsArray[1];

            

            try
            {
                var user = this.userRepository.GetByInformation(username); 

                if (user.Password != password)
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

      
    }
}
