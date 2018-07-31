using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mpp.WEB.Models
{
    public class Feedback
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public string Content { get; set; }
        public HttpPostedFileBase Attachment { get; set; }


    }
}