using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class MLog
    {
        public int Id { get; set; }
        public byte? Type { get; set; }
        public string IpAdress { get; set; }
        public string Description { get; set; }
        public string AdditionalInformation { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ReadDate { get; set; }
        public int RefId { get; set; }
        public int? UserId { get; set; }
        public int? OperationId { get; set; }

        public virtual Operations Operation { get; set; }
        public virtual User User { get; set; }
    }
}
