using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models.DataAccess;

namespace WorkV3.Models
{
    public class ErrorLogModel
    {
        public long ID { get; set; }
        public object Value1 { get; set; }
        public object Value2 { get; set; }
        public object Value3 { get; set; }
        public object Value4 { get; set; }
        public object Value5 { get; set; }
        public object Value6 { get; set; }
        public string Message { get; set; }
        public DateTime LogTime { get; set; }
    }
}