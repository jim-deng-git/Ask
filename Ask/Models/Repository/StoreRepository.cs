using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using WorkV3.Models.Interface;

namespace WorkV3.Models.Repository
{
    public class StoreRepository : RepositoryBase<StoreModels>, Interface.IStoreRepository<StoreModels, StoreSearch>
    {
        public StoreRepository()
        {
            SetTableName("Store");
        }

        public IEnumerable<StoreModels> GetPagedData(StoreSearch search, int pageSize, int pageIndex, out int recordCount, bool showNotIssued = true)
        {
            List<string> where = new List<string>();
            Dictionary<string, object> param = new Dictionary<string, object>();
            string whereClause = where.Count > 0 ? $" WHERE {string.Join(" AND ", where)} " : "";
            string orderClause = " order by Sort, ID DESC ";

            string sql = "";
                sql = $@" SELECT * 
                          FROM Store 
                          {whereClause} ";

            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                IEnumerable<StoreModels> retValue = GetPagedData(sql, pageSize, pageIndex, out recordCount, null, orderClause);
                if (retValue != null && retValue.Count() > 0)
                {
                    var items = retValue.ToList();
                    return items;
                }
                return retValue;
            }
        }
        public void Toggle(long id, string columnName = "IsIssue", string specificColumn = "ID")
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = $" Update Store SET {columnName.Replace("'", "")}=case when {columnName.Replace("'", "")}=0 then 1 else 0 end WHERE  {specificColumn.Replace("'", "")}={id} ";
                //WorkLib.WriteLog.Write(true, sql);

                int exeCount = conn.Execute(sql);
                
            }
        }

        public IEnumerable<ShippingToStoreModel> GetShippings(long storeID)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = @" SELECT * 
                                FROM ShippingToStore WHERE StoreID=@StoreID ";
                var items = conn.Query<ShippingToStoreModel>(sql, new { StoreID = storeID });
                var codeItems = WorkV3.Models.DataAccess.ShippingCodesDAO.GetAllItem();
                foreach (ShippingToStoreModel item in items)
                {
                    var codeITem = codeItems.Where(c => c.ShippingCode == item.ShippingCode);
                    if (codeITem != null && codeITem.Count() > 0)
                        item.Title = codeITem.First().Title;
                }
                return items;
            }
        }

        public bool AddShipping(long storeID, WorkV3.Areas.Backend.ViewModels.ShippingsViewModel[] Shippings)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                for (int i = 0; i < Shippings.Length; i++)
                {
                    string isEnabled = Shippings.ElementAt(i).IsEnabled ? "1" : "0";
                    string sql = @" INSERT INTO ShippingToStore (ID, StoreID, ShippingCode, Fee, IsSupport)"
                              + $" VALUES ( {WorkLib.GetItem.NewSN()}, {storeID.ToString()}, '{Shippings.ElementAt(i).Code.ToString()}', { Shippings.ElementAt(i).Fee.ToString() }, {isEnabled}) ";
                    int exeCount = conn.Execute(sql);
                }
                return true;
            }
            return false;
        }
        public bool UpdShipping(long storeID, WorkV3.Areas.Backend.ViewModels.ShippingsViewModel[] Shippings)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string del_sql = $"DELETE ShippingToStore WHERE StoreID={storeID} ";
                conn.Execute(del_sql);

                if (Shippings != null && Shippings.Length > 0)
                {
                    for (int i = 0; i < Shippings.Length; i++)
                    {

                        string isEnabled = Shippings.ElementAt(i).IsEnabled ? "1" : "0";
                        string sql = @" INSERT INTO ShippingToStore (ID, StoreID, ShippingCode, Fee, IsSupport)"
                                  + $" VALUES ( {WorkLib.GetItem.NewSN()}, {storeID.ToString()}, '{Shippings.ElementAt(i).Code.ToString()}', { Shippings.ElementAt(i).Fee.ToString() }, {isEnabled}) ";
                        int exeCount = conn.Execute(sql);
                    }
                }
                else
                    return true;
            }
            return false;
        }
    }
}