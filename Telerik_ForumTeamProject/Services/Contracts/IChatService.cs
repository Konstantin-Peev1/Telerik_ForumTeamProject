﻿using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Services.Contracts
{
    public interface IChatService
    {
        List<ChatRoom> GetActiveChats();
        ChatRoom GetChatRoom(int id);
        void AddMessage(int chatRoomId, int userId, string userName, string message);
        void CreateChatRoom(string name); // New method for creating chat rooms

    }
}
