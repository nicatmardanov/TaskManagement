using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class Attachment
    {
        public int Id { get; set; }
        public byte? Type { get; set; }
        public string Path { get; set; }
        public int? RefId { get; set; }
    }
}
