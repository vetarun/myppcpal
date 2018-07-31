using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mpp.WEB.Models
{
    public class StatementViewModel
    {
        public string Id { get; set; }
        public string Date { get; set; }
        public string Description {get;set;}
        public string Price { get; set; }
        public string Period { get; set; }
    }
}