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
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<ChatRoomController> _logger;

        public UserController(ILogger<ChatRoomController> logger, IUserService userService)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            _logger.LogInformation($"Getting user by the id: {id}");

            try
            {
                var result = await _userService.GetUser(id);
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
            var chatRooms = await _userService.GetAllUsers(paginationParams);

            return Ok(chatRooms);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] UserCreationRequestDTO userCreationRequestDTO)
        {
            _logger.LogInformation($"New user request: {userCreationRequestDTO}");

            var validator = new UserCreationValidator();
            var validationResult = validator.Validate(userCreationRequestDTO);

            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

            try
            {
                var result = await _userService.CreateUser(userCreationRequestDTO);
                return StatusCode(201, result);
            }
            catch (ObjectAlreadyExistsException e)
            {
                return BadRequest(new ServiceErrorDTO { Message = e.Message });
            }
        }
    }
}