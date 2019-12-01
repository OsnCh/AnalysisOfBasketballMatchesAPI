using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace ApplicationCore.Entities
{
    [Table("Users")]
    public class User: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual Admin AdminData { get; set; }
    }
}
