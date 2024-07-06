namespace Telerik_ForumTeamProject.Models.ResponseDTO
{
    public class UserResponseDTO
    {
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }   
        public List<PostResponseDTO>? Posts { get; set; }
    }
}
