using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class Status
    {
        public Status()
        {
            Meeting = new HashSet<Meeting>();
            MeetingLine = new HashSet<MeetingLine>();
        }

        public byte Id { get; set; }
        public string StatusName { get; set; }

        public virtual ICollection<Meeting> Meeting { get; set; }
        public virtual ICollection<MeetingLine> MeetingLine { get; set; }
    }
}
