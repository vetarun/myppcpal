using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mpp.WEB.Models
{
    public class ProfilesViewModel
    {
        public string ProfileId { get; set; }

        public string SellerStringId { get; set; }

        public SelectList ProfileList { get; set; }
    }
}