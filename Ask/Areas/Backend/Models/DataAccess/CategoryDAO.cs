using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class CategoryDAO
    {
        public static List<CategoryModels> GetItems(string Type, string Ids = "")
        {
            List<CategoryModels> items = new List<CategoryModels>();

            string sql = "SELECT * FROM Categories WHERE 1=1 ";
            if (!string.IsNullOrEmpty(Type))
            {
                sql += $"AND (Type='" + Type.Replace(",", "','") + "') ";
            }
            if (!string.IsNullOrWhiteSpace(Ids))
            {
                sql += $"AND ID in ({Ids}) ";
            }
            
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            foreach (DataRow dr in datas.Rows)
            {
                CategoryModels m = new CategoryModels();
                m.ID = (long)dr["ID"];
                m.Type = dr["Type"].ToString().Trim();
                m.Title = dr["Title"].ToString().Trim();
                m.RemarkText = dr["RemarkText"].ToString().Trim();
                m.ShowStatus = Convert.ToBoolean(dr["ShowStatus"].ToString());
                m.Icon = dr["Icon"].ToString().Trim();
                m.Sort = Convert.ToInt32(dr["Sort"].ToString());
                m.Image = dr["Image"].ToString().Trim();
                m.MemberSession = (!string.IsNullOrWhiteSpace(dr["MemberSession"].ToString()) ? Convert.ToInt32(dr["MemberSession"].ToString()) : 0) ;
                items.Add(m);
            }

            return items;
        }

        public static CategoryModels GetItem(long id)
        {
            CategoryModels m = CommonDA.GetItem<CategoryModels>("[Categories]", id);
            return m;
        }

        public static IEnumerable<CategoryModels> GetIssueItems(string Type)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = "Select * From Categories Where Type='{0}' And ShowStatus = 1";
                return conn.Query<CategoryModels>(string.Format(sql, Type.Replace(",", "','")));
            }
        }

        public static int DeleteCategories(IEnumerable<long> ids)
        {
            if (ids?.Count() == 0)
                return 0;

            string sql =
                "Delete [Categories] Where ID In ({0}) and PresetIdentity not in ({1})"; //20181025 nina 預設的身分不刪除

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                num = conn.Execute(string.Format(sql, string.Join(", ", ids), (int)PresetIdentity.一般會員));
            }

            return num;
        }

        public static void SetItem(CategoryModels item)
        {
            long creator = MemberDAO.SysCurrent.Id;
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = "Select 1 From [Categories] Where ID = " + item.ID;
            item.Icon = item.Icon == null ? "" : item.Icon;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
                {

                    sql = $"IF not EXISTS (SELECT 1 FROM Categories WHERE ID = @ID)";
                    sql += $" INSERT INTO [Categories]([ID],[Type],[Title],[RemarkText],[ShowStatus],[Icon],[Sort],[Creator],[CreateTime] ,[Modifier],[ModifyTime],[Image],[MemberSession],[PresetIdentity]) VALUES({ WorkLib.GetItem.NewSN() },@Type,@Title,@RemarkText, @ShowStatus, @Icon, @Sort ,{creator}, getdate(),{creator}, getdate(),@Image,@MemberSession,@PresetIdentity) ";
                    conn.Execute(sql, item);
                }
            }
            else
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
                {
                    sql = $"UPDATE Categories SET [Title]=@Title,[RemarkText]=@RemarkText, [ShowStatus]=@ShowStatus,[Icon]=@Icon,[Sort]=@Sort,[Modifier]={creator},ModifyTime=getdate(),[Image]=@Image,[MemberSession]=@MemberSession";
                    sql += " WHERE ID=@ID ";
                    conn.Execute(sql, item);
                }
            }
        }

        public static void ChangeStatus(long id)
        {
            string sql = "Update Categories Set ShowStatus = CASE WHEN ShowStatus=0 THEN 1 ELSE 0 END Where ID = " + id;
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
        }
    }
    public enum CategoryType
    {
        Identity,
        Favority,
        Career,
        Marriage,
        Education,
        ChildrenStatus,
        MilitaryService
    }

    public enum MemberSession
    {
        一日期 = 1,
        一週期 = 2,
        雙週期 = 3,
        一月期 = 4,
        雙月期 = 5,
        一季期 = 6,
        四月期 = 7,
        半年期 = 8,
        一年期 = 9,
        不限制 = 10
    }

    public enum PresetIdentity
    {
        一般會員 = 1
    }
}