using System;

namespace Dottalk.App.Ports
{
    // Summary
    //   Interface for persisting/reading chat rooms and chat messages sent from users.
    public interface IChatRoomRepository
    {
        public void Save(string msg, Guid userId, Guid chatRoomId);
    }
}