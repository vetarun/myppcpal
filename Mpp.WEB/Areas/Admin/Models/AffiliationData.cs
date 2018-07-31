using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mpp.WEB.Areas.Admin.Models
{
    public class AffiliationData
    {
        [Required(ErrorMessage = "Code required!", AllowEmptyStrings = false)]
        public String Code { get; set; }
        [Required(ErrorMessage = "Duration required!", AllowEmptyStrings = false)]
        public int? Duration { get; set; }

        public int? Percent { get; set; }
         
        public Decimal? Amount { get; set; }
        public int? Maxredeem { get; set; }
        public DateTime? Redeemby { get; set; }
    }
}