using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace Entities.Posts
{
    public class Category : BaseEntity
    {
        public string? Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        public ICollection<Category> ChildCatefories { get; set; } = new List<Category>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
