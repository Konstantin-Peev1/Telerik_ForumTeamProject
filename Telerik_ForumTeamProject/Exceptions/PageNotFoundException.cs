namespace Telerik_ForumTeamProject.Exceptions
{
    public class PageNotFoundException : ApplicationException
    {
        public PageNotFoundException(string message) 
            : base(message)
        {
            
        }
    }
}
