using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mpp.BUSINESS.DataModel
{
    public class ReportResponseModel
    {
        public string snapshotId { get; set; }
        public string reportId { get; set; }
        public string recordType { get; set; }
        public string status { get; set; }
        public string statusDetails { get; set; }
        public string location { get; set; }
        public int fileSize { get; set; }
    }

    public class KeywordResponse
    {
        public long? keywordId { get; set; }
        public string code { get; set; }
        public string details { get; set; }
    }
}