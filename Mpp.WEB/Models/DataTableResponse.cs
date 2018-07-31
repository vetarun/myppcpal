using Mpp.UTILITIES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mpp.WEB.Models
{
    public class DataTableResponse:PagingOptions
    {
        public DataTableResponse()
        {
            Headers = new List<DataTableHeaders>();
        }
        public List<DataTableHeaders> Headers { get; set; }
        public List<object> Data { get; set; }   
        public int Total { get; set; }
    }
}