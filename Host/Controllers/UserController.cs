using AutoMapper;
using Common.Exceptions;
using Data.Contracts;
using Data.Repositories;
using Entities.Users;
using Host.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly SignInManager<User> signInManager;
        private readonly IMapper mapper;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger, IJwtService jwtService, ISdk sdk
            , UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager, IMapper mapper)
        {
            _userRepository = userRepository;
            _logger = logger;
            this.jwtService = jwtService;
            _sdk = sdk;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.mapper = mapper;
        }

        [HttpGet]
   
        public async Task<ApiResult<List<User>>> Get()
        {

            var users = await _userRepository.TableNoTracking.ToListAsync();
            return users;
        }
        [HttpGet("{id}")]
 
        public async Task<ApiResult<User>> Get(string id,CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(id);
             
            //var user = await _userRepository.GetByIdAsync(cancellationToken, id);
            return user;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<object> Create(User user, CancellationToken cancellationToken)
        {
         
      var res = await  userManager.CreateAsync(user,user.PasswordHash);

       var res2 =  await   roleManager.CreateAsync(new Role
            {
                Name = "Admin"
            });

        var res3=   await userManager.AddToRoleAsync(user, "Admin");


            //_logger.LogInformation("Create User");
            //await _userRepository.AddAsync(user, cancellationToken);
            return new {res , res2 , res3 };
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id , UserViewModel user, CancellationToken cancellationToken)
        {


            //var updatedUser = await _userRepository.GetByIdAsync(cancellationToken, id);
            //updatedUser.UserName = user.UserName;
            //updatedUser.PasswordHash = user.Password;
            //updatedUser.FullName = user.FullName;
            //updatedUser.Gender = user.Gender;

            var updatedUser  = mapper.Map<User>(user);


            await userManager.UpdateAsync(updatedUser);
            return Ok(updatedUser);

        }
        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<string> Loging(UserViewModel user, CancellationToken cancellationToken)
        {
             // var d = await _userRepository.GetByUserAndPass(user.UserName, , cancellationToken);    
 
            var resUser = await userManager.FindByNameAsync(user.UserName);

            if (resUser == null)
            {
                throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");
            }

      //      var ps = await userManager.CheckPasswordAsync(user, user.PasswordHash);

          var ps = await signInManager.PasswordSignInAsync(resUser, user.Password, false, false);

            if(!ps.Succeeded)
                throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");

            return await jwtService.GenerateAsync(resUser);

            
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
