using Dapper;
using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class BankDAO
    {
        public static IEnumerable<BankModel> GetBanks()
        {
            return CommonDA.GetAllItem<BankModel>("Bank");
        }

        public static List<BankModel> GetBank()
        {
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = $"Select * from Bank where ParentID is null";
                return conn.Query<BankModel>(sql).ToList();
            }
        }

        public static List<BankModel> GetBranch(string parentID)
        {
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = $"Select * from Bank where ParentID = '{parentID}'";
                return conn.Query<BankModel>(sql).ToList();
            }
        }
    }
}