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
        public Int32 Impressions { get; set; }
        public Int32 Click { get; set; }
        public Decimal CTR { get; set; }
        public Decimal CPC { get; set; }

    }
    public class ImpressionModel
    {
        public DateTime ReportDay { get; set; }
        public Int32 Impressions { get; set; }
    }
    public class ClickModel
    {
        public DateTime ReportDay { get; set; }
        public Int32 Clicks { get; set; }
    }
    public class CTRModel
    {
        public DateTime ReportDay { get; set; }
        public Int32 Impressions { get; set; }
        public Int32 Clicks { get; set; }
    }
    public class CPCModel
    {
        public DateTime ReportDay { get; set; }
        public Decimal Spend { get; set; }
        public Int32 Clicks { get; set; }
    }
    public class GraphModel
    {
        public List<SpendModel> spendmodel { get; set; }
        public List<SalesModel> salemodel { get; set; }
        public List<AcosModel> acosmodel { get; set; }
        public List<ImpressionModel> impressions { get; set; }
        public List<ClickModel> clicks { get; set; }
        public List<CTRModel> ctr { get; set; }
        public List<CPCModel> cpc { get; set; }
    }
}
