using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class Place
    {
        public Place()
        {
            Meeting = new HashSet<Meeting>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Meeting> Meeting { get; set; }
    }
}
