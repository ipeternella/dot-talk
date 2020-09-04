using System;

namespace Dottalk.App.Ports
{
    public interface IConnectionRepository
    {
        public void AddConnection(Guid chatRoomId, Guid userId, Guid nodeId);
        public void RemoveConnection(Guid chatRoomId, Guid userId, Guid nodeId);
    }
}