using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public interface IEntity
    {

    }
    public abstract class BaseEntity<TKey>:IEntity
    {
        public TKey Id { get; set; }
        public int  CreatedBy { get; set; }
        public int  ModifiedBy   { get; set; }

        public DateTime ModifiedDate { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsActive { get; set; }

    }
    public abstract class BaseEntity:BaseEntity<int>
    {

    }
}
