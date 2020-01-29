using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class MeetingLine
    {
        public MeetingLine()
        {
            Direct = new HashSet<Direct>();
            MlDetails = new HashSet<MlDetails>();
            Revision = new HashSet<Revision>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public DateTime? ReadDate { get; set; }
        public string ResponsibleEmail { get; set; }
        public string FollowerEmail { get; set; }
        public string IdentifierEmail { get; set; }
        public string InformedUserEmail { get; set; }
        public byte? IsPublished { get; set; }
        public byte? IsDirected { get; set; }
        public byte? IsRevised { get; set; }
        public int? CreatorId { get; set; }
        public byte? MlType { get; set; }
        public int? MeetingId { get; set; }
        public byte? StatusId { get; set; }

        public virtual User Creator { get; set; }
        public virtual Meeting Meeting { get; set; }
        public virtual MlType MlTypeNavigation { get; set; }
        public virtual Status Status { get; set; }
        public virtual ICollection<Direct> Direct { get; set; }
        public virtual ICollection<MlDetails> MlDetails { get; set; }
        public virtual ICollection<Revision> Revision { get; set; }
    }
}
