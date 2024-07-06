using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Models.ResponseDTO;

namespace Telerik_ForumTeamProject.Helpers
{
    public class ModelMapper
    {
        public UserResponseDTO Map(User user)
        {
            UserResponseDTO response = new UserResponseDTO();
            response.FirstName = user.FirstName;
            response.LastName = user.LastName;
            response.Email = user.Email;     
            if(user.Posts!= null) { response.Posts = Map(user.Posts); }
            return response;
        }

        public User Map(UserRequestDTO userRequest)
        {
            return new User()
            {
                FirstName = userRequest.FirstName,
                LastName = userRequest.LastName,
                Email = userRequest.Email,
                Password = userRequest.Password,
                UserName = userRequest.UserName,
            };
        }
        public List<PostResponseDTO> Map(List<Post> posts)
        {
            if(posts.Count !=  0) 
            {
                return posts.Select(post => new PostResponseDTO
                {
                    Title = post.Title
                }).ToList();
            }
            return new List<PostResponseDTO>();

          
        }
    }

   
    
}
