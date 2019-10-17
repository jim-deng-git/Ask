using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Dapper;
namespace WorkV3.Models.DataAccess
{
    public class ZonesDAO
    {

        public static List<ZonesModels> GetPageData(long SiteID, long PageNO, bool isShowAll =false)
        {
            string Sql = "Select * From Zones Z where PageNo=@PageNO and SiteID=@SiteID ";
                if (isShowAll == false)
                Sql += " and ShowStatus = 1";
            Sql += "Order by Sort Asc, NO Asc;";

            List<ZonesModels> nLists = new List<ZonesModels>();
            using (var SqlCn = new SqlConnection(WebInfo.Conn))
            {
                var res = SqlCn.Query<ZonesModels>(
                    Sql,
                    new { SiteID = SiteID, PageNO = PageNO });
                nLists = res.ToList();
            }
            return nLists;

        }       
    }
}