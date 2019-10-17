using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Areas.Backend.Models;
namespace WorkV3.Areas.Backend.ViewModels
{
    public class AnalysisMemberGroupViewModel
    {
        public GroupModels GroupModel { get; set; }
        public List<MemberModels> GroupMembers { get; set; }
    }
}