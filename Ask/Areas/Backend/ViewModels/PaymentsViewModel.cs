using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class PaymentsViewModel
    {
        public bool IsEnabled { get; set; }
        public string Code { get; set; }
        public float Fee { get; set; }
    }
}