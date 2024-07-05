namespace Telerik_ForumTeamProject.Exceptions
{
    public class EntityNotFoundException : ApplicationException
    {
        public EntityNotFoundException(string message)
          : base(message)
        {
        }
    }
}
