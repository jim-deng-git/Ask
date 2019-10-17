using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class SiteMailSettingModel
    {
        public long SiteID { get; set; }
        public string Sender { get; set; }
        public string SmtpServer { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
    }
}