using System;
using System.Collections.Generic;
using System.Text;

namespace VR_Project
{
    public class Data
    {
#nullable enable
        public string? id { get; set; }
        public DateTime? beginTime { get; set; }
        public DateTime? lastPing { get; set; }
        public List<Fp>? fps { get; set; }
        public List<string>? features { get; set; }
        public Clientinfo? clientinfo { get; set; }
#nullable disable
    }
}
