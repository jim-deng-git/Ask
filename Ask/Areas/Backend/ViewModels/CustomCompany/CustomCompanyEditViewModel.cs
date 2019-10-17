using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models;

namespace WorkV3.Areas.Backend.ViewModels.CustomCompany
{
    public class CustomCompanyEditViewModel
    {
        public CustomCompanyModel Company { get; set; }
        public bool IsExit { get; set; }
        public string ModifierName { get; set; }
        public IEnumerable<CustomCompanyContactModel> Contacts { get; set; }
    }
}