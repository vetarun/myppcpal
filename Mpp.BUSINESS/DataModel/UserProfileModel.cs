using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mpp.BUSINESS.DataModel
{
    public class UserProfileModel
    {
        public string profileId { get; set; }
        public string countryCode { get; set; }
        public string timeZone { get; set; }
        public string currencyCode { get; set; }
        public double dailyBudget { get; set; }
        public AccountInfo accountinfo { get; set; }
    }

    public class AccountInfo
    {
        public string marketplaceStringId { get; set; }
        public string sellerStringId { get; set; }
    }
}