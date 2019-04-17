using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.DataModel.Models
{
    public partial class Image : BaseEntity
    {
        public string AltName { get; set; }
        public string Extension { get; set; }
        public double Size { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public Guid? AttachmentFileId { get; set; }
    }

    public partial class Image
    {
        public virtual AttachmentFile AttachmentFile { get; set; }
        public ICollection<MappingAlbumImage> AlbumImages { get; set; }
        public ICollection<MappingMovieImage> MovieImages { get; set; }
    }
}
