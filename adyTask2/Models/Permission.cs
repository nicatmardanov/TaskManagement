using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class Permission
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Mtype { get; set; }
        public string Department { get; set; }
        public string Page { get; set; }

        public virtual User User { get; set; }
    }
}
