using System;
using Dottalk.App.DTOs;

namespace Dottalk.App.Ports
{
    // Summary:
    //   Defines the main usecases of the chat application that can be used.
    public interface IChatRoomService
    {
        public VoidServiceResult ConnectToChatRoom(Guid userId, Guid chatRoomId);
        public VoidServiceResult DisconnectFromChatRoom(Guid userId, Guid chatRoomId);
        public VoidServiceResult SendMessageToChatRoom(string msg, Guid userId, Guid chatRoomId);
        public VoidServiceResult SendMessageToChatRoomUser(string msg, Guid userId, Guid chatRoomId, Guid targetUserId);
    }
}