using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class Operations
    {
        public Operations()
        {
            MLog = new HashSet<MLog>();
        }

        public int Id { get; set; }
        public string Operation { get; set; }

        public virtual ICollection<MLog> MLog { get; set; }
    }
}
