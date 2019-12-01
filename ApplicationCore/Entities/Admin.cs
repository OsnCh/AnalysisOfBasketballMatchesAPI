using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ApplicationCore.Entities.Base;

namespace ApplicationCore.Entities
{
    [Table("Admins")]
    public class Admin: BaseEntity
    {
        public string UserId { get; set; }

        public virtual User User { get; set; }
    }
}
