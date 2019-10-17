using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Data.SqlClient;

namespace WorkV3.Models.DataAccess
{
    public class PaymentCodesDAO
    {
        public static IEnumerable<PaymentCodesModel> GetAllItem()
        {
            return CommonDA.GetAllItem<PaymentCodesModel>("PaymentCodes").OrderBy(p=>p.Sort);
        }


        public static IEnumerable<PaymentCodesModel> GetEnabledItem()
        {
            return CommonDA.GetAllItem<PaymentCodesModel>("PaymentCodes").Where(p=>p.IsEnabled);
        }

    }
}