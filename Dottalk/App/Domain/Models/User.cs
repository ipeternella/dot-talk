using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dottalk.App.Domain.Models
{
    //
    // Summary:
    //   Represents a chat user with a name (possibly a nick name).
    public class User : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;
        // navigation property
        public ICollection<ChatMessage> ChatMessages { get; set; } = null!;
    }
}