using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using WorkV3.Areas.Backend.ViewModels;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class CardsDAO
    {
        public static List<CardsModels> GetPageData(long SiteID, long PageNO)
        {
            string Sql = "Select C.* from Cards C inner join Zones Z on C.ZoneNO=Z.[No] where Z.PageNo=@PageNO and Z.SiteID=@SiteID;";

            List<CardsModels> nLists = new List<CardsModels>();
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                var res = cn.Query<CardsModels>(
                    Sql,
                    new { SiteID = SiteID, PageNO = PageNO });
                nLists = res.ToList();           
            }          
            return nLists;

        }

        public static List<CardsModels> GetZoneData(long SiteID, long ZoneNo)
        {
            string Sql = "Select C.* from Cards C inner join Zones Z on C.ZoneNO=Z.[No] where Z.SiteID=@SiteID and Z.[No]=@ZoneNo;";

            List<CardsModels> nLists = new List<CardsModels>();
            using (var cn = new SqlConnection(WebInfo.Conn))
            {

                var res = cn.Query<CardsModels>(
                    Sql,
                    new { SiteID = SiteID, ZoneNo = ZoneNo });
                nLists = res.ToList();
            }
            return nLists;

        }

        public static List<CardsViewModel> GetZoneByPageNo(long SiteID, long PageNo)
        {
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                string sql = @"SELECT C.*, T.[ICON] AS TypeIcon, T.[Title] AS TypeTitle, T.[EditURLAction] AS TypeEditURLAction FROM [Cards] C
                                LEFT JOIN [CardsType] T ON T.[Code] = C.[CardsType]                            
                                LEFT JOIN [Zones] Z ON C.[ZoneNo] = Z.[No]
                                WHERE Z.[SiteID] = @SiteID AND Z.[PageNo] = @PageNo
                                ORDER BY Z.[Sort], C.[SN];";
                return cn.Query<CardsViewModel>(sql, new { SiteID = SiteID, PageNo = PageNo }).ToList();
            }
        }

        public static List<CardsViewModel> GetBySiteID(long SiteID)
        {
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                string sql = @"SELECT C.*, P.MenuID, T.[ICON] AS TypeIcon, T.[Title] AS TypeTitle, T.[EditURLAction] AS TypeEditURLAction FROM [Cards] C
                                LEFT JOIN [CardsType] T ON T.[Code] = C.[CardsType]                            
                                LEFT JOIN [Zones] Z ON C.[ZoneNo] = Z.[No]
                                LEFT JOIN [Pages] P ON Z.[PageNo] = P.[No]
                                WHERE Z.[SiteID] = @SiteID
                                ORDER BY Z.[Sort], C.[SN];";
                return cn.Query<CardsViewModel>(sql, new { SiteID = SiteID }).ToList();
            }
        }

        public static CardsModels GetByNo(long No)
        {
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                string sql = @"SELECT * FROM Cards WHERE No = @No;";
                return cn.Query<CardsModels>(sql, new { No = No }).FirstOrDefault();
            }
        }

        public static int UpdateTitleAndDescriptions(long No, string Title, string Descriptions)
        {
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                var sqlCommand = "UPDATE [Cards] SET Title = @Title, Descriptions = @Descriptions WHERE No = @No;";
                return cn.Execute(sqlCommand, new { No = No, Title = Title, Descriptions = Descriptions });
            }
        }

        public static int UpdateStatus(long No, int Status)
        {
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                var sqlCommand = "UPDATE [Cards] SET Status = @Status WHERE No = @No;";
                return cn.Execute(sqlCommand, new { No = No, Status = Status });
            }
        }

        public static void SetCardInfo(CardsModels card)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("Cards");
            tableObj.GetDataFromObject(card);

            string sql = $"Select 1 From Cards Where No = { card.No } AND Lang = '{ card.Lang.Replace("'", "''") }' AND Ver = { card.Ver }";
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            }
            else
            {
                string[] removeFields = { "No", "Lang", "Ver", "Creator", "CreateTime" };
                foreach (string field in removeFields)
                    tableObj.Remove(field);

                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                SQLData.ParameterCollection keys = new SQLData.ParameterCollection();
                keys.Add("@No", card.No);
                keys.Add("@Lang", card.Lang);
                keys.Add("@Ver", card.Ver);

                tableObj.Update(keys);
            }
        }

        public static void Sort(IEnumerable<Common.SortItem> items) {
            if (items == null || items.Count() == 0)
                return;

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            string sql = $"Select No From Zones Where PageNo = (Select PageNo From Zones Where No = { items.First().ID }) AND NO Not In ({ string.Join(", ", items.Select(item => item.ID)) }) Order By IsNull(Sort, { byte.MaxValue })";
            List<long> ids = db.GetDataTable(sql).AsEnumerable().Select(dr => (long)dr["No"]).ToList();
            
            foreach(Common.SortItem item in items.OrderBy(m => m.Index)) {
                int index = item.Index - 1;
                if (index < 0)
                    index = 0;
                else if (index > ids.Count)
                    index = ids.Count;
                ids.Insert(index, item.ID);
            }

            string fmt = "Update Zones Set Sort = {0} Where No = {1}\n";
            System.Text.StringBuilder sortSql = new System.Text.StringBuilder();
            for(int i = 0, len = ids.Count; i < len; ++i) {
                sortSql.AppendFormat(fmt, i + 1, ids[i]);
            }

            db.ExecuteNonQuery(sortSql.ToString());
        }
    }
}