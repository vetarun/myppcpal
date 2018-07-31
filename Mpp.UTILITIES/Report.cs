using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.UTILITIES
{
    public class Report
    {
        public Int32 ReportID { get; set; }
        public Int32 UserId { get; set; }
        public DateTime ReportStartDate { get; set; }
        public DateTime ReportEndDate { get; set; }
        public Int32 Days { get; set; }
        public DateTime ReportDate { get; set; }  //upload or download 
    }

    //
    // Summary:
    //     Possible results from Selenium Report Downloads
    public enum BulkReportStatus
    {
        //
        // Summary:
        //     Download was successful
        Success = 0,
        //
        // Summary:
        //     Report is in processing stage
        Processing = 1,
        //
        // Summary:
        //     Report is not found
        NotFound = 2,
        //
        // Summary:
        //     Something went wrong
        Failure = 3
    }
}
