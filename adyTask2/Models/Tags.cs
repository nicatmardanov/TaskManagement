using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class Tags
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte? Type { get; set; }
        public int? RefId { get; set; }
    }
}
