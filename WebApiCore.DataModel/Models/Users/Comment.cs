using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.DataModel.Models
{
    public class Comment : BaseEntity
    {
        public Guid ParentId { get; set; }
        public string Content { get; set; }

        public ICollection<MappingUserComment> UserComments { get; set; }
    }
}
