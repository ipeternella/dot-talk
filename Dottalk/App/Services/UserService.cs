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
    //   Usecases of the application regarding chat users of the application.
    public class UserService : IUserService
    {
        private readonly ILogger<IUserService> _logger;
        private readonly IMapper _mapper;
        private readonly DBContext _db;

        public UserService(ILogger<IUserService> logger, DBContext db, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _db = db;
        }
        //
        // Summary:
        //   Gets a specific user, if exists. Otherwise, raises an exception.
        public async Task<UserResponseDTO> GetUser(Guid userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) throw new ObjectDoesNotExistException("User does not exist.");

            return _mapper.Map<User, UserResponseDTO>(user);
        }
        //
        // Summary:
        //   Gets a specific user by its username, if exists. Otherwise, raises an exception.
        public async Task<UserResponseDTO> GetUser(string userName)
        {
            var user = await _db.Users.Where(user => user.Name == userName).FirstOrDefaultAsync();
            if (user == null) throw new ObjectDoesNotExistException("User does not exist.");

            return _mapper.Map<User, UserResponseDTO>(user);
        }
        //
        // Summary:
        //   Gets all users given the pagination params.
        public async Task<IEnumerable<UserResponseDTO>> GetAllUsers(PaginationParams? paginationParams)
        {
            if (paginationParams == null) paginationParams = new PaginationParams();  // default pagination params

            var users = _db.Users.OrderBy(room => room.Id).GetPage(paginationParams);
            return await _mapper.ProjectTo<UserResponseDTO>(users).ToListAsync();
        }
        //
        // Summary:
        //   Creates a new user. If the room name is already taken, then it raises an exception.
        public async Task<UserResponseDTO> CreateUser(UserCreationRequestDTO userCreationRequestDTO)
        {
            var userWithSameName = await _db.Users.Where(user => user.Name == userCreationRequestDTO.Name).FirstOrDefaultAsync();
            if (userWithSameName != null) throw new ObjectAlreadyExistsException("A user with this name already exists.");

            var newUser = _mapper.Map<UserCreationRequestDTO, User>(userCreationRequestDTO);

            await _db.Users.AddAsync(newUser);
            await _db.SaveChangesAsync();

            return _mapper.Map<User, UserResponseDTO>(newUser);
        }
    }
}