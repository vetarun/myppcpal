using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mpp.UTILITIES.Statics;

namespace Mpp.BUSINESS
{
    public class UserPlans
    {
        public static int GetPlanBySku(int skuCount)
        {
            int plan = (int)StripePlans.NO_PLAN;// 0 skus

            if (skuCount > 0 && skuCount <= 10)
                plan = (int)StripePlans.SOLOPRENEUR;
            else if (skuCount > 10 && skuCount <= 25)
                plan = (int)StripePlans.STARTUP;
            else if (skuCount > 25 && skuCount <= 50)
                plan = (int)StripePlans.BUSINESS_TIME;
            else if (skuCount > 50 && skuCount <= 100)
                plan = (int)StripePlans.BIG_BUSINESS;
            else if (skuCount > 100 && skuCount <= 250)
                plan = (int)StripePlans.ENTERPRISE;

            return plan;
        }

        public static decimal GetPlanCost(int plan)
        {
            decimal plancost = 0;

            if ((int)StripePlans.SOLOPRENEUR == plan)
                plancost = 49;
            else if ((int)StripePlans.STARTUP == plan)
                plancost = 99;
            else if ((int)StripePlans.BUSINESS_TIME == plan)
                plancost = 149;
            else if ((int)StripePlans.BIG_BUSINESS == plan)
                plancost = 199;
            else if ((int)StripePlans.ENTERPRISE == plan)
                plancost = 249;

            return plancost;
        }

        public static int GetCustomPlanCost(int skuCount)
        {
            int Totalcost = 249;
            double Extra_Count = skuCount / 250;
            int Extra_Cost = (int)Math.Floor(Extra_Count) * 50;
            Totalcost += Extra_Cost;
            return Totalcost;
        }
    }
}
