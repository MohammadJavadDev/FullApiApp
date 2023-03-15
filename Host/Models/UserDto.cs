using Entities.Users;
using System.ComponentModel.DataAnnotations;

namespace Host.Models
{
    public class UserDto
    {
        [Required]
        [StringLength(100)]
        public string? UserName { get; set; }
        [Required]
        [StringLength(500)]
        public string? Password { get; set; }
        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }

        public Gender? Gender { get; set; }

 

        
    }
}
