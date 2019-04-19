using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.DataModel.Models
{
    public class MappingUserRole : BaseEntity
    {
        public Guid UserCredentialId { get; set; }
        public UserCredential UserCredential { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
    }
}
