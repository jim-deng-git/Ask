using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using WorkV3.Common;
using WorkV3.Models;
using WorkLib;
using Dapper;

namespace WorkV3.Models.DataAccess
{
    public class StoreDAO
    {
        public static StoreModels GetItem(long id)
        {
            return CommonDA.GetItem<StoreModels>("Store", id);
        }


        public static List<StoreModels> GetAllItem(long SiteID)
        {
            var storeList = CommonDA.GetAllItem<StoreModels>("Store");
            if (storeList != null && storeList.Count() > 0)
            {
                var resultList = storeList.Where(s => s.SiteID == SiteID);
                return resultList.ToList();
            }
            return null;
        }
    }
}