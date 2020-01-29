using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class Direct
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public byte? IsActive { get; set; }
        public int? FromUserId { get; set; }
        public int? ToUserId { get; set; }
        public int? MlId { get; set; }

        public virtual User FromUser { get; set; }
        public virtual MeetingLine Ml { get; set; }
        public virtual User ToUser { get; set; }
    }
}
