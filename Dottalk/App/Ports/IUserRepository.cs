using System;

namespace Dottalk.App.Ports
{
    // Summary
    //   Interface for persisting/reading user info.
    public interface IUserRepository
    {
        public void Save(string msg, Guid userId, Guid chatRoomId);
    }
}