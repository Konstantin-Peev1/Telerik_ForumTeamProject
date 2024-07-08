using System.Runtime.InteropServices;
using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Services.Contracts
{
    public interface ILikeService
    {
        Like Create(int id, User user);
        Like Delete(int id, User user, Like like);


    }
}
