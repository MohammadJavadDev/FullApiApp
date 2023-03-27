using AutoMapper;
using Entities;
using Entities.Posts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFramework.Mapper;

namespace WebFramework.Api
{
    public abstract class BaseViewModel<TViewModel, TEntity, TKey>  
        where TViewModel : class, new()
        where TEntity : BaseEntity<TKey>, new()
    {
        private readonly IMapper mapper;

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

        public virtual TViewModel Map(TEntity entity)
        {
            return mapper.Map<TViewModel>(entity);
        }

        public virtual TEntity Map(TViewModel dto)
        {
            return mapper.Map<TEntity>(dto);
        }
    }
}
