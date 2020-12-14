using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movie_api
{
    public class StatData
    {
        public string MovieID { get; set; }
        public List<string> WatchDurationMs { get; set; }
        public int Count { get; set; }
    }
}
