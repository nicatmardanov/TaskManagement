using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class OtherParticipiants
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Profession { get; set; }
        public string Company { get; set; }
        public short? CountryId { get; set; }
        public int? PositionId { get; set; }

        public virtual Country Country { get; set; }
        public virtual PositionOthers Position { get; set; }
    }
}
