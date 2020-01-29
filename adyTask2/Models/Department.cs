using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class Department
    {
        public Department()
        {
            MDepartment = new HashSet<MDepartment>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public byte? Active { get; set; }

        public virtual ICollection<MDepartment> MDepartment { get; set; }
    }
}
