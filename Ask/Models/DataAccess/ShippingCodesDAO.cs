using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Data.SqlClient;

namespace WorkV3.Models.DataAccess
{
    public class ShippingCodesDAO
    {
        public static IEnumerable<ShippingCodesModel> GetAllItem()
        {
            return CommonDA.GetAllItem<ShippingCodesModel>("ShippingCodes").OrderBy(p=>p.Sort);
        }


        public static IEnumerable<ShippingCodesModel> GetEnabledItem()
        {
            return CommonDA.GetAllItem<ShippingCodesModel>("ShippingCodes").Where(p=>p.IsEnabled);
        }

    }
}