using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.UTILITIES
{
   public class Formula
    {
        public Int32 FormulaID { get; set; }
        public String FormulaName { get; set; }
        public Nullable<Decimal> AcosPause { get; set; }
        public Nullable<Decimal> AcosLower { get; set; }
        public Nullable<Decimal> AcosRaise { get; set; }
        public Nullable<Decimal> AcosNegative { get; set; }

        public Nullable<Decimal> SpendPause { get; set; }
        public Nullable<Decimal> SpendLower { get; set; }
        public Nullable<Decimal> SpendNegative { get; set; }


        public Nullable<Decimal> ClicksPause { get; set; }
        public Nullable<Decimal> ClicksLower { get; set; }
        public Nullable<Decimal> ClicksNegative { get; set; }

        public Nullable<Decimal> CTRPause { get; set; }
        public Nullable<Decimal> CTRLower { get; set; }
        public Nullable<Decimal> CTRNegative { get; set; }

        public Nullable<Decimal> BidRaise { get; set; }
        public Nullable<Decimal> BidLower { get; set; }

        public Nullable<Decimal> MinBid { get; set; }
        public Nullable<Decimal> MaxBid { get; set; }
        public bool IsBestSrchCheked { get; set; }
        public Nullable<Decimal> BestSearchACos { get;set; }
        public Nullable<Decimal> BestSearchImpressons { get; set; }
    }
}
