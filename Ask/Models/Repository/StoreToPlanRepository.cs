using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using WorkV3.Areas.Backend.ViewModels;

namespace WorkV3.Models.Repository
{
    public class StoreToPlanRepository : RepositoryBase<StoreToPlanModel>, Interface.IStoreToPlanRepository<StoreToPlanModel>
    {
        public StoreToPlanRepository()
        {
            SetTableName("StoreToPlan");
        }

        public IEnumerable<StoreToPlanViewModel> GetPlans(long StoreID)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = @" SELECT SP.*, P.PlanName FROM StoreToPlan SP LEFT JOIN StorePlan P ON P.ID = SP.PlanID WHERE StoreID=@StoreID  ";

                return conn.Query<StoreToPlanViewModel>(sql, new { StoreID = StoreID });
            }
        }

        public StoreToPlanModel GetPlansByPlanID(long storeId, long planId)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = @" SELECT Top 1 * FROM StoreToPlan WHERE StoreID=@StoreID and PlanID = @PlanID and IsContractSave = 1 Order By CreateTime Desc";

                return conn.Query<StoreToPlanModel>(sql, new { StoreID = storeId, PlanID = planId }).SingleOrDefault();
            }
        }

        public void SetItemBusiness(long storeToPlanId, IEnumerable<long> business)
        {
            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            sql.AppendLine("DELETE StoreToPlanBusiness WHERE StoreToPlanID = " + storeToPlanId);

            if (business != null && business.Count() > 0)
            {
                string fmt = "Insert StoreToPlanBusiness(StoreToPlanID, BusinessID, Sort) Values(" + storeToPlanId + ", {0}, {1})\n";
                int i = 0;
                foreach (long businessId in business)
                    sql.AppendFormat(fmt, businessId, ++i);
            }

            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                conn.Execute(sql.ToString());
            }
        }

        public IEnumerable<StoreToPlanBusinessModels> GetItemBusiness(long storeToPlanId)
        {
            string sql = $"Select T.BusinessID, M.Name, M.Email From StoreToPlanBusiness T Left Join Member M on M.ID = T.BusinessID Where T.StoreToPlanID = {storeToPlanId}";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                return conn.Query<StoreToPlanBusinessModels>(sql);
            }
        }
    }
}