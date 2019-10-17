using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WorkV3.Models.DataAccess
{
    public class PointsDAO
    {
        public static IEnumerable<PointsModel> GetItems(long siteId, long memberShipID, int pageSize, int pageIndex, out int recordCount)
        {
            List<PointsModel> items = new List<PointsModel>();

            string sql = "Select * From Points Where {0} Order By CreateTime desc";
            List<string> where = new List<string>();
            where.Add(" SiteID = " + siteId + " and MemberShipID = " + memberShipID);

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetPageData(string.Format(sql, string.Join(" And ", where)), pageSize, pageIndex, out recordCount);

            foreach (DataRow dr in datas.Rows)
            {
                int pointType = 0;
                if(string.IsNullOrWhiteSpace(dr["PointType"].ToString()))
                {
                    if (dr["Point"].ToString().StartsWith("-"))
                    {
                        pointType = 1;
                    }
                }
                else
                {
                    pointType = (int)dr["PointType"];
                }

                items.Add(new PointsModel
                {
                    ID = (long)dr["ID"],
                    SiteID = (long)dr["SiteID"],
                    MemberShipID = (long)dr["MemberShipID"],
                    Remark = dr["Remark"].ToString().Trim(),
                    Description = dr["Description"].ToString().Trim(),
                    Point = (decimal)dr["Point"],
                    IsManually = (bool)dr["IsManually"],
                    CreateTime = (DateTime)dr["CreateTime"],
                    PointType = pointType
                });
            }

            return items;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="IsManually">是否手動新增</param>
        public static void SetItem(PointsModel item, bool IsManually = false)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("Points");

            tableObj.GetDataFromObject(item);

            DateTime now = DateTime.Now;
            string sql = "Select 1 From Points Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;

            tableObj["SiteID"] = item.SiteID;
            tableObj["MemberShipID"] = item.MemberShipID;
            tableObj["Remark"] = string.IsNullOrEmpty(item.Remark) ? "" : item.Remark;
            tableObj["Description"] = string.IsNullOrEmpty(item.Description) ? "" : item.Description;
            tableObj["Point"] = item.Point;
            tableObj["IsManually"] = IsManually;
            tableObj["PointType"] = item.PointType;

            if (isNew)
            {
                if (item.Creator == 0)
                    tableObj["Creator"] = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = now;

                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("SiteID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                Common.Member curUser = Common.Member.Current;
                tableObj["Modifier"] = curUser == null ? Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id : curUser.ID;
                tableObj["ModifyTime"] = now;

                tableObj.Update(item.ID);
            }
        }

        public static int DeletePoints(IEnumerable<long> ids)
        {
            if (ids?.Count() == 0)
                return 0;

            string sql =
                "Delete [Points] Where ID In ({0})";

            int num = 0;
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                num = conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }

            return num;
        }

        public static decimal GetPointsTotal(long siteId,long membershipId)
        {
            decimal Total = 0;
            string Sql = $"Select SUM(Point) as Total from Points where SiteID = {siteId} and MemberShipID = {membershipId}";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.SelectObject Obj = db.GetSelectObject(Sql);
            if (Obj["Total"] != null && !string.IsNullOrEmpty(Obj["Total"].ToString()))
            {
                Total = decimal.Parse(Obj["Total"].ToString());
            }
            return Total;
        }
    }
}