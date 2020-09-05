using System;

namespace Dottalk.App.Ports
{
    public interface IWebSocketConnectionPool
    {
        public void AddWebSocket(Guid chatRoomId, Guid userId);
        public void RemoveWebSocket(Guid chatRoomId, Guid userId);
        public void BroadcastMsgToWebSockets(string msg, Guid chatRoomId);
        public void SendMsgToWebSocket(string msg, Guid chatRoomId, Guid userId);
    }
}