using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class Revision
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UserId { get; set; }
        public int? MlId { get; set; }
        public byte? Active { get; set; }

        public virtual MeetingLine Ml { get; set; }
        public virtual User User { get; set; }
    }
}
