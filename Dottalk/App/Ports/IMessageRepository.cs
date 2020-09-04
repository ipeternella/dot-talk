using System;

namespace Dottalk.App.Ports
{
    public interface IMessageRepository
    {
        public void Save(string msg, Guid userId, Guid chatRoomId);
    }
}