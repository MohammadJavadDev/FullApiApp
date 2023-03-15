 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Users;
namespace Entities.Posts
{
    public class Post :BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int  CategoryId { get; set; }
        public int AuthorId{ get; set; }

        public Category? Category { get; set; }

        public User? Author { get; set; }



    }

    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c=>c.Title).IsRequired().HasMaxLength(50);
            builder.Property(c=>c.Description).IsRequired().HasMaxLength(500);
            builder.HasOne(c => c.Category).WithMany(c => c.Posts).HasForeignKey(c => c.CategoryId);
            builder.HasOne(c => c.Author).WithMany(c => c.Posts).HasForeignKey(c => c.AuthorId);
        }
    }


}
