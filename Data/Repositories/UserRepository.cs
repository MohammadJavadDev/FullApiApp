using Common.Exceptions;
using Common.Utilities;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class UserRepository : Repository<User> , IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public  Task<User?> GetByUserAndPass(string username , string password , CancellationToken cancellationToken)
            {
            var passwordHash = SecurityHelper.GetSha256Hash(password);
          return   TableNoTracking.Where(c => c.UserName == username && c.Password == passwordHash)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public override async Task AddAsync(User entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            var user = await TableNoTracking.Select(c => new { c.UserName }).FirstOrDefaultAsync(c => c.UserName == entity.UserName);
            if (user != null)
            {
                throw new BadRequestException("نام کاربری تکراری میباشد.");

            }

            entity.Password = SecurityHelper.GetSha256Hash(entity.Password);

             await base.AddAsync(entity, cancellationToken, saveNow);
        }
    }
}
