using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using WorkV3.Common;
using WorkV3.Areas.Backend.Models;
using WorkLib;
using Dapper;
using System.Data.SqlClient;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class AdsStatisticsLabelDAO
    {
        public static AdsStatisticsLabelsModel GetItem(long id)
        {
            return CommonDA.GetItem<AdsStatisticsLabelsModel>("AdsStatisticsLabels", id);
        }
        
        public static int Delete(long id) {
            long[] ids = { id };
            return Delete(ids);
        }

        public static int Delete(IEnumerable<long> ids) {
            if (ids?.Count() == 0)
                return 0;

            string sql =
                "Delete AdsStatisticsLabels Where ID In ({0})\n";

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                num = conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }

            ParagraphDAO.DeleteBySourceNo(ids);

            return num;
        }

        public static IEnumerable<AdsStatisticsLabelsModel> GetItems(DateTime startDate, DateTime endDate)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = @" SELECT * 
                                FROM AdsStatisticsLabels
                                WHERE LabelDate BETWEEN @StartDate AND @EndDate 
                                AND ShowStatus = 1 ";
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("@StartDate", startDate.ToString("yyyy/MM/dd"));
                param.Add("@EndDate", endDate.ToString("yyyy/MM/dd"));

                return conn.Query<AdsStatisticsLabelsModel>(sql, param);
            }
        }

        public static void SetItem(AdsStatisticsLabelsModel item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("AdsStatisticsLabels");
            tableObj.GetDataFromObject(item);
            bool isNew = false;
            string sql = "Select 1 From AdsStatisticsLabels Where ID = " + item.ID;
            isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Title"] = item.Title;
                tableObj["LabelDate"] = item.LabelDate;
                tableObj["LabelColor"] = item.LabelColor;
                tableObj["ShowStatus"] = item.ShowStatus;
                tableObj["Creator"] = item.Creator;
                tableObj["CreateTime"] = item.CreateTime;
                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");
                tableObj["Title"] =  item.Title;
                tableObj["LabelDate"] = item.LabelDate;
                tableObj["ShowStatus"] = item.ShowStatus;
                tableObj["LabelColor"] = item.LabelColor;
                tableObj["Modifier"] = item.Modifier;
                tableObj["ModifyTime"] = item.ModifyTime;
                tableObj.Update(item.ID);
            }
        }
        public static void SetItemStatus(long ID, bool ShowStatus)
        {
            AdsStatisticsLabelsModel item = new AdsStatisticsLabelsModel();
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@ID", ID);
            paraList.Add("@ShowStatus", ShowStatus);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string updateSQL = " UPDATE AdsStatisticsLabels SET ShowStatus=@ShowStatus WHERE ID=@ID ";
            db.ExecuteNonQuery(updateSQL, paraList);
        }
        public static IEnumerable<AdsStatisticsLabelsModel> GetItems(AdsStatisticsLabelsSearchModel search, int pageSize, int pageIndex, out int recordCount) {
            List<AdsStatisticsLabelsModel> items = new List<AdsStatisticsLabelsModel>();
            if (search == null) {
                recordCount = 0;
                return items;
            }

            string sql = "Select *  From AdsStatisticsLabels  Where 1=1 {0} Order By LabelDate Desc";

            List<string> where = new List<string>();

            if (!string.IsNullOrWhiteSpace(search.Keyword)) {
                where.Add(string.Format(" ( Title Like N'%{0}%' )", search.Keyword.Replace("'", "''")));
            }

            if(search.StartDate.HasValue) 
                where.Add(string.Format(" LabelDate >= '{0:yyyy/MM/dd HH:mm}' ", search.StartDate));

            if (search.EndDate.HasValue)
                where.Add(string.Format(" LabelDate <= '{0:yyyy/MM/dd HH:mm}' ", search.EndDate));
            
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetPageData(string.Format(sql, where.Count > 0? " And " + string.Join(" And ", where):""), pageSize, pageIndex, out recordCount);
            if (datas != null)
            {
                foreach (DataRow dr in datas.Rows)
                {
                    items.Add(new AdsStatisticsLabelsModel
                    {
                        ID = (long)dr["ID"],
                        Title = dr["Title"].ToString().Trim(),
                        LabelColor = dr["LabelColor"].ToString().Trim(),
                        LabelDate = (DateTime)dr["LabelDate"],
                        ShowStatus = (bool)dr["ShowStatus"],
                        CreateTime = DateTime.Parse(dr["CreateTime"].ToString()),
                        Creator = (long)dr["Creator"],
                        ModifyTime = DateTime.Parse(dr["ModifyTime"].ToString()),
                        Modifier = (long)dr["Modifier"]
                    });
                }
            }

            return items;
        }

        public static IEnumerable<AdsStatisticsLabelsModel> GetShowLabelLine(DateTime startDate, DateTime endDate)
        {
            List<AdsStatisticsLabelsModel> items = new List<AdsStatisticsLabelsModel>();
            string sql = "Select *  From AdsStatisticsLabels  Where ShowStatus=1 AND LabelDate>=@StartDate AND LabelDate<=@EndDate Order By LabelDate Asc";

            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@StartDate", startDate);
            paraList.Add("@EndDate", endDate);

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql, paraList);
            if (datas != null)
            {
                foreach (DataRow dr in datas.Rows)
                {
                    items.Add(new AdsStatisticsLabelsModel
                    {
                        ID = (long)dr["ID"],
                        Title = dr["Title"].ToString().Trim(),
                        LabelColor = dr["LabelColor"].ToString().Trim(),
                        LabelDate = (DateTime)dr["LabelDate"],
                        ShowStatus = (bool)dr["ShowStatus"],
                        CreateTime = DateTime.Parse(dr["CreateTime"].ToString()),
                        Creator = (long)dr["Creator"],
                        ModifyTime = DateTime.Parse(dr["ModifyTime"].ToString()),
                        Modifier = (long)dr["Modifier"]
                    });
                }
            }

            return items;
        }

        public static int DeleteItems(IEnumerable<long> ids)
        {
            if (ids?.Count() == 0)
                return 0;

            string sql =
                //"Delete [Member] Where ID In ({0})";
                "delete AdsStatisticsLabels where id IN  ({0})";

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                num = conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }

            return num;
        }
    }
}