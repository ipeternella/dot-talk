using System;
using System.Collections.Generic;
using Dottalk.App.DTOs;

namespace Dottalk.App.Ports
{
    // Summary:
    //   Message broker interface for publishing chat messages to node channels so that 
    //   other nodes can listen to messages sent from the same room. Offers a Subscribe 
    //   interface to subscribe to a channel.
    public interface IMessageBRoker
    {
        public void Publish(ChatMessageDTO msg, IEnumerable<string> channels);
        public void Publish(ChatMessageDTO msg, string channel);
        public void Subscribe(string channel);
    }
}