using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using WorkV3.Areas.Backend.Models.Extensions;
using WorkV3.Areas.Backend.ViewModels;
using WorkV3.Common;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class AdvertisementDAO
    {
        /// <summary>
        /// 查詢廣告主 列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<AdvertisersModel> GetAdvertisers(int pageSize, int pageIndex,long siteId, out int recordCount)
        {
            List<AdvertisersModel> list = new List<AdvertisersModel>();
            string sql = $"Select * From Advertisers where SiteID={siteId} Order by Sort";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetPageData(sql, pageSize, pageIndex, out recordCount);
            foreach (DataRow dr in datas.Rows)
            {
                list.Add(new AdvertisersModel
                {
                    ID = (long)dr["ID"],
                    SiteID = (long)dr["SiteID"],
                    CompanyName = dr["CompanyName"].ToString().Trim(),
                    ContactPerson = dr["ContactPerson"].ToString().Trim(),
                    Gender = dr["Gender"] == null ? "" : dr["Gender"].ToString().Trim(),
                    JobTitle = dr["JobTitle"] == null ? "" : dr["JobTitle"].ToString().Trim(),
                    ContactPhone = dr["ContactPhone"].ToString().Trim(),
                    Email = dr["Email"] == null ? "" : dr["Email"].ToString().Trim(),
                    WebSite = dr["WebSite"] == null ? "" : dr["WebSite"].ToString().Trim(),
                    Description = dr["Description"] == null ? "" : dr["Description"].ToString().Trim(),
                    IsIssue = (bool)dr["IsIssue"],
                    Sort = (int)dr["Sort"]
                });
            }
            return list;
        }
        public static IEnumerable<AdvertisersModel> GetAllAdvertisers()
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = " SELECT * FROM Advertisers ORDER BY Sort ";

                return conn.Query<AdvertisersModel>(sql);
            }
        }
        /// <summary>
        /// 儲存廣告主
        /// </summary>
        /// <param name="item"></param>
        public static void SetAdvertisersItem(AdvertisersModel item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("Advertisers");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From Advertisers Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("SiteID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                tableObj["Modifier"] = MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                tableObj.Update(item.ID);
            }
        }
        /// <summary>
        /// 刪除廣告主
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static int AdvertisersDelete(IEnumerable<long> ids)
        {
            if (ids.Count() > 0)
            {
                //檢查欲刪除廣告主是否已經被使用，如有則排除
                List<long> used = new List<long>();
                for (int i = 0; i < ids.Count(); i++)
                {
                    if (CheckAdvertisersUsed(ids.ElementAt(i)))
                    {
                        used.Add(ids.ElementAt(i));
                    }
                }
                ids = ids.Where(m => !used.Contains(m));
            }


            if (ids?.Count() == 0)
                return 0;

            string sql = "Delete From Advertisers Where ID In ({0}) ";

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                num = conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }

            return num;
        }
        /// <summary>
        /// 檢查"廣告主"資訊是否已經被使用
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static bool CheckAdvertisersUsed(long code)
        {
            if (code == 0)
                return false;

            string sql = $"select ID from AdsCustomize where Advertisers_ID='{ code }'";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                var result = conn.Query<long>(sql);
                return result.Any();
            }
        
            //SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            //DataTable datas = db.GetDataTable(sql);
            //if (datas.Rows.Count > 0)
            //    return true;
            //else
            //    return false;
        }
        /// <summary>
        /// 變更刊登狀態
        /// </summary>
        /// <param name="id"></param>
        /// <param name="TableName"></param>
        public static void ToggleIssue(long id,string TableName)
        {
            CommonDA.ToggleIssue(TableName, id);
        }
        /// <summary>
        /// 修改排序
        /// </summary>
        /// <param name="items"></param>
        /// <param name="TableName"></param>
        /// <param name="where"></param>
        public static void SortEdit(IEnumerable<SortItem> items, string TableName,string where = "")
        {
            CommonDA.Sort(TableName, items, where);
        }
        /// <summary>
        /// 查詢廣告主 單筆資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static AdvertisersModel GetAdvertisersItem(long id)
        {
            return CommonDA.GetItem<AdvertisersModel>("Advertisers", id);
        }
        /// <summary>
        /// 查詢廣告區 單筆資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static AdvertisementModel GetAdvertisementItem(long id)
        {
            return CommonDA.GetItem<AdvertisementModel>("Advertisement", id);
        }
        /// <summary>
        /// 儲存廣告區
        /// </summary>
        /// <param name="item"></param>
        public static void SetAdvertisementItem(AdvertisementModel item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("Advertisement");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From Advertisement Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("SiteID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                tableObj["Modifier"] = MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                tableObj.Update(item.ID);
            }
        }
        /// <summary>
        /// 查詢廣告區 列表
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<AdvertisementModel> GetAdvertisement(AdvertisementSearchModel search,int pageSize, int pageIndex,long SiteID, out int recordCount)
        {
            List<AdvertisementModel> list = new List<AdvertisementModel>();
            recordCount = 0;

            if (search == null)
                return list;

            StringBuilder sb = new StringBuilder();
            if(search.AdvertisementType != null)
            {
                var type = search.AdvertisementType;
                for (int i = 0; i < type.Count(); i++)
                {
                    type[i] = "'" + type[i] + "'";
                }
                sb.Append(type.Count() != 0 ? string.Format(" and Type in ({0})", string.Join(", ",type)) : string.Empty);
            }
            if (!string.IsNullOrEmpty(search.keyword))
            {
                sb.Append(string.Format(" and AreaName like N'%{0}%'", search.keyword.Replace("'", "''")));
            }
            string sql = string.Format("Select * From Advertisement where SiteID="+ SiteID + " {0} Order by CreateTime desc", sb.ToString());
            
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetPageData(sql, pageSize, pageIndex, out recordCount);

            if(datas == null)
                return list;

            foreach (DataRow dr in datas.Rows)
            {
                list.Add(new AdvertisementModel
                {
                    ID = (long)dr["ID"],
                    //Advertisers_ID = dr["Advertisers_ID"] as long?,
                    AreaName = dr["AreaName"].ToString().Trim(),
                    IsUseForComputer = (bool)dr["IsUseForComputer"],
                    ComputerHeight = dr["ComputerHeight"] as int?,
                    ComputerWidth = dr["ComputerWidth"] as int?,
                    IsUseForPhone = (bool)dr["IsUseForPhone"],
                    PhoneHeight = dr["PhoneHeight"] as int?,
                    PhoneWidth = dr["PhoneWidth"] as int?,
                    Type = dr["Type"].ToString().Trim(),
                    Remark = dr["Remark"] != null? dr["Remark"].ToString().Trim() : string.Empty,
                    ComputerThirdPartyEmbedCode = dr["ComputerThirdPartyEmbedCode"] != null ? dr["ComputerThirdPartyEmbedCode"].ToString().Trim() : string.Empty,
                    PhoneThirdPartyEmbedCode = dr["PhoneThirdPartyEmbedCode"] != null ? dr["PhoneThirdPartyEmbedCode"].ToString().Trim() : string.Empty,
                    ThirdPartyLink = dr["ThirdPartyLink"] != null ? dr["ThirdPartyLink"].ToString().Trim() : string.Empty
                    //DisplayWay = dr["DisplayWay"] != null ? dr["DisplayWay"].ToString().Trim() : string.Empty,
                });
            }
            return list;
        }
        /// <summary>
        /// 刪除廣告區
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static int AdvertisementDelete(IEnumerable<long> ids)
        {
            if (ids?.Count() == 0)
                return 0;

            string sql = @"Delete From Advertisement Where ID In ({0}) 
                           Delete From AdsCustomize Where Advertisement_ID In ({0}) 
                           Delete From AdsDisplayAreaSet Where Advertisement_ID In ({0})";

            string query_custom_id = "select ID from AdsCustomize Where Advertisement_ID In ({0})";
            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                var query = conn.ExecuteReader(string.Format(query_custom_id, string.Join(", ", ids)));
                DataTable dt = new DataTable();
                dt.Load(query);
                List<long> custom_ids = new List<long>();
                if (dt.Rows.Count>0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        custom_ids.Add((long)dr["ID"]);
                    }
                    sql += @"
                           Delete From AdsCustomizeAccountSet Where AdsCustomize_ID In ({1}) 
                           Delete From AdsCustomizeToLink Where AdsCustomize_ID In ({1}) 
                           Delete From AdsCustomizeToVideo Where AdsCustomize_ID In ({1}) 
                                ";
                    num = conn.Execute(string.Format(sql, string.Join(", ", ids),string.Join(", ", custom_ids)));
                }else
                    num = conn.Execute(string.Format(sql, string.Join(", ", ids)));

            }

            return num;
        }
        /// <summary>
        /// 刪除刊登費用
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static int AccountDel(IEnumerable<long> ids)
        {
            if (ids?.Count() == 0)
                return 0;

            string sql = "Delete From AdsCustomizeAccountSet Where ID In ({0}) ";

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                num = conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }

            return num;
        }
        /// <summary>
        /// 透過廣告區ID查詢顯示設定筆數(一般)
        /// </summary>
        /// <param name="Advertisement_ID"></param>
        /// <returns></returns>
        public static int GetAdsAreaSetCount(long Advertisement_ID)
        {
            if (Advertisement_ID == 0)
                return 0;

            string sql = $"select ID from AdsDisplayAreaSet where Advertisement_ID={ Advertisement_ID } and ParentAdsDisplayAreaSetID is null";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            if (datas == null)
                return 0;

            return datas.Rows.Count;
        }
        /// <summary>
        /// 透過廣告區ID查詢顯示設定筆數(一般)
        /// </summary>
        /// <param name="Advertisement_ID"></param>
        /// <returns></returns>
        public static IEnumerable<AdsDisplayAreaSetModel> GetAdsAreaSet(long Advertisement_ID, long siteId)
        {
            if (Advertisement_ID == 0)
                return new List<AdsDisplayAreaSetModel>();

            string sql = $@"SELECT ID, GroupPosition 
                            FROM AdsDisplayAreaSet 
                            WHERE Advertisement_ID={ Advertisement_ID } AND ParentAdsDisplayAreaSetID IS NULL AND SiteID = {siteId}  ";

            using (var conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                return conn.Query<AdsDisplayAreaSetModel>(sql);
            }
        }

        /// <summary>
        /// 透過自訂廣告ID查詢相關之刊登時間費用等資料
        /// </summary>
        /// <param name="AdsCustomize_ID"></param>
        /// <returns></returns>
        public static List<AdsCustomizeAccountSet> QueryAccountSetByAdsCustomizeID(long AdsCustomize_ID)
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                List<AdsCustomizeAccountSet> list = new List<AdsCustomizeAccountSet>();
                string sql = "Select * From AdsCustomizeAccountSet where AdsCustomize_ID = @CustomId ";

                list = conn.Query<AdsCustomizeAccountSet>(sql, new { CustomId = AdsCustomize_ID }).ToList();

                foreach(AdsCustomizeAccountSet item in list)
                {
                    int diff = int.MaxValue;

                    if(item.IssueEnd != null)
                    {
                        diff = new TimeSpan(DateTime.Now.Ticks - ((DateTime)item.IssueEnd).Ticks).Days;
                    }

                    item.TimeDiff = diff;
                }

                return list;
            }
        }
        /// <summary>
        /// 儲存自訂廣告
        /// </summary>
        /// <param name="item"></param>
        public static void SetCustomizeItem(AdsCustomizeModel item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("AdsCustomize");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From AdsCustomize Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
                //UpdateAdvertisersInAdvertisement(item.Advertisement_ID, item.AdvertiserId);
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("Advertisement_ID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                tableObj["Modifier"] = MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                tableObj.Update(item.ID);
                //UpdateAdvertisersInAdvertisement(item.Advertisement_ID, item.AdvertiserId);
            }
        }
        /// <summary>
        /// 自訂廣告儲存廣告主
        /// </summary>
        /// <param name="adsCustomizeId"></param>
        /// <param name="advertisersId"></param>
        public static void saveAdvertisers(long adsCustomizeId, long advertisersId)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"update AdsCustomize set Advertisers_ID={advertisersId} where ID={adsCustomizeId}";
                conn.Execute(sql);
            }
        }

        /// <summary>
        /// 取得自訂廣告
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="Advertisement_ID"></param>
        /// <param name="SiteID"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<AdsCustomizeModel> GetAdsCustomize(AdsCustomizeSearchModel search, int pageSize, int pageIndex,long Advertisement_ID,long SiteID, out int recordCount)
        {
            List<AdsCustomizeModel> list = new List<AdsCustomizeModel>();
            recordCount = 0;

            if (search == null)
                return list;

            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(search.keyword))
            {
                //查詢 廣告主企業名稱、聯絡人姓名、職稱、電話、Email、相關資訊、廣告說明等關鍵字
                string condition = " and ("
                                 + " Advertisers.CompanyName like N'%{0}%'"
                                 + " or Advertisers.ContactPerson like N'%{0}%'"
                                 + " or Advertisers.JobTitle like N'%{0}%'"
                                 + " or Advertisers.ContactPhone like N'%{0}%'"
                                 + " or Advertisers.Email like N'%{0}%'"
                                 + " or Advertisers.Description like N'%{0}%'"
                                 + " or AdsCustomize.Description like N'%{0}%'"
                                 + ")";
                sb.Append(string.Format(condition, search.keyword.Trim().Replace("'", "''")));
            }
            if (search.IssueTimeStart != null && search.IssueTimeEnd != null)
            {
                sb.Append($" and AdsCustomizeAccountSet.IssueStart >= '{ search.IssueTimeStart.ToString("yyyy/MM/dd")}' and AdsCustomizeAccountSet.IssueEnd <= '{ search.IssueTimeEnd.ToString("yyyy/MM/dd") }'");
            }
            if (search.FilterIssueSetType != null)
            {
                List<string> temp = new List<string>();
                foreach (var item in search.FilterIssueSetType)
                {
                    switch(item)
                    {
                        case IssueSetType.IssueTime:
                        temp.Add($" AdsCustomizeAccountSet.IssueSetType='{IssueSetType.IssueTime}'");
                            break;
                        case IssueSetType.Click:
                        temp.Add($" AdsCustomizeAccountSet.IssueSetType='{IssueSetType.Click}'");
                            break;
                        case IssueSetType.Free:
                        temp.Add($" AdsCustomizeAccountSet.IssueSetType='{IssueSetType.Free}'");
                            break;
                    }
                }
                sb.Append("and (" + string.Join("or", temp) + ")");
            }

            if(!string.IsNullOrWhiteSpace(search.FilterCost) && search.CostMin != null && search.CostMax != null)
            {
                if(search.CostMax < search.CostMin)
                {
                    var temp = search.CostMax;
                    search.CostMin = search.CostMax;
                    search.CostMax = temp;
                }

                switch(search.FilterCost)
                {
                    case CostSearchType.Total:
                        sb.Append($" and (AdsCustomizeAccountSet.CostOfPeriod >= {search.CostMin} and AdsCustomizeAccountSet.CostOfPeriod <= {search.CostMax})");
                        break;
                    case CostSearchType.Month:
                        //sb.Append($" and (AdsCustomizeAccountSet.CostOfPeriod >= {search.CostMin} and AdsCustomizeAccountSet.CostOfPeriod <= {search.CostMax})");
                        break;
                    case CostSearchType.Day:
                        //sb.Append($" and (AdsCustomizeAccountSet.CostOfPeriod >= {search.CostMin} and AdsCustomizeAccountSet.CostOfPeriod <= {search.CostMax})");
                        break;
                }
            }
            if(search.ClickCountMin != null && search.ClickCountMax != null)
            {
                if (search.ClickCountMax < search.ClickCountMin)
                {
                    var temp = search.ClickCountMax;
                    search.ClickCountMin = search.ClickCountMax;
                    search.ClickCountMax = temp;
                }
                sb.Append($" and (AdsCustomizeAccountSet.BillingByClick >= {search.ClickCountMin} and AdsCustomizeAccountSet.BillingByClick <= {search.ClickCountMax})");
            }
            if (search.BrowseCountMin != null && search.BrowseCountMax != null)
            {
                if (search.BrowseCountMax < search.BrowseCountMin)
                {
                    var temp = search.BrowseCountMax;
                    search.BrowseCountMin = search.BrowseCountMax;
                    search.BrowseCountMax = temp;
                }
                sb.Append($" and (AdsCustomizeAccountSet.BillingByBrowse >= {search.BrowseCountMin} and AdsCustomizeAccountSet.BillingByBrowse <= {search.BrowseCountMax})");
            }
            if (search.FilterIssueSetType != null)
            {
                var type = search.FilterIssueSetType;
                for (int i = 0; i < type.Count(); i++)
                {
                    type[i] = "'" + type[i] + "'";
                }
                sb.Append(type.Count() != 0 ? string.Format(" and AdsCustomizeAccountSet.IssueSetType in ({0})", string.Join(", ", type)) : string.Empty);
            }
            //if (!string.IsNullOrEmpty(search.keyword))
            //{
            //    sb.Append(string.Format(" and AreaName like N'%{0}%'", search.keyword.Replace("'", "''")));
            //}

            string querystr = "Select DISTINCT AdsCustomize.* " +
                              "From AdsCustomize " +
                              "left join ( " +
                                          @"select *, 
	                                        datediff(day,ISNULL(IssueStart,getdate()),ISNULL(IssueEnd,getdate())) as IssueDayDiff ,
	                                        datediff(MONTH,ISNULL(IssueStart,getdate()),ISNULL(IssueEnd,getdate())) as IssueMonthDiff
	                                        from AdsCustomizeAccountSet" +
                              " )as AdsCustomizeAccountSet  on AdsCustomizeAccountSet.AdsCustomize_ID=AdsCustomize.ID " +
                              $"left join  (SELECT * FROM Advertisers WHERE SiteID={SiteID}) AS Advertisers on AdsCustomize.Advertisers_ID=Advertisers.ID " +
                              $"where AdsCustomize.Advertisement_ID={Advertisement_ID} " +
                              " {0} Order by AdsCustomize.Sort, CreateTime desc";

            string sql = string.Format(querystr, sb.ToString());

            //string sql = string.Format("Select * From AdsCustomize where Advertisement_ID=" + Advertisement_ID + " {0} Order by CreateTime desc", sb.ToString());

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetPageData(sql, pageSize, pageIndex, out recordCount);

            if (datas == null)
                return list;

            foreach (DataRow dr in datas.Rows)
            {
                list.Add(new AdsCustomizeModel
                {
                    ID = (long)dr["ID"],
                    Advertisement_ID = (long)dr["Advertisement_ID"],
                    Advertisers_ID = dr["Advertisers_ID"] as long ?,
                    Description = dr["Description"] != null ? dr["Description"].ToString().Trim() : string.Empty,
                    //ClickEvent = dr["ClickEvent"],
                    PhonePicFollowPC = (bool)dr["PhonePicFollowPC"],
                    IsIssue = (bool)dr["IsIssue"],
                    PCPicture = dr["PCPicture"] != null ? dr["PCPicture"].ToString().Trim() : string.Empty,
                    MobilePicture = dr["MobilePicture"] != null ? dr["MobilePicture"].ToString().Trim() : string.Empty,
                    Sort = (int)dr["Sort"]
                });
            }

            if (!string.IsNullOrWhiteSpace(search.FilterCost) && search.CostMin != null && search.CostMax != null)
            {
                switch (search.FilterCost)
                {
                    case CostSearchType.Total:
                        //sb.Append($" and (AdsCustomizeAccountSet.CostOfPeriod >= {search.CostMin} and AdsCustomizeAccountSet.CostOfPeriod <= {search.CostMax})");
                        break;
                    case CostSearchType.Month:
                        list = list.Where(m => m.CastDivideMonth >= search.CostMin && m.CastDivideMonth <= search.CostMax).ToList();
                        break;
                    case CostSearchType.Day:
                        list = list.Where(m => m.CastDivideDay >= search.CostMin && m.CastDivideDay <= search.CostMax).ToList();
                        break;
                }
            }
            return list;
        }
        /// <summary>
        /// 查詢廣告區 單筆資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static AdsCustomizeModel GetAdsCustomizeItem(long id)
        {
            return CommonDA.GetItem<AdsCustomizeModel>("AdsCustomize", id);
        }
        /// <summary>
        /// 搜尋出全部資料
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static IEnumerable<AdvertisersModel> SearchAdvertisersItems(string key,long siteId)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                key = $"N'%{ (key ?? string.Empty).Trim().Replace("'", "''") }%'";
                string sql = $"Select * From Advertisers Where SiteID={siteId} and IsIssue=1 and (CompanyName Like { key } OR ContactPerson Like { key } OR Description Like { key }) Order By IsNull(Sort, { int.MaxValue }), ID Desc";
                return conn.Query<AdvertisersModel>(sql);
            }
        }
        /// <summary>
        /// 更新廣告區主檔的廣告主ID
        /// </summary>
        /// <param name="id">廣告區主檔ID</param>
        /// <param name="Advertisers_ID">廣告主ID</param>
        //public static void UpdateAdvertisersInAdvertisement(long id,string Advertisers_ID)
        //{
        //    long aid = 0;
        //    if (long.TryParse(Advertisers_ID, out aid))
        //    {
        //        SQLData.Database db = new SQLData.Database(WebInfo.Conn);
        //        string sql = $"update Advertisement set Advertisers_ID = {aid} where ID= {id} ";
        //        db.ExecuteNonQuery(sql);
        //    }
        //}

        /// <summary>
        /// 移除自訂廣告的廣告主ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Advertisers_ID"></param>
        public static int RemoveAdvertisersInAdsCustomize(long id)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = $"update AdsCustomize set Advertisers_ID = NULL where ID= {id} ";
            return db.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// 儲存自訂廣告設定的刊登費用
        /// </summary>
        /// <param name="item"></param>
        public static void SetAdsCustomizeAccount(AdsCustomizeAccountSet item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("AdsCustomizeAccountSet");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From AdsCustomizeAccountSet Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("AdsCustomize_ID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                tableObj["Modifier"] = MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                tableObj.Update(item.ID);
            }
        }
        /// <summary>
        /// 取得自訂廣告設定的刊登費用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static AdsCustomizeAccountSet GetAdsCustomizeAccountItem(long id)
        {
            return CommonDA.GetItem<AdsCustomizeAccountSet>("AdsCustomizeAccountSet", id);
        }
        /// <summary>
        /// 透過廣告區ID查詢自訂廣告筆數
        /// </summary>
        /// <param name="Advertisement_ID"></param>
        /// <returns></returns>
        public static int GetAdsCustomizeCount(long Advertisement_ID)
        {
            if (Advertisement_ID == 0)
                return 0;

            string sql = $"select ID from AdsCustomize where Advertisement_ID={ Advertisement_ID }";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            if (datas == null)
                return 0;

            return datas.Rows.Count;
        }
        /// <summary>
        /// 儲存 自訂廣告管理_廣告事件連結
        /// </summary>
        /// <param name="item"></param>
        public static void SetAdsCustomizeLinkEdit(AdsCustomizeToLinkModel item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("AdsCustomizeToLink");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From AdsCustomizeToLink Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                string check = "Select 1 From AdsCustomizeToLink Where AdsCustomize_ID = " + item.AdsCustomize_ID;
                bool isHave = db.GetFirstValue(check) != null; //一個自訂廣告item只能有一個連結事件資料

                if(!isHave)
                {
                    tableObj.Insert();
                    UpdateClickEventInAdsCustomize(ClickEvent.Link, item.AdsCustomize_ID);
                }
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("AdsCustomize_ID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                tableObj["Modifier"] = MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                tableObj.Update(item.ID);
                UpdateClickEventInAdsCustomize(ClickEvent.Link, item.AdsCustomize_ID);
            }
        }
        /// <summary>
        /// 取得自訂廣告管理 點擊事件-連結 的資料
        /// </summary>
        /// <param name="AdsCustomize_ID"></param>
        /// <returns></returns>
        public static AdsCustomizeToLinkModel GetAdsCustomizeLinkItem(long AdsCustomize_ID)
        {
            if (AdsCustomize_ID == 0)
                return null;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = "Select * From AdsCustomizeToLink Where AdsCustomize_ID = " + AdsCustomize_ID;
                DataTable dt = new DataTable();
                dt.Load(conn.ExecuteReader(sql));

                if (dt.Rows.Count >= 1)
                {
                    return conn.Query<AdsCustomizeToLinkModel>(sql).FirstOrDefault();
                }

                return null;
            }
        }
        public static IEnumerable<dynamic> GetAdsCustomizeLinkInfo(long AdsCustomizeId)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = @" SELECT a.ID, b.Url, c.Type VideoType, c.Link VideoLink
                                FROM (select @ID ID) a
                                LEFT OUTER JOIN AdsCustomizeToLink b ON (a.ID = b.AdsCustomize_ID)
                                LEFT OUTER JOIN AdsCustomizeToVideo c ON (a.ID = c.AdsCustomize_ID)
                                WHERE a.ID = @ID ";
                return conn.Query(sql, new { ID = AdsCustomizeId });
            }
        }
        /// <summary>
        /// 更新自訂廣告管理的ClickEvent
        /// </summary>
        /// <param name="type"></param>
        /// <param name="AdsCustomize_ID"></param>
        public static void UpdateClickEventInAdsCustomize(string type, long AdsCustomize_ID)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = $"update AdsCustomize set ClickEvent = '{type}' where ID= {AdsCustomize_ID} ";
            db.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// 刪除自訂廣告
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static int AdsCustomizeDelete(IEnumerable<long> ids)
        {
            if (ids?.Count() == 0)
                return 0;

            string sql = @"Delete From AdsCustomize Where ID In ({0}) 
                           Delete From AdsCustomizeAccountSet Where AdsCustomize_ID In ({0}) 
                           Delete From AdsCustomizeToLink Where AdsCustomize_ID In ({0}) 
                           Delete From AdsCustomizeToVideo Where AdsCustomize_ID In ({0}) ";

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                num = conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }

            return num;
        }
        /// <summary>
        /// 儲存自訂廣告點擊事件(影片)部分
        /// </summary>
        /// <param name="item"></param>
        public static void SetAdsCustomizeVideoEdit(AdsCustomizeToVideoModel item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("AdsCustomizeToVideo");
            tableObj.GetDataFromObject(item);

            item.Link = item.Link ?? string.Empty;

            string sql = "Select 1 From AdsCustomizeToVideo Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                string check = "Select 1 From AdsCustomizeToVideo Where AdsCustomize_ID = " + item.AdsCustomize_ID;
                bool isHave = db.GetFirstValue(check) != null; //一個自訂廣告item只能有一個影片事件資料

                if (!isHave)
                {
                    tableObj.Insert();
                    UpdateClickEventInAdsCustomize(ClickEvent.Video, item.AdsCustomize_ID);
                }
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("AdsCustomize_ID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                tableObj["Modifier"] = MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                tableObj.Update(item.ID);
                UpdateClickEventInAdsCustomize(ClickEvent.Video, item.AdsCustomize_ID);
            }
        }
        /// <summary>
        /// 取得自訂廣告點擊事件(影片)部分
        /// </summary>
        /// <param name="AdsCustomize_ID"></param>
        /// <returns></returns>
        public static AdsCustomizeToVideoModel GetAdsCustomizeVideoItem(long AdsCustomize_ID)
        {
            if (AdsCustomize_ID == 0)
                return null;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = "Select * From AdsCustomizeToVideo Where AdsCustomize_ID = " + AdsCustomize_ID;
                DataTable dt = new DataTable();
                dt.Load(conn.ExecuteReader(sql));

                if (dt.Rows.Count >= 1)
                {
                    return conn.Query<AdsCustomizeToVideoModel>(sql).FirstOrDefault();
                }

                return null;
            }
        }
        /// <summary>
        /// 透過自訂廣告ID刪除點擊事件-影片
        /// </summary>
        /// <param name="id"></param>
        public static void DelVideoByAdsCustom(long id)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = $"update AdsCustomize set ClickEvent = '{ClickEvent.None}' where ID= {id} ";
            sql += $" delete from AdsCustomizeToVideo where AdsCustomize_ID={id}";
            db.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// 透過自訂廣告ID刪除點擊事件-連結
        /// </summary>
        /// <param name="id"></param>
        public static void DelLinksByAdsCustom(long id)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = $"update AdsCustomize set ClickEvent = '{ClickEvent.None}' where ID= {id} ";
            sql += $" delete from AdsCustomizeToLink where AdsCustomize_ID={id}";
            db.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// 取得顯示位置設定的選單樹資訊
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AdsDisplayAreaTrees> GetDisplayAreaTree(long Advertisement_ID, long SiteID)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = $" SELECT * FROM MENUS WHERE SiteID = {SiteID} AND AreaID in (1, 2, 3) AND ShowStatus in (1, 2) AND DataType in ('Article','Event','Member','Questionnaire','ArticleIntro','Intro') ORDER BY Sort ";

                IEnumerable<AdsDisplayAreaTrees> allNodes = conn.Query<AdsDisplayAreaTrees>(sql);
                
                IEnumerable<AdsDisplayAreaTrees> root = allNodes.Where(m => m.ParentID == 0);

                foreach(var node in root)
                {
                    node.CollectTreeNodes(allNodes);
                }

                return root;
            }
        }
        /// <summary>
        /// 取得顯示位置設定 (單筆 by AdsDisplayAreaSet.ID)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static AdsDisplayAreaSetModel GetAdsDisplayPositionItem(long id)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"select * from AdsDisplayAreaSet where ID={id} and ParentAdsDisplayAreaSetID is null";
                IEnumerable<AdsDisplayAreaSetModel> datas = conn.Query<AdsDisplayAreaSetModel>(sql);
                if (datas.Count() > 0)
                {
                    var data = datas.SingleOrDefault();
                    data.OverlayRepeatSeconds = data.OverlayRepeatSeconds / 60;
                    GetAreaSetChild(ref data);

                    return data;
                }
                else
                    return null;
            }
        }
        /// <summary>
        /// 取得顯示位置設定 (多筆 by AdsDisplayAreaSet.ID)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IEnumerable<AdsDisplayAreaSetModel> GetAdsDisplayPositionItems(long AdvertisementId, long menuId)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = "select * from AdsDisplayAreaSet where Advertisement_ID = @AdId and MenuID = @menuId";
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("@AdId", AdvertisementId);
                param.Add("@menuId", menuId);
                return conn.Query<AdsDisplayAreaSetModel>(sql, param);                
            }
        }
        /// <summary>
        /// 取得子資料
        /// </summary>
        /// <param name="model"></param>
        private static void GetAreaSetChild(ref AdsDisplayAreaSetModel model)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string childquery = $"select * from AdsDisplayAreaSet where ParentAdsDisplayAreaSetID={model.ID}";
                IEnumerable<AdsDisplayAreaSetModel> childs = conn.Query<AdsDisplayAreaSetModel>(childquery);
                if(childs.Count()>0)
                {
                    Type tye = typeof(WorkV3.Areas.Backend.Models.ChildType);
                    FieldInfo[] fields = tye.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

                    foreach (FieldInfo field in fields)
                    {
                        string val = field.GetValue(null).ToString();
                        if (childs.SingleOrDefault().ChildType == val)
                        {
                            typeof(AdsDisplayAreaSetModel).GetProperty(val + "AreaSet").SetValue(model, childs.SingleOrDefault(),null);
                        }
                    }
                }
            }
        }
        public static IEnumerable<AdsDisplayAreaSetModel> GetAreaSetChildByAreaSetID(long AreaSetID)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string childquery = $"select * from AdsDisplayAreaSet where ParentAdsDisplayAreaSetID={AreaSetID}";
                IEnumerable<AdsDisplayAreaSetModel> childs = conn.Query<AdsDisplayAreaSetModel>(childquery);
                if (childs.Count() > 0)
                    return childs;
                else
                    return new List<AdsDisplayAreaSetModel>();
            }
        }
        /// <summary>
        /// 儲存顯示位置設定(多選)。
        /// 因為位置要改為可多選，所以儲存機制改為先刪除所有目前已存位置再重新儲存。
        /// 若未選擇將視為未設定。
        /// </summary>
        /// <param name="items"></param>
        public static void SetAdsDisplayPositionItem(AdsPositionSetViewModel items, out string msg)
        {
            items.ListGroup = items.ListGroup ?? new string[]{ };
            items.InsideGroup = items.InsideGroup ?? new string[]{ };
            items.LoginGroup = items.LoginGroup ?? new string[]{ };
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = " SELECT * FROM AdsDisplayAreaSet WHERE Advertisement_ID = @AdId AND MenuID = @MenuId ";
                List<AdsDisplayAreaSetModel> backup = conn.Query<AdsDisplayAreaSetModel>(sql, new { AdId = items.AdvertisementId, MenuId = items.MenuId }).ToList();


                sql = " DELETE FROM AdsDisplayAreaSet WHERE Advertisement_ID = @AdId AND MenuID = @MenuId ";
                conn.Execute(sql, new { AdId = items.AdvertisementId, MenuId = items.MenuId });

                List<string> alreadySetPositions = new List<string>();
                List<AdsDisplayAreaSetModel> areas = new List<AdsDisplayAreaSetModel>();
                foreach (var item in items.ListGroup)
                {
                    AdsDisplayAreaSetModel area = new AdsDisplayAreaSetModel();
                    area.ID = WorkLib.GetItem.NewSN();
                    area.Advertisement_ID = items.AdvertisementId;
                    area.CreateTime = DateTime.Now;
                    area.Creator = MemberDAO.SysCurrent.Id;
                    area.SiteID = items.SiteId;
                    area.MenuID = items.MenuId;
                    area.DataType = items.DataType;
                    area.GroupPosition = item;
                    if (item == "Overlay")
                    {
                        area.OverlayChance = items.ListOverlayChance;
                        area.OverlayIdleSeconds = items.ListOverlayIdleSeconds;
                        area.OverlayRepeatSeconds = items.ListOverlayRepeatSeconds;
                        area.OverlayType = items.ListOverlayType;
                    }

                    SetParameterByPosition(area);
                    areas.Add(area);

                    string error = "";
                    if(CheckAreaSetUsed(area.GroupPosition, area.MenuID, area.Advertisement_ID, out error, area.ChildType != "", area.ChildType))
                        alreadySetPositions.Add(error);
                }
                foreach(var item in items.InsideGroup)
                {
                    AdsDisplayAreaSetModel area = new AdsDisplayAreaSetModel();
                    area.ID = WorkLib.GetItem.NewSN();
                    area.Advertisement_ID = items.AdvertisementId;
                    area.CreateTime = DateTime.Now;
                    area.Creator = MemberDAO.SysCurrent.Id;
                    area.SiteID = items.SiteId;
                    area.MenuID = items.MenuId;
                    area.DataType = items.DataType;
                    area.GroupPosition = item;
                    area.ChildType = "Inside";
                    if (item == "Overlay")
                    {
                        area.OverlayChance = items.InsideOverlayChance;
                        area.OverlayIdleSeconds = items.InsideOverlayIdleSeconds;
                        area.OverlayRepeatSeconds = items.InsideOverlayRepeatSeconds;
                        area.OverlayType = items.InsideOverlayType;
                    }

                    SetParameterByPosition(area);
                    areas.Add(area);

                    string error = "";
                    if (CheckAreaSetUsed(area.GroupPosition, area.MenuID, area.Advertisement_ID, out error, area.ChildType != "", area.ChildType))
                        alreadySetPositions.Add(error);
                }
                foreach(var item in items.LoginGroup)
                {
                    AdsDisplayAreaSetModel area = new AdsDisplayAreaSetModel();
                    area.ID = WorkLib.GetItem.NewSN();
                    area.Advertisement_ID = items.AdvertisementId;
                    area.CreateTime = DateTime.Now;
                    area.Creator = MemberDAO.SysCurrent.Id;
                    area.SiteID = items.SiteId;
                    area.MenuID = items.MenuId;
                    area.DataType = items.DataType;
                    area.GroupPosition = item;
                    area.ChildType = "Login";
                    if (item == "Overlay")
                    {
                        area.OverlayChance = items.LoginOverlayChance;
                        area.OverlayIdleSeconds = items.LoginOverlayIdleSeconds;
                        area.OverlayRepeatSeconds = items.LoginOverlayRepeatSeconds;
                        area.OverlayType = items.LoginOverlayType;
                    }

                    SetParameterByPosition(area);
                    areas.Add(area);

                    string error = "";
                    if (CheckAreaSetUsed(area.GroupPosition, area.MenuID, area.Advertisement_ID, out error, area.ChildType != "", area.ChildType))
                        alreadySetPositions.Add(error);
                }
                msg = string.Format("{0}", string.Join("     ", alreadySetPositions));

                PropertyInfo[] props = typeof(AdsDisplayAreaSetModel).GetProperties();
                string[] propNames = props.Where(m => m.Name != "InsideAreaSet" && m.Name != "LoginAreaSet" && m.Name != "DataType").Select(m => m.Name).ToArray();
                if(string.IsNullOrEmpty(msg))
                {
                    try
                    {
                        conn.Execute($@" INSERT INTO AdsDisplayAreaSet({string.Join(", ", propNames)}) VALUES ({string.Join(", ", propNames.AddPrefix("@"))})", areas);
                    }
                    catch (Exception ex)
                    {
                        msg = "廣告區顯示設定儲存失敗";
                    }
                }
                else
                {
                    // 復原一開始刪除的位置
                    try
                    {
                        conn.Execute($@" INSERT INTO AdsDisplayAreaSet({string.Join(", ", propNames)}) VALUES ({string.Join(", ", propNames.AddPrefix("@"))})", backup);
                    }
                    catch (Exception ex)
                    {
                        msg = "廣告區顯示設定儲存失敗";
                    }
                }

                msg = string.IsNullOrEmpty(msg)? "儲存成功": msg;
            }
        }
        /// <summary>
        /// 儲存顯示位置設定
        /// </summary>
        /// <param name="item"></param>
        public static void SetAdsDisplayPositionItem(AdsDisplayAreaSetModel item,out string msg)
        {
            SetParameterByPosition(item);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("AdsDisplayAreaSet");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From AdsDisplayAreaSet Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                // 沒有選擇廣告區塊的話就直接離開
                if (item.GroupPosition == null)
                {
                    msg = "未選擇廣告區塊";
                    return;
                }

                if (!CheckAreaSetUsed(item.GroupPosition, item.MenuID, item.Advertisement_ID,out msg))
                    tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("Advertisement_ID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                tableObj["Modifier"] = MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                if(!CheckAreaSetUsed(item.GroupPosition,item.MenuID,item.Advertisement_ID, out msg))
                    tableObj.Update(item.ID);
            }

            if (string.IsNullOrEmpty(msg))
            {
                try
                {
                    SetChildArea(db, item,out msg);
                    if(string.IsNullOrWhiteSpace(msg))
                        msg = "廣告區顯示設定已儲存";
                }
                catch (Exception ex)
                {
                    msg = "廣告區顯示設定儲存失敗";
                }
            }
        }
        /// <summary>
        /// 儲存位置設定的子資料
        /// </summary>
        /// <param name="db"></param>
        /// <param name="model"></param>
        private static void SetChildArea(SQLData.Database db, AdsDisplayAreaSetModel model,out string msg)
        {
            msg = string.Empty;
            switch (model.DataType)
            {
                case AreaSetDataType.Article:
                case AreaSetDataType.Event:
                default: 
                    if (model.InsideAreaSet != null)
                    {
                        var item = model.InsideAreaSet;
                        SetParameterByPosition(item);

                        SQLData.TableObject obj = db.GetTableObject("AdsDisplayAreaSet");
                        obj.GetDataFromObject(item);
                        string sql = "Select 1 From AdsDisplayAreaSet Where ID = " + item.ID;
                        bool isNew = db.GetFirstValue(sql) == null;
                        if (isNew)
                        {
                            obj["Creator"] = MemberDAO.SysCurrent.Id;
                            obj["CreateTime"] = DateTime.Now;
                            if (!CheckAreaSetUsed(item.GroupPosition, item.MenuID, item.Advertisement_ID, out msg, IsChild: true, childtype: item.ChildType))
                                obj.Insert();
                        }
                        else
                        {
                            obj.Remove("ID");
                            obj.Remove("Advertisement_ID");
                            obj.Remove("Creator");
                            obj.Remove("CreateTime");
                            obj["Modifier"] = MemberDAO.SysCurrent.Id;
                            obj["ModifyTime"] = DateTime.Now;
                            if (!CheckAreaSetUsed(item.GroupPosition, item.MenuID, item.Advertisement_ID, out msg, IsChild: true, childtype: item.ChildType))
                                obj.Update(item.ID);
                        }
                    }
                    break;
                case AreaSetDataType.Member:
                    if (model.LoginAreaSet != null)
                    {
                        var item = model.LoginAreaSet;
                        SetParameterByPosition(item);

                        SQLData.TableObject obj = db.GetTableObject("AdsDisplayAreaSet");
                        obj.GetDataFromObject(item);
                        string sql = "Select 1 From AdsDisplayAreaSet Where ID = " + item.ID;
                        bool isNew = db.GetFirstValue(sql) == null;
                        if (isNew)
                        {
                            obj["Creator"] = MemberDAO.SysCurrent.Id;
                            obj["CreateTime"] = DateTime.Now;
                            if (!CheckAreaSetUsed(item.GroupPosition, item.MenuID, item.Advertisement_ID, out msg, IsChild: true, childtype: item.ChildType))
                                obj.Insert();
                        }
                        else
                        {
                            obj.Remove("ID");
                            obj.Remove("Advertisement_ID");
                            obj.Remove("Creator");
                            obj.Remove("CreateTime");
                            obj["Modifier"] = MemberDAO.SysCurrent.Id;
                            obj["ModifyTime"] = DateTime.Now;
                            if (!CheckAreaSetUsed(item.GroupPosition, item.MenuID, item.Advertisement_ID, out msg, IsChild: true, childtype: item.ChildType))
                                obj.Update(item.ID);
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// 查詢在有效期間內  是否在不同廣告區之間用了同一個顯示位置
        /// </summary>
        /// <param name="GroupPosition"></param>
        /// <param name="MenuID"></param>
        /// <param name="limitCount"></param>
        /// <returns></returns>
        private static bool CheckAreaSetUsed(string GroupPosition,long MenuID,long Advertisement_ID, out string msg,bool IsChild = false,string childtype="")
        {
            msg = string.Empty ;
            var AdvertisementRenderDatas = 
                WorkV3.Models.DataAccess.AdvertisementRenderTools.GetAdvertisementRender(MenuID)
                .Where(m => m.GroupPosition == GroupPosition && m.ID != Advertisement_ID); // 先抓相同位置的，排除掉自己的廣告區ID

            if (IsChild)
            {
                if(string.IsNullOrWhiteSpace(childtype))
                {
                    msg = "發生錯誤(子資料集類型遺失)";
                    return false;
                }
                AdvertisementRenderDatas = AdvertisementRenderDatas.Where(m => m.AreaSetChildType == childtype); // 再抓內頁類型相同的 (inside, login)
            }
            else
                AdvertisementRenderDatas = AdvertisementRenderDatas.Where(m => string.IsNullOrEmpty(m.AreaSetChildType));

            if (AdvertisementRenderDatas.Count() == 0)
                return false;
            else
            {
                msg = string.Format("與「{0}」設定的刊登期間與顯示位置衝突，請重新設定。", AdvertisementRenderDatas.ToList()[0].AreaName);
                return true;
            }
        }
        /// <summary>
        /// 根據顯示位置設定相關參數
        /// </summary>
        /// <param name="model"></param>
        private static void SetParameterByPosition(AdsDisplayAreaSetModel model)
        {
            if (model == null)
                return;
            if (string.IsNullOrEmpty(model.GroupPosition))
                return;
            if (model.GroupPosition != DisplayAreaShardType.Overlay)
                return;

            if(model.OverlayType== OverlayType.Chance)
            {
                model.OverlayChance = model.OverlayChance??0; //蓋台機率
                model.OverlayRepeatSeconds = 0; //重複播放秒數
                model.OverlayIdleSeconds = 0; //閒置秒數
            }
            else if(model.OverlayType == OverlayType.Once)
            {
                model.OverlayChance = 0; //蓋台機率
                model.OverlayRepeatSeconds = 0; //重複播放秒數
                model.OverlayIdleSeconds = 0; //閒置秒數
            }
            else if (model.OverlayType == OverlayType.AfrerSec)
            {
                model.OverlayChance = 0; //蓋台機率
                model.OverlayRepeatSeconds = model.OverlayRepeatSeconds != null ? model.OverlayRepeatSeconds * 60 : 0; //重複播放秒數
                model.OverlayIdleSeconds = 0; //閒置秒數
            }
            else if (model.OverlayType == OverlayType.AfterIdle)
            {
                model.OverlayChance = 0; //蓋台機率
                model.OverlayRepeatSeconds = 0; //重複播放秒數
                model.OverlayIdleSeconds = model.OverlayIdleSeconds??0; //閒置秒數
            }
        }
        /// <summary>
        /// 取得顯示位置資訊
        /// </summary>
        /// <param name="SiteID"></param>
        /// <param name="MenuID"></param>
        /// <param name="Advertisement_ID"></param>
        /// <param name="DataType"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetDisplayPositionInfo(long SiteID,long MenuID,long Advertisement_ID,string DataType, DisplayAreaType type = DisplayAreaType.List)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string childTypeOption = "";
                
                switch(type)
                {
                    case DisplayAreaType.List:
                    default:
                        childTypeOption = "ChildType is null";
                        break;
                    case DisplayAreaType.Inside:
                        childTypeOption = "ChildType = 'Inside'";
                        break;
                    case DisplayAreaType.Login:
                        childTypeOption = "ChildType = 'Login'";
                        break;
                }

                string sql = $"select GroupPosition from AdsDisplayAreaSet where Advertisement_ID={Advertisement_ID} and SiteID={SiteID} and MenuID={MenuID} and {childTypeOption} ";
                var query = conn.ExecuteReader(sql);
                DataTable dt = new DataTable();
                dt.Load(query);
                List<string> GroupPositionDatas = new List<string>();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if(dr["GroupPosition"] != null)
                        {
                            var result = GetDataTypeAttrName(dr["GroupPosition"].ToString().Trim(), DataType);
                            if(!string.IsNullOrEmpty(result))
                                GroupPositionDatas.Add(result);
                        }
                    }
                }
                return GroupPositionDatas;
            }
        }
        /// <summary>
        ///  取得顯示位置資訊之名稱
        /// </summary>
        /// <param name="ItemStr"></param>
        /// <param name="DataType"></param>
        /// <returns></returns>
        public static string GetDataTypeAttrName(string ItemStr,string DataType)
        {
            if (!string.IsNullOrEmpty(ItemStr) && !string.IsNullOrEmpty(DataType))
            {
                try
                {
                    string typestr = "WorkV3.Areas.Backend.Models.DisplayAreaType" + DataType;
                    Type t = Type.GetType(typestr);
                    MemberInfo[] memInfo = t.GetMember(ItemStr);
                    var da = memInfo[0].GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
                    if (da != null)
                        return da.Name;
                }
                catch
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 取得顯示位置對應名稱
        /// </summary>
        /// <param name="DataType"></param>
        /// <returns></returns>
        public static string GetDataTypeName(string DataType)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(DataType))
                return result;

            switch(DataType)
            {
                case AreaSetDataType.Intro:
                    result = "首頁";
                    break;
                case AreaSetDataType.ArticleIntro:
                    result = "單頁";
                    break;
                case AreaSetDataType.Article:
                    result = "列表";
                    break;
                case AreaSetDataType.Questionnaire:
                    result = "列表";
                    break;
                case AreaSetDataType.Event:
                    result = "列表";
                    break;
                case AreaSetDataType.Member:
                    result = "主頁";
                    break;
                default:
                    break;
            }
            return result;
        }
        /// <summary>
        /// 取得顯示位置對應名稱
        /// </summary>
        /// <param name="DataType"></param>
        /// <returns></returns>
        public static string GetChildTypeName(string ChildType)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(ChildType))
                return result;

            switch (ChildType)
            {
                case WorkV3.Areas.Backend.Models.ChildType.Inside:
                    result = "內頁";
                    break;
                case WorkV3.Areas.Backend.Models.ChildType.Login:
                    result = "登入";
                    break;
                default:
                    break;
            }
            return result;
        }
        /// <summary>
        /// 從dbo.AdsStatistics查詢瀏覽或點擊數量
        /// </summary>
        /// <param name="AdsCustomize_ID"></param>
        /// <param name="Event"></param>
        /// <returns></returns>
        public static long QueryAdFlow(long AdsCustomize_ID, string Event, long specificId = 0)
        {
            if (AdsCustomize_ID == 0 || string.IsNullOrWhiteSpace(Event))
                return 0;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                string sql = "";

                if (specificId == 0)
                {
                    sql = "select count(*) from AdsStatistics where AdsCustomizeID=@CustomizeId and Event=@Event";
                    param.Add("@CustomizeId", AdsCustomize_ID);
                    param.Add("@Event", Event);
                }
                else
                {
                    sql = @" SELECT COUNT(*)
                             FROM AdsStatistics a
                             JOIN AdsCustomizeAccountSet b ON ( b.ID = @AccountSetId AND a.AdsCustomizeID = b.AdsCustomize_ID AND a.RecordTime BETWEEN b.IssueStart and b.IssueEnd)
                             WHERE AdsCustomizeID=@CustomizeId and Event=@Event  ";
                    param.Add("AccountSetID", specificId);
                    param.Add("@CustomizeId", AdsCustomize_ID);
                    param.Add("@Event", Event);
                }

                var result = conn.Query<long>(sql, param);

                if (result.Count() > 0)
                    return result.ElementAt(0);
                else
                    return 0;
            }
        }
        /// <summary>
        /// 自訂廣告區複製
        /// </summary>
        /// <param name="AdsCustomizeIds"></param>
        /// <param name="siteId"></param>
        /// <param name="menuId"></param>
        /// <param name="AdvertisementID"></param>
        public static void AdsCustomizeCopy(IEnumerable<long> AdsCustomizeIds, long siteId, long menuId, long AdvertisementID)
        {
            if (AdsCustomizeIds == null || siteId == 0 || menuId == 0 || AdvertisementID == 0)
                return;

            foreach (var id in AdsCustomizeIds)
            {
                //關聯 : 自訂廣告主檔 / 帳務(目前沒有複製) / AdsCustomizeToLink / AdsCustomizeToVideo 
                long newAdsCustomizeID = WorkLib.GetItem.NewSN();

                AdsCustomizeModel AdsCustomizeItem = GetAdsCustomizeItem(id); //取得自訂廣告
                if (AdsCustomizeItem == null)
                    continue;
                AdsCustomizeToLinkModel Link = GetAdsCustomizeLinkItem(id); //取得自訂廣告管理 廣告事件 連結
                AdsCustomizeToVideoModel Video = GetAdsCustomizeVideoItem(id); //取得自訂廣告管理 廣告事件 影片

                //替換新的自訂廣告ID和主鍵ID並儲存
                AdsCustomizeItem.ID = newAdsCustomizeID; 
                SetCustomizeItem(AdsCustomizeItem);
                if (Link != null)
                {
                    Link.ID = WorkLib.GetItem.NewSN();
                    Link.ModifyTime = null;
                    Link.Modifier = null;
                    Link.AdsCustomize_ID = newAdsCustomizeID;
                    SetAdsCustomizeLinkEdit(Link);
                }
                if (Video != null)
                {
                    Video.ID = WorkLib.GetItem.NewSN();
                    Video.ModifyTime = null;
                    Video.Modifier = null;
                    Video.AdsCustomize_ID = newAdsCustomizeID;
                    SetAdsCustomizeVideoEdit(Video);
                }
            }
        }

        //--------------------------------
        /// <summary>
        /// 暫時使用 : 顯示位置設定因部分尚未完成, 為順利測試, 僅完成的部分可秀出內容
        /// 使用位置 : 後台 AdsDisplaySetting.cshtml
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool CheckIsWork(AdsDisplayAreaTrees data)
        {
            var typ = typeof(AreaSetDataType);
            FieldInfo[] fields = typ.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            List<string> typlist = new List<string>();
            foreach (FieldInfo field in fields)
            {
                typlist.Add(field.GetValue(null).ToString());
            }
            return typlist.Any(m => m == data.DataType);
        }
        /// <summary>
        /// 取得CPUID
        /// </summary>
        /// <returns></returns>
        public static string GetCpuID()
        {
            string cupid = string.Empty;
            try
            {
                System.Management.ManagementObjectSearcher wmiSearcher = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                foreach (System.Management.ManagementObject obj in wmiSearcher.Get())
                {
                    cupid = obj.Properties["processorID"].Value.ToString();
                    break;
                }
            }
            catch
            {

            }
            return cupid;
        }

        public static bool IsDurationOverlapped(DateTime time1Start, DateTime time1End, DateTime time2Start, DateTime time2End)
        {
            List<DateTime> timeList = new List<DateTime>();
            timeList.Add(time1Start);
            timeList.Add(time1End);
            timeList.Add(time2Start);
            timeList.Add(time2End);

            TimeSpan ts1 = time1End.Subtract(time1Start); // time1 diff
            TimeSpan ts2 = time2End.Subtract(time2Start); // time2 diff
            TimeSpan ts3 = timeList.Max().Subtract(timeList.Min());

            return ts1 + ts2 > ts3;
        }
    }
}

namespace WorkV3.Areas.Backend.Models.Extensions
{
    public static class Extension
    {
        /// <summary>
        /// 在陣列所有元素加上前綴字
        /// </summary>
        /// <returns></returns>
        public static string[] AddPrefix(this string[] array, string prefix)
        {
            string[] retValue = new string[array.Count()];
            for (int i = 0; i < array.Count(); i++)
            {
                retValue[i] = prefix + array[i];
            }

            return retValue;
        }
        
        public static void CollectTreeNodes(this AdsDisplayAreaTrees currentNode, IEnumerable<AdsDisplayAreaTrees> nodes)
        {
            currentNode.children = nodes.Where(m => m.ParentID == currentNode.ID);

            foreach(var node in currentNode.children)
            {
                node.CollectTreeNodes(nodes);
            }
        }
    }
}