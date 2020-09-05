using System;
using System.Collections.Generic;
using Dottalk.App.DTOs;

namespace Dottalk.App.Ports
{
    // Summary:
    //   Publishes a chat message to node channels so that other nodes can listen to messages sent
    //   from the same room. Offers a Subscribe interface to subscribe to a channel.
    public interface IPubSub
    {
        public void Pub(ChatMessageDTO msg, List<string> channels);
        public void Pub(ChatMessageDTO msg, string channel);
        public void Sub(string channel);
    }
}