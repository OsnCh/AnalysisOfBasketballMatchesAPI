using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace ApplicationCore.Entities
{
    [Table("Roles")]
    public class Role: IdentityRole
    {
        public Role(string roleName) : base(roleName)
        {

        }

        public Role()
        {
        }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}
