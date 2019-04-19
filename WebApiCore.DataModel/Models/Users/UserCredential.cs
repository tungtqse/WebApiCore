using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.DataModel.Models
{
    public partial class UserCredential : BaseEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsLock { get; set; }
        public int AccessFailedCount { get; set; }
        public string SecurityStamp { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public Guid UserProfileId { get; set; }        
    }

    public partial class UserCredential
    {
        public virtual UserProfile UserProfile { get; set; }
        public ICollection<MappingUserRole> UserRoles { get; set; }
    }
}
