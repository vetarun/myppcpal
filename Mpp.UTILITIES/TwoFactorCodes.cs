using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.UTILITIES
{
    public class TwoFactorCodes
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public String Time { get; set; }
        public bool Required { get; set; }
        public int? Code { get; set; }
    }
}
