using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Areas.Backend.Models;
using WorkV3.Models;

namespace WorkV3.ViewModels
{
    public class PointsRecordViewModel
    {
        public string Name { get; set; }
        public decimal Total { get; set; }
        public IEnumerable<PointsModel> Points { get; set; }
    }
}