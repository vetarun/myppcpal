using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.UTILITIES
{
    public class SpendModel
    { 
        public DateTime ReportDay { get; set; }
        public Decimal Spend { get; set; }
    }

    public class SalesModel
    {
        public DateTime ReportDay { get; set; }
        public Decimal Sales { get; set; }
    }

    public class AcosModel
    {
        public DateTime ReportDay { get; set; }
        public Decimal Spend { get; set; }
        public Decimal Sales { get; set; }
    }

    public class GraphInventoryModel
    {
        public DateTime ReportDay { get; set; }
        public Decimal Spend { get; set; }
        public Decimal Sales { get; set; }
        public Decimal ACoS { get; set; }
    }

    public class GraphModel
    {
        public List<SpendModel> spendmodel { get; set; }
        public List<SalesModel> salemodel { get; set; }
        public List<AcosModel> acosmodel { get; set; }
    }
}
