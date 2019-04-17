using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.DataModel.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<MappingMovieCategory> MovieCategories { get; set; }
    }
}
