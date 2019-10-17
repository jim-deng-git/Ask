using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class EpaperSubRecExcelIndexModel
    {
        public string EpaperTypeName { get; set; }

        public IEnumerable<EpaperSubscribeRecordExcelDayModel> excelDayModel { get; set; }
        public IEnumerable<EpaperSubscribeRecordExcelMonthModel> excelMonthModel { get; set; }
    }
}