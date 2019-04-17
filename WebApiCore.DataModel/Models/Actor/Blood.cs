using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.DataModel.Models
{
    public class Blood : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<Idol> Idols { get; set; }
    }
}
