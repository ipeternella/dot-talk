using System;
using System.Linq;
using System.Threading.Tasks;
using Dottalk.App.DTOs;
using Dottalk.App.Exceptions;
using Dottalk.App.Ports;
using Dottalk.App.Services;
using Dottalk.App.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hangman.Controllers.V1
{
    //
    // Summary:
    //   Controller used to manage CRUD operations for the chat rooms.
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ChatRoomController : ControllerBase
    {
        private readonly IChatRoomService _chatRoomService;
        private readonly ILogger<ChatRoomController> _logger;

        public ChatRoomController(ILogger<ChatRoomController> logger, IChatRoomService chatRoomService)
        {
            _chatRoomService = chatRoomService;
            _logger = logger;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            _logger.LogInformation($"Getting chat room by the id: {id}");

            try
            {
                var result = await _chatRoomService.GetChatRoom(id);
                return Ok(result);
            }
            catch (ObjectDoesNotExistException e)
            {
                return NotFound(new ServiceErrorDTO { Message = e.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult> All([FromQuery] PaginationParams paginationParams)
        {
            var chatRooms = await _chatRoomService.GetAllChatRooms(paginationParams);

            return Ok(chatRooms);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ChatRoomCreationRequestDTO chatRoomCreationRequestDTO)
        {
            _logger.LogInformation($"New chat room request: {chatRoomCreationRequestDTO}");

            var validator = new ChatRoomCreationValidator();
            var validationResult = validator.Validate(chatRoomCreationRequestDTO);

            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

            try
            {
                var result = await _chatRoomService.CreateChatRoom(chatRoomCreationRequestDTO);
                return StatusCode(201, result);
            }
            catch (ObjectAlreadyExistsException e)
            {
                return BadRequest(new ServiceErrorDTO { Message = e.Message });
            }
        }
    }
}