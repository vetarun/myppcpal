using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.UTILITIES
{
    public class Administration
    {
        public Int32 UserID { get; set; }
        public String Email { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public Boolean Active { get; set; }
        public int UserType { get; set; }
    }
}
