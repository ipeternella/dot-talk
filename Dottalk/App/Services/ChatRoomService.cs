using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dottalk.App.Domain.Models;
using Dottalk.App.DTOs;
using Dottalk.App.Exceptions;
using Dottalk.App.Ports;
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
        // private readonly IChatRoomConnectionsRepository _connectionsRepository;        
        private readonly ILogger<IChatRoomService> _logger;
        private readonly IMapper _mapper;
        private readonly DBContext _db;

        public ChatRoomService(ILogger<IChatRoomService> logger, DBContext db, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _db = db;
        }
        //
        // Summary:
        //   Checks if a given room is full by checking the connections repository of the application. If not,
        //   updates the repository with a new connection for the chat room.
        public async Task<ChatRoomCreationResponseDTO> CreateChatRoom(ChatRoomCreationRequestDTO chatRoomCreationRequestDTO)
        {
            var roomWithSameName = await _db.ChatRooms.Where(room => room.Name == chatRoomCreationRequestDTO.Name).FirstOrDefaultAsync();
            if (roomWithSameName != null) throw new ChatRoomAlreadyExistsException("A chat room with this name already exists.");

            var newChatRoom = _mapper.Map<ChatRoomCreationRequestDTO, ChatRoom>(chatRoomCreationRequestDTO);

            await _db.ChatRooms.AddAsync(newChatRoom);
            await _db.SaveChangesAsync();

            return _mapper.Map<ChatRoom, ChatRoomCreationResponseDTO>(newChatRoom);
        }
    }
}