using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.DataModel.Models
{
    public class MappingMovieIdol : BaseEntity
    {
        public Guid MovieId { get; set; }
        public Movie Movie { get; set; }
        public Guid IdolId { get; set; }
        public Idol Idol { get; set; }
    }
}
