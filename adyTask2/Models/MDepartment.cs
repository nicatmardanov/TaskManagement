using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class MDepartment
    {
        public int Id { get; set; }
        public int? Type { get; set; }
        public int DepartmentId { get; set; }
        public int RefId { get; set; }

        public virtual Department Department { get; set; }
    }
}
