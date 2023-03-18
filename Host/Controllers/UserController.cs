using Common.Exceptions;
using Data.Contracts;
using Data.Repositories;
using Entities.Users;
using Host.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.SdkServices;
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
        private readonly IJwtService jwtService;
        private readonly ILogger<UserController> _logger;
        private readonly ISdk _sdk;



        public UserController(IUserRepository userRepository, ILogger<UserController> logger, IJwtService jwtService, ISdk sdk)
        {
            _userRepository = userRepository;
            _logger = logger;
            this.jwtService = jwtService;
            _sdk = sdk;
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
            var z = _sdk.CurrentUser.Id;
            _logger.LogInformation("Create User");
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
        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<string> Loging(UserDto user, CancellationToken cancellationToken)
        {
            
            var d = await _userRepository.GetByUserAndPass(user.UserName, user.Password, cancellationToken);    
            if(d == null)
                throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");

            return  jwtService.Generate(d);

            
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
