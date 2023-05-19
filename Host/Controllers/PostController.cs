using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Exceptions;
using Data.Contracts;
using Entities.Posts;
using Host.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFramework.Api;
using WebFramework.Filtters;

namespace Host.Controllers
{
    [Route("api/[controller]")]
    [ApiResultFilter]
    [ApiController]
    public class PostController : Controller
    {
        private readonly IRepository<Post> repository;
        private readonly IMapper mapper;

        public PostController(IRepository<Post> repository, IMapper mapper  )
        {
            this.repository = repository;
            this.mapper = mapper;
          
        }

        [HttpGet]
        public async Task<object> GetAllPost()
        {
            return await repository.TableNoTracking.ToArrayAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<object> FindById(int id)
        {
            //var pp = await repository.TableNoTracking
            //    .ProjectTo<PostViewModel>(mapper.ConfigurationProvider)
            //    .FirstOrDefaultAsync(c => c.Id == id);

            //if (pp == null)
            //    throw new NotFoundException("هیچ پستی با این شناسه یافت نشد.");
 

            return new { };
        }
        [HttpPost]
        public ApiResult Create(PostViewModel post)
        {
            var mapperpost = new PostViewModelMapper(mapper);
          var pp = mapperpost.Map(post);
 
            repository.Add(pp);
            return  Ok();
        }

 

        [HttpPut]
        public ActionResult Update(PostViewModel model, int id)
        {
            //var oldModel =repository.GetById(id);

            //var newModel = model.Map(oldModel);

            //repository.Update(newModel);

            return View();
        }



        [HttpDelete]
        public ActionResult Delete(int id)
        {
            return View();
        }

 
    }
}
