using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.DataModel.Models
{
    public partial class Movie : BaseEntity
    {
        public string Title { get; set; }
        public string Code { get; set; }
        public double Duration { get; set; }
        public string Description { get; set; }
        public double Rate { get; set; }
        public DateTime? PublishDate { get; set; }
        public Guid? LabelImageId { get; set; }
    }

    public partial class Movie
    {
        public ICollection<MappingMovieIdol> MovieIdols { get; set; }
        public ICollection<MappingMovieImage> MovieImages { get; set; }
        public ICollection<MappingMovieCategory> MovieCategories { get; set; }
        public ICollection<AttachmentFile> AttachmentFiles { get; set; }
        public ICollection<SourceLink> SourceLinks { get; set; }
    }
}
