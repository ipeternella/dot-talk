using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dottalk.App.Domain.Models;
using Dottalk.App.DTOs;
using Dottalk.App.Exceptions;
using Dottalk.App.Ports;
using Dottalk.App.Utils;
using Dottalk.Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dottalk.App.Services
{
    //
    // Summary:
    //   Chatting usecases/services offered by the application.
    public class ChatRoomService : IChatRoomService
    {
        private readonly ILogger<IChatRoomService> _logger;
        private readonly IUserService _userService;
        private readonly RedisContext _redis;
        private readonly IMapper _mapper;
        private readonly DBContext _db;

        public ChatRoomService(ILogger<IChatRoomService> logger, DBContext db,
            IMapper mapper, RedisContext redis, IUserService userService)
        {
            _userService = userService;
            _logger = logger;
            _mapper = mapper;
            _redis = redis;
            _db = db;
        }
        //
        // Summary:
        //   Gets a specific chat room, if exists. Otherwise, raises an exception.
        public async Task<ChatRoomResponseDTO> GetChatRoom(Guid chatRoomId)
        {
            var chatRoom = await _db.ChatRooms.FindAsync(chatRoomId);
            if (chatRoom == null) throw new ObjectDoesNotExistException("Chat room does not exist!");

            return _mapper.Map<ChatRoom, ChatRoomResponseDTO>(chatRoom);
        }
        //
        // Summary:
        //   Gets a specific chat room by its name, if exists. Otherwise, raises an exception.
        public async Task<ChatRoomResponseDTO> GetChatRoom(string chatRoomName)
        {
            var chatRoom = await _db.ChatRooms.Where(room => room.Name == chatRoomName).FirstOrDefaultAsync();
            if (chatRoom == null) throw new ObjectDoesNotExistException("Chat room does not exist!");

            return _mapper.Map<ChatRoom, ChatRoomResponseDTO>(chatRoom);
        }
        // 
        // Summary: 
        //   Gets all the users connected to a given chat room across all application instances hosting such room.
        //   If the room is not found, it raises an exception.
        public async Task<ChatRoomConnectionPool> GetChatRoomConnectionPool(string chatRoomName)
        {
            _logger.LogInformation("Getting connection store for chat room name: {chatRoomName:l}", chatRoomName);
            var chatRoom = await GetChatRoom(chatRoomName);

            _logger.LogInformation("Chat room exists, attempting to retrieve a previous connection pool from Redis...");
            var chatRoomActiveConnectionPool = await _redis.GetKey<ChatRoomConnectionPool>(chatRoom.Id);
            if (chatRoomActiveConnectionPool != null) return chatRoomActiveConnectionPool;

            _logger.LogInformation("Chat room had no previous connection pool on Redis. Creating a new one...");
            var newChatRoomConnectionPool = new ChatRoomConnectionPool
            {
                ChatRoomId = chatRoom.Id,
                ActiveConnectionsLimit = chatRoom.ActiveConnectionsLimit,
                TotalActiveConnections = 0,
                ServerInstances = new List<ServerInstance>()
            };

            return newChatRoomConnectionPool;
        }
        //
        // Summary:
        //   Connects a user to a given chat room by updating the chat room's active connection pool with the user's
        //   new connection. If the the user or chat room are not found or if the room is full, it raises an exception.
        //
        // Returns:
        //   ChatRoomConnectionPool - an updated chat room connection pool with the users new connection.
        //
        // Raises:
        //   ChatRoomIsFullException - the chat is full (risen by the chat room domain logic)
        //   ObjectDoesNotExistException - the chat room or the user were not found
        //   UserIsAlreadyConnectedException - the user is attempting to connect to a chat room in which he's already connected
        public async Task<ChatRoomConnectionPool> AddUserToChatRoomConnectionPool(string chatRoomName, string userName, string connectionId)
        {
            var chatRoomConnectionPool = await GetChatRoomConnectionPool(chatRoomName);
            var user = await _userService.GetUser(userName);

            var chatRoomId = chatRoomConnectionPool.ChatRoomId;
            var serverInstanceId = GlobalState.ServerInstanceId;

            // domain logic to increment the chat room distributed connections
            var updatedChatRoomConnectionPool = ChatRoomLogic.IncrementChatRoomConnectionPool(user.Id, serverInstanceId, connectionId, chatRoomConnectionPool);
            await _redis.SetKey(chatRoomId, updatedChatRoomConnectionPool, null);

            return updatedChatRoomConnectionPool;
        }
        //
        // Summary:
        //   Removes a connection id from a given chat room by updating the chat room's active connection pool.
        //
        // Returns:
        //   ChatRoomConnectionPool - an updated chat room connection pool without the user connection.
        //
        // Raises:
        //   ChatRoomIsFullException - the chat is full (risen by the chat room domain logic)
        //   ObjectDoesNotExistException - the chat room or the user were not found
        //   UserIsAlreadyConnectedException - the user is attempting to connect to a chat room in which he's already connected
        public async Task<Tuple<ChatRoomConnectionPool, string>> RemoveConnectionFromChatRoomConnectionPool(string chatRoomName, string connectionId)
        {
            var chatRoomConnectionPool = await GetChatRoomConnectionPool(chatRoomName);
            var chatRoomId = chatRoomConnectionPool.ChatRoomId;
            var serverInstanceId = GlobalState.ServerInstanceId;

            var updatedChatRoomConnectionPool = ChatRoomLogic.DecrementChatRoomConnectionPool(serverInstanceId, connectionId, chatRoomConnectionPool);
            await _redis.SetKey(chatRoomId, updatedChatRoomConnectionPool, null);

            return new Tuple<ChatRoomConnectionPool, string>(updatedChatRoomConnectionPool, connectionId);
        }
        //
        // Summary:
        //   Removes a username id from a given chat room by updating the chat room's active connection pool.
        //
        // Returns:
        //   ChatRoomConnectionPool - an updated chat room connection pool without the user connection.
        //
        // Raises:
        //   ChatRoomIsFullException - the chat is full (risen by the chat room domain logic)
        //   ObjectDoesNotExistException - the chat room or the user were not found
        //   UserIsAlreadyConnectedException - the user is attempting to connect to a chat room in which he's already connected
        public async Task<Tuple<ChatRoomConnectionPool, string>> RemoveUserFromChatRoomConnectionPool(string chatRoomName, string userName)
        {
            var chatRoomConnectionPool = await GetChatRoomConnectionPool(chatRoomName);
            var user = await _userService.GetUser(userName);
            var chatRoomId = chatRoomConnectionPool.ChatRoomId;
            var serverInstanceId = GlobalState.ServerInstanceId;

            var removedConnectionId = ChatRoomLogic.GetConnectionIdFromChatRoomConnectionPool(user.Id, chatRoomConnectionPool);
            var updatedChatRoomConnectionPool = ChatRoomLogic.DecrementChatRoomConnectionPool(serverInstanceId, user.Id, chatRoomConnectionPool);

            await _redis.SetKey(chatRoomId, updatedChatRoomConnectionPool, null);

            return new Tuple<ChatRoomConnectionPool, string>(updatedChatRoomConnectionPool, removedConnectionId);
        }
        //
        // Summary:
        //   Gets all chat rooms given the pagination params.
        public async Task<IEnumerable<ChatRoomResponseDTO>> GetAllChatRooms(PaginationParams? paginationParams)
        {
            if (paginationParams == null) paginationParams = new PaginationParams();  // default pagination params

            var chatRooms = _db.ChatRooms.OrderBy(room => room.Id).GetPage(paginationParams);
            return await _mapper.ProjectTo<ChatRoomResponseDTO>(chatRooms).ToListAsync();
        }
        //
        // Summary:
        //   Creates a new chat room. If the room name is already taken, then it raises an exception.
        public async Task<ChatRoomResponseDTO> CreateChatRoom(ChatRoomCreationRequestDTO chatRoomCreationRequestDTO)
        {
            var roomWithSameName = await _db.ChatRooms.Where(room => room.Name == chatRoomCreationRequestDTO.Name).FirstOrDefaultAsync();
            if (roomWithSameName != null) throw new ObjectAlreadyExistsException("A chat room with this name already exists!");

            var newChatRoom = _mapper.Map<ChatRoomCreationRequestDTO, ChatRoom>(chatRoomCreationRequestDTO);

            await _db.ChatRooms.AddAsync(newChatRoom);
            await _db.SaveChangesAsync();

            return _mapper.Map<ChatRoom, ChatRoomResponseDTO>(newChatRoom);
        }
    }
}