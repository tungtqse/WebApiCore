using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.DataModel.Models
{
    public partial class UserProfile : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public int? Age { get; set; }
        public bool? Sex { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Guid? AvatarImage { get; set; }
    }

    public partial class UserProfile
    {
        public virtual AttachmentFile AttachmentFile { get; set; }
        public ICollection<MappingUserComment> UserComments { get; set; }
    }
}
