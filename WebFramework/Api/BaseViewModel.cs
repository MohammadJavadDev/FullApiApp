using AutoMapper;
using Entities;
 
using WebFramework.Mapper;

namespace WebFramework.Api
{
    public  class BaseViewModel<TViewModel, TEntity, TKey> : IHaveCustomMapping
        where TViewModel : class, new()
        where TEntity : BaseEntity<TKey>, new()
    {
        public   IMapper mapper;

        public BaseViewModel(IMapper mapper)
        {
            this.mapper = mapper;
        }
          
        public TKey Id { get; set; }
        public string? CreatedByFullName { get; set; }
        public string? ModifiedByFullName { get; set; }

        public DateTime ModifiedDate { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsActive { get; set; }

        public void CreateMappings(Profile profile)
        {
            profile.CreateMap<TEntity, TViewModel>().ReverseMap();
        }

        public TEntity ToEntity()
        {  
          return  mapper.Map<TEntity>(CastToDerivedClass(this));
        } 
        
        public TEntity ToEntity(TEntity entity)
        {  
          return  mapper.Map(CastToDerivedClass(this) , entity);
        }

        public  TViewModel FromEntity(TEntity entity)
        {
            return mapper.Map<TViewModel>(entity);
        }
        protected TViewModel CastToDerivedClass(BaseViewModel<TViewModel, TEntity, TKey> baseViewModel)
        {
            return mapper.Map<TViewModel>(baseViewModel);
        }
    }
}
