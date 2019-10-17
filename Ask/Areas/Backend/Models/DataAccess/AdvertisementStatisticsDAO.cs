using Dapper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using WorkV3.Common;
using Dapper;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using WorkV3.Areas.Backend.ViewModels;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class AdvertisementStatisticsDAO
    {
        public static Dictionary<string, string> AdsPosition = new Dictionary<string, string>();
        static AdvertisementStatisticsDAO()
        {
            AdsPosition.Add("Top", "上橫幅");
            AdsPosition.Add("Top_1", "上一橫幅");
            AdsPosition.Add("Top_2", "上二橫幅");
            AdsPosition.Add("Middle_1", "中一橫幅");
            AdsPosition.Add("Middle_2", "中二橫幅");
            AdsPosition.Add("Bottom", "下橫幅");
            AdsPosition.Add("Bottom_1", "下一橫幅");
            AdsPosition.Add("Bottom_2", "下二橫幅");
            AdsPosition.Add("Right_1", "右一橫幅");
            AdsPosition.Add("Right_2", "右二橫幅");
            AdsPosition.Add("Right_3", "右三橫幅");
            AdsPosition.Add("Right_4", "右四橫幅");
            AdsPosition.Add("Overlay", "蓋台廣告");
        }

        public static string GetChart(DateTime startDate, DateTime endDate, List<long> advertisers = null, long adsCustomizeId = 0)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string advertisersList = (advertisers == null)? string.Empty: string.Join(", ", advertisers);
                string filterAdvertisers = string.Empty;
                if(advertisers != null)
                {
                    filterAdvertisers = $" AND AdsCustomizeID IN ( SELECT ID FROM AdsCustomize WHERE Advertisers_ID IN ({advertisersList}) ) ";
                }
                else if(adsCustomizeId != 0)
                {
                    filterAdvertisers = $" AND AdsCustomizeID = {adsCustomizeId} ";
                }

                string sql = $@"select dt, sum(BrowseCount) BrowseCount, sum(ClickCount) ClickCount
                       from (
	                       select convert(varchar,RecordDay,111) dt, sum(1) BrowseCount, 0 ClickCount
	                       from AdsStatistics
	                       where [Event] = 'Browsing' {filterAdvertisers}
	                       group by convert(varchar,RecordDay,111)
	                       union all
	                       select convert(varchar,RecordDay,111) dt, 0 BrowseCount, sum(1) ClickCount
	                       from AdsStatistics
	                       where [Event] = 'Click' {filterAdvertisers}
	                       group by convert(varchar,RecordDay,111)
                       ) as b
                       where dt between @StartDate and @EndDate
                       group by dt";
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("@StartDate", startDate);
                param.Add("@EndDate", endDate);

                IEnumerable<StatisticsModel> result = conn.Query<StatisticsModel>(sql, param);

                JavaScriptSerializer jss = new JavaScriptSerializer();
                return jss.Serialize(result);
            }
        }

        public static List<AdsStatisticsViewModel> GetAll(AdsStatisticsSearchModel search)
        {
            int recordCount = 0;
            return GetAdsStatisticsData(search, int.MaxValue, 1, out recordCount);
        }

        public static List<AdsStatisticsDetailViewModel> GetAllDetail(AdsDetailStatisticsSearchModel search)
        {
            int recordCount = 0;
            return GetStatisticsDetail(search, int.MaxValue, 1, out recordCount);
        }

        public static List<AdvertiserStatisticsViewModel> GetAllAdvertisers(AdsDetailStatisticsSearchModel search)
        {
            int recordCount = 0;
            return GetAdvertiserStatistics(search, int.MaxValue, 1, out recordCount);
        }

        /// <summary>
        /// 取得廣告成效資料
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<AdsStatisticsViewModel> GetAdsStatisticsData(AdsStatisticsSearchModel search, int pageSize, int pageIndex, out int recordCount)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                search.Advertisers = search.Advertisers ?? new List<long>();
                string whereClause = search.Advertisers.Count > 0? $" WHERE Advertisers_ID IN ({string.Join(", ", search.Advertisers)}) ": "";
                string dateConstraint = "";
                
                if (search.StartDate != DateTime.MinValue && search.EndDate != DateTime.MinValue)
                {
                    dateConstraint = $" AND (RecordDay BETWEEN '{search.StartDate.ToString("yyyy/MM/dd")}' AND '{search.EndDate.ToString("yyyy/MM/dd")}') AND a.AdsCustomizeID = b.AdsCustomize_ID ";
                }

                string orderDirection = search.IsAsc? " ASC ": " DESC ";
                string order = "";
                switch(search.OrderType)
                {
                    case 1:
                    default:
                        order = $" ORDER BY ClickCount {orderDirection}, BrowseCount {orderDirection} ";
                        break;
                    case 2:
                        order = $" ORDER BY BrowseCount {orderDirection}, ClickCount {orderDirection} ";
                        break;
                    case 3:
                        order = $" ORDER BY FullPrice {orderDirection}, ClickCount {orderDirection}, BrowseCount {orderDirection} ";
                        break;
                    case 4:
                        order = $" ORDER BY CP {orderDirection}, ClickCount {orderDirection}, BrowseCount {orderDirection} ";
                        break;
                }

                string filterAdvertiser = search.Advertisers.Count > 0 ? $" AND Advertisers_ID IN ({string.Join(", ", search.Advertisers)}) " : "";
                string sql = $@"select z.MenuID, z.BrowseCount, z.ClickCount, a.ID AdsCustomizeID, c.Title MenuTitle, 
	                                    (case when d.BillingByClick is null then 0 else d.BillingByClick end * z.ClickCount + case when d.BillingByBrowse is null then 0 else d.BillingByBrowse end * z.BrowseCount) FullPrice,
	                                    case when 
			                                 case when d.BillingByClick is null then 0 else d.BillingByClick end + case when d.BillingByBrowse is null 
	                                    then 0 
	                                    else d.BillingByBrowse end > 0 then ROUND(CAST(z.ClickCount AS DECIMAL(20,2)) / CAST((case when d.BillingByClick is null then 0 else d.BillingByClick end + case when d.BillingByBrowse is null then 0 else d.BillingByBrowse end) AS DECIMAL(20,2)), 3) else 0 end CP,
	                                    f.CompanyName, f.ContactPerson, f.ContactPhone, a.*
                                 from (
	                                 select  AdsCustomizeID, MenuID, sum(case when [Event] = 'Browsing' then [Count] else 0 end) BrowseCount, sum(case when [Event] = 'Click' then [Count] else 0 end) ClickCount
	                                 from 
	                                 (
		                                 select a.AdsCustomizeID, c.MenuID, a.[Event], count(*) 'Count'
		                                 from AdsStatistics a
		                                 join AdsCustomize b on (a.AdsCustomizeID = b.ID)
		                                 join Pages c on (a.PageID = c.No)
		                                 join (
				                                 select Advertisement_ID, MenuID
				                                 from AdsDisplayAreaSet
				                                 group by Advertisement_ID, MenuID
			                                 ) d on (b.Advertisement_ID = d.Advertisement_ID and c.MenuID = d.MenuID)
		                                 where AdsCustomizeID in (
			                                 select distinct a.AdsCustomizeID 
			                                 from AdsStatistics a
			                                 join AdsCustomize b on (a.AdsCustomizeID = b.ID {filterAdvertiser})
			                                 where RecordDay BETWEEN '{search.StartDate.ToString("yyyy/MM/dd")}' AND '{search.EndDate.ToString("yyyy/MM/dd")}'
		                                 )
		                                 group by a.AdsCustomizeID, c.MenuID, a.[Event]
	                                 ) a
                                 group by AdsCustomizeID, MenuID
                                 ) z
                                 join AdsCustomize a on(z.AdsCustomizeID = a.ID)
                                 JOIN (SELECT Advertisement_ID, MenuID FROM AdsDisplayAreaSet group by Advertisement_ID, MenuID) b ON (a.Advertisement_ID = b.Advertisement_ID and z.MenuID = b.MenuID)
                                 JOIN Menus c on (b.MenuID = c.ID)
                                 join AdsCustomizeAccountSet d on (d.AdsCustomize_ID = a.ID)
                                 LEFT JOIN Advertisers f ON (a.Advertisers_ID = f.ID)
                                 {whereClause} ";

                return CommonDA.GetPageData<AdsStatisticsViewModel>(sql, pageSize, pageIndex, out recordCount, null, order).ToList();
            }
        }

        public static List<AdvertiserStatisticsViewModel> GetAdvertiserStatistics(AdsDetailStatisticsSearchModel search, int pageSize, int pageIndex, out int recordCount)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {

                List<string> wheres = new List<string>();

                string dateConstraint = "";
                if (search.StartDate != DateTime.MinValue && search.EndDate != DateTime.MinValue)
                {
                    dateConstraint = ($" AND (RecordDay BETWEEN '{search.StartDate.ToString("yyyy/MM/dd")}' AND '{search.EndDate.ToString("yyyy/MM/dd")}') ");
                }

                string sql = $@"SELECT AdvertiserID, CompanyName, ContactInfo, sum(ClickCount) ClickCount, sum(BrowseCount) BrowseCount, sum(FeeEstimate) FeeEstimate
                                FROM (
                                        SELECT c.ID AdvertiserID, c.CompanyName, (c.ContactPerson + ' ' + c.ContactPhone) ContactInfo, a.ClickCount, a.BrowseCount, a.FeeEstimate, CASE WHEN a.FeeEstimate = 0 THEN 0 ELSE ROUND(CAST(a.ClickCount AS DECIMAL(20,2)) / CAST(a.FeeEstimate AS DECIMAL(20,2)), 3) END CP
                                        FROM (SELECT AdsCustomizeID, sum(ClickCount) ClickCount, sum(BrowseCount) BrowseCount, sum(Fee) FeeEstimate
		                                        FROM (SELECT a.AdsCustomizeID, 1 ClickCount, 0 BrowseCount, ISNULL(b.BillingByClick, 0) Fee
				                                        FROM AdsStatistics a 
				                                        JOIN AdsCustomizeAccountSet b ON (a.AdsCustomizeID = b.AdsCustomize_ID)
				                                        WHERE a.Event = 'Click' {dateConstraint}
				                                        UNION ALL
				                                        SELECT a.AdsCustomizeID, 0 ClickCount, 1 BrowseCount, ISNULL(b.BillingByBrowse, 0) Fee
				                                        FROM AdsStatistics a 
				                                        JOIN AdsCustomizeAccountSet b ON (a.AdsCustomizeID = b.AdsCustomize_ID)
				                                        WHERE a.Event = 'Browsing' {dateConstraint}
		                                        ) a GROUP BY AdsCustomizeID
                                        ) a
                                        JOIN AdsCustomize b ON (a.AdsCustomizeID = b.ID)
                                        JOIN Advertisers c ON (b.Advertisers_ID = c.ID)
                                        WHERE b.Advertisers_ID is not null
                                        GROUP BY c.ID, c.CompanyName, c.ContactPerson, c.ContactPhone, a.ClickCount, a.BrowseCount, a.FeeEstimate ) a
                                GROUP BY AdvertiserID, CompanyName, ContactInfo ";
                string orderDirection = search.IsAsc ? " ASC " : " DESC ";
                string order = $" ORDER BY ClickCount {orderDirection} ";

                return CommonDA.GetPageData<AdvertiserStatisticsViewModel>(sql, pageSize, pageIndex, out recordCount, null, order).ToList();
            }
        }

        public static List<AdsStatisticsDetailViewModel> GetStatisticsDetail(AdsDetailStatisticsSearchModel search, int pageSize, int pageIndex, out int recordCount)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                List<string> wheres = new List<string>();
                wheres.Add($" [Event] = '{search.Type}' ");

                string dateConstraint = "";
                if (search.StartDate != DateTime.MinValue && search.EndDate != DateTime.MinValue)
                {
                    wheres.Add($" (RecordDay BETWEEN '{search.StartDate.ToString("yyyy/MM/dd")}' AND '{search.EndDate.ToString("yyyy/MM/dd")}') ");
                }

                if(search.AdsCustomId != 0)
                {
                    wheres.Add($" AdsCustomizeID = @AdsCustomizeID ");
                    param.Add("@AdsCustomizeID", search.AdsCustomId);
                }

                string whereClause = wheres.Count == 0? "": $" WHERE {String.Join(" AND ", wheres)} ";


                string orderDirection = search.IsAsc ? " ASC " : " DESC ";
                string order = $" ORDER BY RecordTime {orderDirection} ";
                string sql = $@"SELECT distinct d.Name MemberName, d.Email MemberEmail, c.Title, a.*
                                FROM [huashan].[dbo].[AdsStatistics] a
                                JOIN AdsCustomize b on (a.AdsCustomizeID = b.ID)
                                JOIN Pages c on (a.PageID = c.No)
                                LEFT OUTER JOIN Member d on (a.MemberId = d.ID)
                                {whereClause}  ";

                return CommonDA.GetPageData<AdsStatisticsDetailViewModel>(sql, pageSize, pageIndex, out recordCount, param, order).ToList();
            }
        }
    }
}