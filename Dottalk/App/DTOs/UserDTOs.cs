using System;

namespace Dottalk.App.DTOs
{
    public class UserCreationRequestDTO
    {
        public string Name { get; set; } = default!;
    }

    public class UserResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}