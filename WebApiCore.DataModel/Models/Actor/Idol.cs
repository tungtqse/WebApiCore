using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.DataModel.Models
{
    public partial class Idol : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public int? Age { get; set; }
        public bool Sex { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Guid? AvatarImage { get; set; }
        public string NickName { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public string Star { get; set; }
        public string Habit { get; set; }
        public Guid? BloodId { get; set; }
    }

    public partial class Idol
    {
        public virtual Blood Blood { get; set; }
        public ICollection<MappingAlbumIdol> AlbumIdols { get; set; }
        public ICollection<MappingMovieIdol> MovieIdols { get; set; }
    }
}
