using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.DataModel.Models
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<MappingUserRole> UserRoles { get; set; }
    }
}
