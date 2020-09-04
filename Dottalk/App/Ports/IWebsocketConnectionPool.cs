using System;

namespace Dottalk.App.Ports
{
    public interface IWebsocketConnectionPool
    {
        public void AddWebsocket(Guid chatRoomId, Guid userId);
        public void RemoveWebsocket(Guid chatRoomId, Guid userId);
        public void BroadcastMsgToWebSockets(string msg, Guid chatRoomId);
        public void SendMsgToWebSocket(string msg, Guid chatRoomId, Guid userId);
    }
}