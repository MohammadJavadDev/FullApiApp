using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Posts;
using Microsoft.AspNetCore.Identity;

namespace Entities.Users
{

    public class User : IdentityUser,IEntity
    {
  

        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }

        public Gender? Gender { get; set; }


        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }

    public enum Gender
    {
        [Display(Name = "خانم")]
        Woman = 0,
        [Display(Name = "آقا")]
        Man = 1
    }
}
