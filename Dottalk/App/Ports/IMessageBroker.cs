using System;
using System.Collections.Generic;

namespace Dottalk.App.Ports
{
    public interface IMessageBroker
    {
        public void SendMsg(string msg, List<Guid> nodeIds);
        public void SendMsg(string msg, Guid nodeId);
    }
}