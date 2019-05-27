using System;
using System.Collections.Generic;

namespace dbcontext
{
    public partial class UnityLog
    {
        public long Id { get; set; }
        public string Ip { get; set; }
        public string LogString { get; set; }
        public string StackTrace { get; set; }
        public byte Type { get; set; }
        public DateTime Time { get; set; }
    }
}
