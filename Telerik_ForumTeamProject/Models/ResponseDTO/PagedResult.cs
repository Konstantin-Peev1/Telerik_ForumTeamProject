namespace Telerik_ForumTeamProject.Models.ResponseDTO
{
    public class PagedResult<T>
    {
        public ICollection<T> Items { get; set; }
        public PaginationMetadata Metadata { get; set; }
    }
}
