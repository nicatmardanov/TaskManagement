using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class UserOld
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public byte? Active { get; set; }
        public string Position { get; set; }
        public int? DepartmentId { get; set; }
    }
}
