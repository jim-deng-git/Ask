using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class CustomCompanyContactModel
    {
        public long ID { get; set; }
        public long CustomCompanyID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Sort { get; set; }
    }
}