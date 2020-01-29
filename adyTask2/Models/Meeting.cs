using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class Meeting
    {
        public Meeting()
        {
            Mail = new HashSet<Mail>();
            MeetingLine = new HashSet<MeetingLine>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public byte? MeetingType { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? MeetingDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string Description { get; set; }
        public int? Place { get; set; }
        public byte? IsActive { get; set; }
        public string OwnerUser { get; set; }
        public string FollowerUser { get; set; }
        public string InformedUser { get; set; }
        public string Participiants { get; set; }
        public string OtherParticipiants { get; set; }
        public int? CreatorId { get; set; }
        public byte? StatusId { get; set; }
        public byte? IsPublished { get; set; }

        public virtual MeetingType MeetingTypeNavigation { get; set; }
        public virtual Place PlaceNavigation { get; set; }
        public virtual Status Status { get; set; }
        public virtual ICollection<Mail> Mail { get; set; }
        public virtual ICollection<MeetingLine> MeetingLine { get; set; }
    }
}
