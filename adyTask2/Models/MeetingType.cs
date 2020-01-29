using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class MeetingType
    {
        public MeetingType()
        {
            Meeting = new HashSet<Meeting>();
        }

        public byte Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Meeting> Meeting { get; set; }
    }
}
