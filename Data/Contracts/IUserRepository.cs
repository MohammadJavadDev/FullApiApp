using Common;
using Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Contracts
{
    public interface IUserRepository  :IRepository<User> 
    {
         Task<User> GetByUserAndPass(string username,string password , CancellationToken cancellationToken);

            
    }
}
