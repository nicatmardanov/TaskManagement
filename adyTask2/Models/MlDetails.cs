using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class MlDetails
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Completion { get; set; }
        public string Description { get; set; }
        public int? MlId { get; set; }
        public int? UserId { get; set; }

        public virtual MeetingLine Ml { get; set; }
        public virtual User User { get; set; }
    }
}
