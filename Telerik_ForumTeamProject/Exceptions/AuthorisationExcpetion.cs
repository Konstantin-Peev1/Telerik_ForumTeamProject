namespace Telerik_ForumTeamProject.Exceptions
{
    public class AuthorisationExcpetion : ApplicationException
    {
        public AuthorisationExcpetion(string message)
           : base(message)
        {
        }
    }
}
