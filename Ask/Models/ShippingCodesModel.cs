using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class ShippingCodesModel
    {
        public string ShippingCode { get; set; }
        public int ShippingRegion { get; set; }
        public string Title { get; set; }
        public int Sort { get; set; }
        public bool IsEnabled { get; set; }
        
        public static IEnumerable<ShippingCodesModel> GetRegionItems(int regionType)
        {
            return DataAccess.ShippingCodesDAO.GetEnabledItem().Where(p=>p.ShippingRegion==regionType);
        }
        public static IEnumerable<ShippingCodesModel> GetItems()
        {
            return DataAccess.ShippingCodesDAO.GetEnabledItem();
        }
    }
}