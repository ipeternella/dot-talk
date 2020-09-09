using System;
using Dottalk.App.DTOs;
using Dottalk.App.Ports;
using Microsoft.Extensions.Logging;

namespace Dottalk.App.Services
{
    //
    // Summary:
    //   Chatting services offered by the application.
    public class ChatRoomService : IChatRoomService
    {
        private readonly IChatRoomConnectionsRepository _connectionsRepository;
        private readonly ILogger<IChatRoomService> _logger;

        public ChatRoomService(ILogger<IChatRoomService> logger, IChatRoomConnectionsRepository connectionsRepository)
        {
            _connectionsRepository = connectionsRepository;
            _logger = logger;
        }
        //
        // Summary:
        //   Checks if a given room is full by checking the connections repository of the application. If not,
        //   updates the repository with a new connection for the chat room.
        public ServiceResultVoidDTO AddConnection(Guid userId, Guid chatRoomId)
        {
            // 0. Assert that such user and chat room exist!
            // 1. Get chat room connections (if exists)x
            // 2. If ActiveConnectionsLimit < TotalActiveConnectionsLimit -> update the connections store
            // 3. If not, return false and refuse the connection.
            throw new NotImplementedException();
        }
    }
}