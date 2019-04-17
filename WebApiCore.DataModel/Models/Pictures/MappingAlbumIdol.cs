using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.DataModel.Models
{
    public class MappingAlbumIdol : BaseEntity
    {
        public Guid AlbumId { get; set; }
        public Album Album { get; set; }
        public Guid IdolId { get; set; }
        public Idol Idol { get; set; }
    }
}
