using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class Mail
    {
        public int Id { get; set; }
        public string ToUser { get; set; }
        public string Cc { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? SendDate { get; set; }
        public byte? Type { get; set; }
        public byte? Count { get; set; }
        public string ErrorText { get; set; }
        public int? RefId { get; set; }

        public virtual Meeting Ref { get; set; }
    }
}
