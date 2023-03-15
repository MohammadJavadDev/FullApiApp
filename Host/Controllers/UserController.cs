using Data.Contracts;
using Data.Repositories;
using Entities.Users;
using Host.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFramework.Api;
using WebFramework.Filtters;

namespace Host.Controllers
{
    [Route("api/[controller]")]
    [ApiResultFilter]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ApiResult<List<User>>> Get()
        {

            var users = await _userRepository.TableNoTracking.ToListAsync();
            return users;
        }
        [HttpGet("{id:int}")]
        public async Task<ApiResult<User>> Get(int id,CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(cancellationToken, id);
            return user;
        }
        [HttpPost]
        public async Task<object> Create(User user, CancellationToken cancellationToken)
        {
            await _userRepository.AddAsync(user, cancellationToken);
            return user;
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id , UserDto user, CancellationToken cancellationToken)
        {
            var updatedUser = await _userRepository.GetByIdAsync(cancellationToken, id);
            updatedUser.UserName = user.UserName;
            updatedUser.Password = user.Password;
            updatedUser.FullName = user.FullName;
            updatedUser.Gender = user.Gender;
      
            
            await _userRepository.UpdateAsync(updatedUser,cancellationToken);
            return Ok(updatedUser);

        }

        [HttpDelete]
        public async Task<ActionResult> Delete (int id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(cancellationToken, id);
            await _userRepository.DeleteAsync(user,cancellationToken);
            return Ok(user);
        }
    }
}
