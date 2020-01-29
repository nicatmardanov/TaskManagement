using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class MlType
    {
        public MlType()
        {
            MeetingLine = new HashSet<MeetingLine>();
        }

        public byte Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<MeetingLine> MeetingLine { get; set; }
    }
}
