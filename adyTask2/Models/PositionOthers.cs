using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class PositionOthers
    {
        public PositionOthers()
        {
            OtherParticipiants = new HashSet<OtherParticipiants>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<OtherParticipiants> OtherParticipiants { get; set; }
    }
}
