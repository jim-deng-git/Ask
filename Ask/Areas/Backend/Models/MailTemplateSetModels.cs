using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class MailTemplateSetModels
    {
        public string TemplateName { get; set; }
        public string MailTitle { get; set; }
        public string MailContent { get; set; }
        public string MailFromName { get; set; }
        public string MailFromAddress { get; set; }
        public string AttFiles { get; set; }
        public string AttShowFiles { get; set; }
    }


}