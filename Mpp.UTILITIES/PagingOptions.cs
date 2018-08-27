using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.UTILITIES
{
    public class PagingOptions
    {
        public Int32 Start { get; set; }
        public Int32 Length { get; set; }
        public String ColumnName { get; set; }
        public bool Direction { get; set; }
        public String SearchName { get; set; }
        public string Date { get; set; }
        public string Date2 { get; set; }
        public int Client { get; set; }
        public bool IsIgnoreZero { get; set; }
    }
}
