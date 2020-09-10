using System;
using System.Linq;
using System.Threading.Tasks;
using Dottalk.App.DTOs;
using Dottalk.App.Services;
using Dottalk.App.Utils;
using Dottalk.Infra.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hangman.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ChatRoomController : ControllerBase
    {
        private readonly ILogger<ChatRoomController> _logger;
        private readonly DBContext _db;

        public ChatRoomController(ILogger<ChatRoomController> logger, DBContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            var chatRoom = await _db.ChatRooms.FindAsync(id);
            if (chatRoom == null) return NotFound(new { Message = "Chat room was not found." });

            return Ok(chatRoom);
        }

        [HttpGet]
        public async Task<ActionResult> All([FromQuery] PaginationParams paginationParams)
        {
            var chatRooms = await _db.ChatRooms.OrderBy(room => room.Id).GetPage(paginationParams).ToListAsync();
            return Ok(chatRooms);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ChatRoomCreationDTO chatRoomCreationDTO)
        {
            var validator = new ChatRoomCreationValidator();
            var validationResult = validator.Validate(chatRoomCreationDTO);

            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

            return Ok();
        }
    }
}