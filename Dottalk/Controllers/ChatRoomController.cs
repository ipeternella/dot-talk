using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dottalk.App.Domain.Models;
using Dottalk.Infra.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hangman.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ChatRoomController : ControllerBase
    {
        private readonly DBContext _DBContext;
        private readonly ILogger<ChatRoomController> _logger;

        public ChatRoomController(ILogger<ChatRoomController> logger, DBContext DBContext)
        {
            _DBContext = DBContext;
            _logger = logger;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ChatRoom>> GetById(Guid id)
        {
            var chatRoom = await _DBContext.ChatRooms.FindAsync(id);
            if (chatRoom == null) return NotFound(new { Message = "Chat room was not found." });

            return Ok(chatRoom);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatRoom>>> All()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<ActionResult<ChatRoom>> Create()
        {
            throw new NotImplementedException();
        }
    }
}