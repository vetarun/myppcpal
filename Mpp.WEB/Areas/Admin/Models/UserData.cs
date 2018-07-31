using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mpp.WEB.Areas.Admin.Models
{
    public class UserData
    {

        [Required(ErrorMessage = "Type Required")]
        public int Type { get; set; }
        [Required(ErrorMessage = "Email required!", AllowEmptyStrings = false)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Email ID not valid!")]
        public String Email { get; set; }
        [Required(ErrorMessage = "FirstName required!", AllowEmptyStrings = false)]
        public String FirstName { get; set; }
        public String LastName { get; set; }
        [Required(ErrorMessage = "Password required!", AllowEmptyStrings = false)]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public String Password { get; set; }
    }
}