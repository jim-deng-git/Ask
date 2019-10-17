using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WorkV3.Models
{
    public class ApiLogsModels
    {
        public long  ID { get; set; }
        public int ApiType { get; set; }
        public string ApiMethod { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string PostData { get; set; }
        public string GetData { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime? ResponseTime { get; set; }
        public string LogMessage { get; set; }
    }
}