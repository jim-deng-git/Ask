using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Common;
using WorkV3.Models;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class ShippingsViewModel
    {
        public bool IsEnabled { get; set; }
        public string Code { get; set; }
        public float Fee { get; set; }
    }
}