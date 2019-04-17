using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.DataModel.Models
{
    public partial class Album : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? PublishDate { get; set; }

    }

    public partial class Album
    {
        public ICollection<MappingAlbumIdol> AlbumIdols { get; set; }
        public ICollection<MappingAlbumImage> AlbumImages { get; set; }
    }
}
