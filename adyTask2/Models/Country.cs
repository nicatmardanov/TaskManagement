using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class Country
    {
        public Country()
        {
            OtherParticipiants = new HashSet<OtherParticipiants>();
        }

        public short Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<OtherParticipiants> OtherParticipiants { get; set; }
    }
}
