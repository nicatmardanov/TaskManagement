using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class Reports
    {
        public int Id { get; set; }
        public string ReportName { get; set; }
        public string ReportString { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreateDate { get; set; }

        public virtual User User { get; set; }
    }
}
