using Mpp.BUSINESS.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mpp.API.Models
{
    public class AuthorizationResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string state { get; set; }
        public string scope { get; set; }
    }

    public class UserProfile
    {
        public string profileId { get; set; }
        public string countryCode { get; set; }
        public string timeZone { get; set; }
        public string currencyCode { get; set; }
        public double dailyBudget { get; set; }
        public accountInfo accountinfo { get; set; }

    }

    public class accountInfo
    {
        public string marketplaceStringId { get; set; }
        public string sellerStringId { get; set; }
    }

    public static class Statics
    {
        public enum RecordType
        {
            Campaign = 1, AdGroup = 2, Keyword = 3, SearchTerm = 4
        }

        public enum ReportStatus
        {
            NOTSET = 0, SET = 1, GET = 2, REFRESH_NOTSET = 3, REFRESH_SET = 4, FAILED_NOTSET = 5
        }

        public enum ResponseStatus
        {
            FAILED = -1, NORECORDS = -2, SUCCESS = 1
        }
    }

    public class DashboardModel
    {
        public List<CampaignModel> campaignModel { get; set; }
        public List<AdGroupModel> adGroupModel { get; set; }
        public List<KeywordModel> keywordModel { get; set; }
    }

}