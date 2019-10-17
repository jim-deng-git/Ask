using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using WorkV3.Areas.Backend.Models;

namespace WorkV3.Models.Repository
{
    public class CompanyRepository : RepositoryBase<CompanyModel>, Interface.ICompanyRepository
    {
        public CompanyRepository()
        {
            SetTableName("Company");
        }

        #region 休假日、國定假日
        /// <summary>
        /// 取得日期資料
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public HolidaySetModels GetHoliday(string date)
        {
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = $"Select * From HolidaySet Where [Date]= '{date}' ";
                
                return conn.Query<HolidaySetModels>(sql).SingleOrDefault();
            }
        }

        /// <summary>
        /// 取年度的休假資料
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<HolidaySetModels> GetHolidaySetList(string Year, int? type)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"Select [Date] = convert(varchar(10),[Date],120) From HolidaySet Where YEAR([Date])= {Year} ";
                if (type.HasValue)
                    sql += $" And Type = {type}";
                return conn.Query<HolidaySetModels>(sql).ToList();
            }
        }

        /// <summary>
        /// 更新休假日設定
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="dateList"></param>
        public void UpdateHolidaySet(string Year, List<HolidaySetModels> dateList, int type)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendFormat("delete HolidaySet where YEAR([Date])={0} And Type = {1};", Year, type);
            foreach (HolidaySetModels model in dateList)
                strSQL.AppendFormat("insert into [HolidaySet] values('{0}','', {1});", model.Date.ToShortDateString(), type);

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                conn.Execute(strSQL.ToString());
            }
        }

        /// <summary>
        /// 判断日期是否为假日
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool IsHoliday(string date, int type)
        {
            string strSQL = "select 1 from HolidaySet where [Date]=@date And Type = @type;";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.ParameterCollection para = new SQLData.ParameterCollection();
            para.Add("@date", date);
            para.Add("@type", type);
            return db.GetDataTable(strSQL, para).Rows.Count > 0;
        }

        /// <summary>
        /// 取得日期是平日、休假日或國定假日
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public int CheckHolidayType(string date)
        {
            var dateItem = GetHoliday(date);

            if (dateItem == null)
                return (int)HolidayType.WeekDay;
            else
                return dateItem.Type;
        }
        #endregion

        public void Sort(IEnumerable<Common.SortItem> items, int? parentId)
        {
            string where = "ParentID Is Null";
            if (parentId.HasValue)
                where = $"ParentID = {parentId}";

            Areas.Backend.Models.DataAccess.CommonDA.Sort("Company", items, where);
        }

        public void ToggleIssue(long id)
        {
            Areas.Backend.Models.DataAccess.CommonDA.ToggleIssue("Company", id);
        }
    }
}