using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities.Base
{ 
    public abstract class BaseEntity
    {
        public long Id { get; set; }
        public DateTime CreatingDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
