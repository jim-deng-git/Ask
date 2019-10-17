using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class PaymentCodesModel
    {
        public string PaymentCode { get; set; }
        public string Title { get; set; }
        public int Sort { get; set; }
        public bool IsEnabled { get; set; }

        public static IEnumerable<PaymentCodesModel> GetRegionItems()
        {
            return DataAccess.PaymentCodesDAO.GetEnabledItem();
        }
    }
}