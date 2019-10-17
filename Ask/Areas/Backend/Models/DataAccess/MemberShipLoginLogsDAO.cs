using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class MemberShipLoginLogsDAO
    {

        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static List<ViewModels.MemberAnalysisDailyLogViewModel> GetMemberLoginStatistics(DateTime startDate, DateTime endDate)
        {
            List<ViewModels.MemberAnalysisDailyLogViewModel> logModelList = new List<ViewModels.MemberAnalysisDailyLogViewModel>();
            DataTable dt = GetMemberLoginStatisticsTable(startDate, endDate);
            for (int i = 0; i < endDate.Subtract(startDate).Days; i++)
            {
                DateTime sdate = startDate.AddDays(i);
                DataRow[] selLogDate = dt.Select("LogDate='"+ sdate.ToString("yyyy/MM/dd") + "'");
                if (selLogDate != null && selLogDate.Length > 0)
                {
                    logModelList.Add(new ViewModels.MemberAnalysisDailyLogViewModel()
                    {
                        LogDate = DateTime.Parse(selLogDate[0]["LogDate"].ToString()).ToString("MM/dd"),
                        MemberCount = long.Parse(selLogDate[0]["MemberCount"].ToString()),
                        LoginCount = long.Parse(selLogDate[0]["LoginCount"].ToString())
                    });
                }
                else
                {
                    logModelList.Add(new ViewModels.MemberAnalysisDailyLogViewModel()
                    {
                        LogDate = sdate.ToString("MM/dd"),
                        MemberCount = 0,
                        LoginCount = 0
                    });
                }
            }
            return logModelList;
        }
        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static DataTable GetMemberLoginStatisticsTable(DateTime startDate, DateTime endDate)
        {
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@StartDate", startDate);
            paraList.Add("@EndDate", endDate);
            string Sql = @" SELECT LogDate, SUM(MemberCount) AS MemberCount, SUM(LoginCount) AS LoginCount FROM
                            (
                                SELECT Convert(nvarchar(10), CreateTime, 111) AS LogDate, SUM(1) AS MemberCount,0 AS LoginCount
                                FROM MemberShip WHERE Convert(nvarchar(10), CreateTime, 111)>=@StartDate AND Convert(nvarchar(10), CreateTime, 111)<=@EndDate
                                GROUP BY Convert(nvarchar(10), CreateTime, 111) 
                                UNION ALL
                                SELECT Convert(nvarchar(10), AddTime, 111) AS LogDate, 0 AS MemberCount, Count(1) AS LoginCount
                                FROM [MemberShipLoginLogs]  WHERE Convert(nvarchar(10), AddTime, 111)>=@StartDate AND Convert(nvarchar(10), AddTime, 111)<=@EndDate
                                GROUP BY Convert(nvarchar(10), AddTime, 111) 
                            ) AS LogData
                            GROUP BY LogDate
                            ORDER BY LogDate ASC";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable dt = db.GetDataTable(Sql, paraList);
            return dt;
        }
        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static List<ViewModels.MemberAnalysisMonthlyLogViewModel> GetMonthMemberLoginStatistics(DateTime startDate, DateTime endDate)
        {
            List<ViewModels.MemberAnalysisMonthlyLogViewModel> logModelList = new List<ViewModels.MemberAnalysisMonthlyLogViewModel>();
            DataTable dt = GetMonthMemberLoginStatisticsTable(startDate, endDate);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    logModelList.Add(new ViewModels.MemberAnalysisMonthlyLogViewModel()
                    {
                        LogMonth = row["LogMonth"].ToString().Replace("/","年")+"月",
                        MemberCount = long.Parse(row["MemberCount"].ToString()),
                        LoginCount = long.Parse(row["LoginCount"].ToString())
                    });
                }
            }
            return logModelList;
        }
        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static DataTable GetMonthMemberLoginStatisticsTable(DateTime startDate, DateTime endDate)
        {
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@StartDate", startDate);
            paraList.Add("@EndDate", endDate);
            List<ViewModels.MemberAnalysisMonthlyLogViewModel> logModelList = new List<ViewModels.MemberAnalysisMonthlyLogViewModel>();
            string Sql = @" SELECT Left(LogDate,7) AS LogMonth, SUM(MemberCount) AS MemberCount, SUM(LoginCount) AS LoginCount FROM
                            (
                                SELECT Convert(nvarchar(10), CreateTime, 111) AS LogDate, SUM(1) AS MemberCount,0 AS LoginCount
                                FROM MemberShip WHERE Convert(nvarchar(10), CreateTime, 111)>=@StartDate AND Convert(nvarchar(10), CreateTime, 111)<=@EndDate
                                GROUP BY Convert(nvarchar(10), CreateTime, 111) 
                                UNION ALL
                                SELECT Convert(nvarchar(10), AddTime, 111) AS LogDate, 0 AS MemberCount, Count(1) AS LoginCount
                                FROM [MemberShipLoginLogs]  WHERE Convert(nvarchar(10), AddTime, 111)>=@StartDate AND Convert(nvarchar(10), AddTime, 111)<=@EndDate
                                GROUP BY Convert(nvarchar(10), AddTime, 111) 
                            ) AS LogData
                            GROUP BY Left(LogDate,7) 
                            ORDER BY LogMonth ASC";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable dt = db.GetDataTable(Sql, paraList);
            return dt;
        }

        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static ViewModels.MemberFieldViewModel GetStatisticsValue(ViewModels.FieldCategory catetory, string fieldName, DateTime startDate, DateTime endDate)
        {
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@StartDate", startDate.ToString("yyyy/MM/dd"));
            paraList.Add("@EndDate", endDate.ToString("yyyy/MM/dd"));
            ViewModels.MemberFieldViewModel fieldViewModel = new ViewModels.MemberFieldViewModel();
            fieldViewModel.FieldName = fieldName;
            string Sql_Input = @" SELECT SUM(case when ({0}='' or {0} is null) then 0 else 1 end) AS valued_count
                                , SUM(case when ({0}='' or {0} is null) then 1 else 0 end) AS non_valued_count FROM  MemberShip 
                                WHERE Convert(nvarchar(10), CreateTime, 111)>=@StartDate AND Convert(nvarchar(10), CreateTime, 111)<=@EndDate ";
            string Sql_Input_Address = @" SELECT SUM(case when (Address<>'' and Address is not null and Regions<>'' and Regions is not null) then 1 else 0 end) AS valued_count
                                , SUM(case when (Address<>'' and Address is not null and Regions<>'' and Regions is not null) then 0 else 1 end) AS non_valued_count FROM  MemberShip 
                                WHERE Convert(nvarchar(10), CreateTime, 111)>=@StartDate AND Convert(nvarchar(10), CreateTime, 111)<=@EndDate ";
            string Sql_Options = @" SELECT {0}, SUM(1) AS Count FROM  MemberShip 
                                 WHERE Convert(nvarchar(10), CreateTime, 111)>=@StartDate AND Convert(nvarchar(10), CreateTime, 111)<=@EndDate
                                 GROUP BY {0} ORDER BY {0}";
            string Sql_Checked = @" SELECT SUM(case when {0}=1 then 1 else 0 end) AS valued_count
                                , SUM(case when {0}=0 then 1 else 0 end) AS non_valued_count FROM  MemberShip 
                                WHERE Convert(nvarchar(10), CreateTime, 111)>=@StartDate AND Convert(nvarchar(10), CreateTime, 111)<=@EndDate ";
            string Sql = "";
            fieldViewModel.Category = catetory;
            fieldViewModel.StatisticList = new List<ViewModels.MemberFieldStatisticViewModel>();
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable dt = new DataTable();
            if (catetory == ViewModels.FieldCategory.Sex || catetory == ViewModels.FieldCategory.MemberType)
            {
                Sql = string.Format(Sql_Options, catetory.ToString());
                fieldViewModel.Type = ViewModels.FieldType.Single;
                if (catetory == ViewModels.FieldCategory.MemberType)
                    fieldViewModel.FieldName = "類型";
                dt = db.GetDataTable(Sql, paraList);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ViewModels.MemberFieldStatisticViewModel fieldStatisticViewModel = new ViewModels.MemberFieldStatisticViewModel()
                        {
                            FieldValue = dt.Rows[i][0].ToString() == "" ? "未填" : dt.Rows[i][0].ToString(),
                            Count = long.Parse(dt.Rows[i][1].ToString()),
                            Percentage = 0
                        };
                        if (catetory == ViewModels.FieldCategory.MemberType)
                        {
                            fieldStatisticViewModel.FieldValue = GetMemberTypeName((WorkV3.Models.MemberType)int.Parse(dt.Rows[i][0].ToString()));
                        }
                        fieldViewModel.StatisticList.Add(fieldStatisticViewModel);
                    }
                    long TotalCount = fieldViewModel.StatisticList.Sum(p => p.Count);
                    if (TotalCount > 0)
                    {
                        for (int i = 0; i < fieldViewModel.StatisticList.Count; i++)
                        {
                            fieldViewModel.StatisticList[i].Percentage = decimal.Round(100 * fieldViewModel.StatisticList[i].Count / TotalCount, 2);
                        }
                    }
                }
                else
                {
                    if (catetory == ViewModels.FieldCategory.MemberType)
                    {
                        ViewModels.MemberFieldStatisticViewModel fieldStatisticViewModel = new ViewModels.MemberFieldStatisticViewModel()
                        {
                            FieldValue = GetMemberTypeName(WorkV3.Models.MemberType.Web),
                            Count = 0,
                            Percentage = 0
                        };
                        fieldViewModel.StatisticList.Add(fieldStatisticViewModel);
                        ViewModels.MemberFieldStatisticViewModel fieldStatisticViewModel2 = new ViewModels.MemberFieldStatisticViewModel()
                        {
                            FieldValue = GetMemberTypeName(WorkV3.Models.MemberType.FB),
                            Count = 0,
                            Percentage = 0
                        };
                        fieldViewModel.StatisticList.Add(fieldStatisticViewModel2);
                    }
                    if (catetory == ViewModels.FieldCategory.Sex)
                    {
                        ViewModels.MemberFieldStatisticViewModel fieldStatisticViewModel = new ViewModels.MemberFieldStatisticViewModel()
                        {
                            FieldValue = "男",
                            Count = 0,
                            Percentage = 0
                        };
                        fieldViewModel.StatisticList.Add(fieldStatisticViewModel);
                        ViewModels.MemberFieldStatisticViewModel fieldStatisticViewModel2 = new ViewModels.MemberFieldStatisticViewModel()
                        {
                            FieldValue = "女",
                            Count = 0,
                            Percentage = 0
                        };
                        fieldViewModel.StatisticList.Add(fieldStatisticViewModel2);
                    }
                }

            }
            else if (catetory == ViewModels.FieldCategory.OrderEpaper)
            {
                Sql = string.Format(Sql_Checked, catetory.ToString());
                fieldViewModel.Type = ViewModels.FieldType.Check;
                dt = db.GetDataTable(Sql, paraList);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ViewModels.MemberFieldStatisticViewModel fieldStatisticViewModel_Checked = new ViewModels.MemberFieldStatisticViewModel()
                        {
                            FieldValue = "訂閱",
                            Count = 0,
                            Percentage = 0
                        };
                        if (!string.IsNullOrEmpty(dt.Rows[i][0].ToString()))
                            fieldStatisticViewModel_Checked.Count = long.Parse(dt.Rows[i][0].ToString());
                        ViewModels.MemberFieldStatisticViewModel fieldStatisticViewModel_NonChecked = new ViewModels.MemberFieldStatisticViewModel()
                        {
                            FieldValue = "未訂閱",
                            Count = 0,
                            Percentage = 0
                        };
                        if (!string.IsNullOrEmpty(dt.Rows[i][1].ToString()))
                            fieldStatisticViewModel_NonChecked.Count = long.Parse(dt.Rows[i][1].ToString());
                        fieldViewModel.StatisticList.Add(fieldStatisticViewModel_Checked);
                        fieldViewModel.StatisticList.Add(fieldStatisticViewModel_NonChecked);
                    }
                    long TotalCount = fieldViewModel.StatisticList.Sum(p => p.Count);
                    if (TotalCount > 0)
                    {
                        for (int i = 0; i < fieldViewModel.StatisticList.Count; i++)
                        {
                            fieldViewModel.StatisticList[i].Percentage = decimal.Round(100 * fieldViewModel.StatisticList[i].Count / TotalCount, 2);
                        }
                    }
                }
            }
            else if (catetory == ViewModels.FieldCategory.Address)
            {
                Sql = Sql_Input_Address;
                fieldViewModel.Type = ViewModels.FieldType.Text;
                dt = db.GetDataTable(Sql, paraList);
                ViewModels.MemberFieldStatisticViewModel fieldStatisticViewModel_Valued = new ViewModels.MemberFieldStatisticViewModel()
                {
                    FieldValue = "已填",
                    Count = 0,
                    Percentage = 0
                };
                ViewModels.MemberFieldStatisticViewModel fieldStatisticViewModel_NoneValued = new ViewModels.MemberFieldStatisticViewModel()
                {
                    FieldValue = "未填",
                    Count = 0,
                    Percentage = 0
                };
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
                    {
                        //fieldStatisticViewModel_Valued.FieldValue += long.Parse(dt.Rows[0][0].ToString());
                        fieldStatisticViewModel_Valued.Count = long.Parse(dt.Rows[0][0].ToString());
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[0][1].ToString()))
                    {
                        //fieldStatisticViewModel_NoneValued.FieldValue += long.Parse(dt.Rows[0][1].ToString());
                        fieldStatisticViewModel_NoneValued.Count = long.Parse(dt.Rows[0][1].ToString());
                    }
                    long TotalCount = fieldStatisticViewModel_Valued.Count + fieldStatisticViewModel_NoneValued.Count;
                    if (TotalCount > 0)
                    {
                        fieldStatisticViewModel_Valued.Percentage = decimal.Round(100 * fieldStatisticViewModel_Valued.Count / TotalCount, 2);
                        fieldStatisticViewModel_NoneValued.Percentage = decimal.Round(100 * fieldStatisticViewModel_NoneValued.Count / TotalCount, 2);
                    }
                }
                fieldViewModel.StatisticList.Add(fieldStatisticViewModel_Valued);
                fieldViewModel.StatisticList.Add(fieldStatisticViewModel_NoneValued);
            }
            else
            {
                if(catetory != ViewModels.FieldCategory.SerialNumber)
                {
                    Sql = string.Format(Sql_Input, catetory.ToString());
                    fieldViewModel.Type = ViewModels.FieldType.Text;
                    dt = db.GetDataTable(Sql, paraList);
                    ViewModels.MemberFieldStatisticViewModel fieldStatisticViewModel_Valued = new ViewModels.MemberFieldStatisticViewModel()
                    {
                        FieldValue = "已填",
                        Count = 0,
                        Percentage = 0
                    };
                    ViewModels.MemberFieldStatisticViewModel fieldStatisticViewModel_NoneValued = new ViewModels.MemberFieldStatisticViewModel()
                    {
                        FieldValue = "未填",
                        Count = 0,
                        Percentage = 0
                    };
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
                        {
                            fieldStatisticViewModel_Valued.Count = long.Parse(dt.Rows[0][0].ToString());
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[0][1].ToString()))
                        {
                            fieldStatisticViewModel_NoneValued.Count = long.Parse(dt.Rows[0][1].ToString());
                        }
                        long TotalCount = fieldStatisticViewModel_Valued.Count + fieldStatisticViewModel_NoneValued.Count;
                        if (TotalCount > 0)
                        {
                            fieldStatisticViewModel_Valued.Percentage = decimal.Round(100 * fieldStatisticViewModel_Valued.Count / TotalCount, 2);
                            fieldStatisticViewModel_NoneValued.Percentage = decimal.Round(100 * fieldStatisticViewModel_NoneValued.Count / TotalCount, 2);
                        }
                    }
                    fieldViewModel.StatisticList.Add(fieldStatisticViewModel_Valued);
                    fieldViewModel.StatisticList.Add(fieldStatisticViewModel_NoneValued);
                }
                
            }
            return fieldViewModel;
        }

        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static ViewModels.MemberFieldViewModel GetCategoryStatisticsValue(ViewModels.FieldCategory catetory, string fieldName, DateTime startDate, DateTime endDate)
        {
            string IdentityType = WorkV3.Areas.Backend.Models.DataAccess.CategoryType.Identity.ToString();
            string FavorityType = WorkV3.Areas.Backend.Models.DataAccess.CategoryType.Favority.ToString();
            string CareerType = WorkV3.Areas.Backend.Models.DataAccess.CategoryType.Career.ToString();
            string EducationType = WorkV3.Areas.Backend.Models.DataAccess.CategoryType.Education.ToString();
            string MarriageType = WorkV3.Areas.Backend.Models.DataAccess.CategoryType.Marriage.ToString();
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@StartDate", startDate);
            paraList.Add("@EndDate", endDate);
            
            ViewModels.MemberFieldViewModel fieldViewModel = new ViewModels.MemberFieldViewModel();
            fieldViewModel.Category = catetory;
            fieldViewModel.FieldName = fieldName;
            paraList.Add("@Type", catetory.ToString());
            
            fieldViewModel.StatisticList = new List<ViewModels.MemberFieldStatisticViewModel>();
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable dt = new DataTable();
           
            if (catetory == ViewModels.FieldCategory.Career ||
                catetory == ViewModels.FieldCategory.Education ||
                catetory == ViewModels.FieldCategory.Marriage)
            {
                string columnName = catetory.ToString();
                string Sql = string.Format(@"  SELECT * FROM (
								        SELECT CASE WHEN Categories.Title IS NOT NULL THEN Categories.Title ELSE '' END AS Title, {0},
								        SUM(1) AS Counts FROM MemberShip 
								        LEFT JOIN Categories ON MemberShip.{0}=Categories.ID AND MemberShip.{0}<>''
								        WHERE Convert(nvarchar(10), MemberShip.CreateTime, 111)>=@StartDate AND Convert(nvarchar(10), MemberShip.CreateTime, 111)<=@EndDate 
								        AND {0}<>''
								        GROUP BY Title, {0}
								        UNION 
								        SELECT '未填' AS Title, '0' AS {0},
								        SUM(1) AS Counts FROM MemberShip 
								        LEFT JOIN Categories ON MemberShip.{0}=Categories.ID AND MemberShip.{0}<>''
								        WHERE Convert(nvarchar(10), MemberShip.CreateTime, 111)>=@StartDate AND Convert(nvarchar(10), MemberShip.CreateTime, 111)<=@EndDate 
								        AND {0}=''
								        GROUP BY Title, {0} ) 
                                        AS Result ORDER BY {0}", columnName);
                dt = db.GetDataTable(Sql, paraList);
            }
            else
            {
                string Sql = @" SELECT Categories.Title ,MemberShipSetting.CategoryID, SUM(1) AS Count FROM MemberShipSetting 
                                LEFT JOIN Categories ON Categories.ID=MemberShipSetting.CategoryID
                                WHERE MemberShipID IN (SELECT ID FROM MemberShip WHERE Convert(nvarchar(10), CreateTime, 111)>=@StartDate AND Convert(nvarchar(10), CreateTime, 111)<=@EndDate )
                                    AND CategoryID IN (SELECT ID FROM Categories) AND MemberShipSetting.Type=@Type
                                GROUP BY Categories.Title, MemberShipSetting.CategoryID";
                dt = db.GetDataTable(Sql, paraList);
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ViewModels.MemberFieldStatisticViewModel fieldStatisticViewModel = new ViewModels.MemberFieldStatisticViewModel()
                    {
                        FieldValue = dt.Rows[i][0].ToString() == "" ? "未填" : dt.Rows[i][0].ToString(),
                        Count = long.Parse(dt.Rows[i][2].ToString()),
                        Percentage = 0
                    };
                    fieldViewModel.StatisticList.Add(fieldStatisticViewModel);
                }
                long TotalCount = fieldViewModel.StatisticList.Sum(p => p.Count);
                if (TotalCount > 0)
                {
                    for (int i = 0; i < fieldViewModel.StatisticList.Count; i++)
                    {
                        fieldViewModel.StatisticList[i].Percentage = decimal.Round(100 * fieldViewModel.StatisticList[i].Count / TotalCount, 2);
                    }
                }
            }
            else
            {

                string Sql = @" SELECT Title ,ID, 0 AS Count FROM Categories
                                WHERE Type=@Type ORDER BY Sort ";
                dt = db.GetDataTable(Sql, paraList);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ViewModels.MemberFieldStatisticViewModel fieldStatisticViewModel = new ViewModels.MemberFieldStatisticViewModel()
                        {
                            FieldValue = dt.Rows[i][0].ToString(),
                            Count = 0,
                            Percentage = 0
                        };
                        fieldViewModel.StatisticList.Add(fieldStatisticViewModel);
                    }
                }
            }
            return fieldViewModel;
        }
        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static ViewModels.MemberFieldViewModel GetOrderEpaperStatisticsValue(ViewModels.FieldCategory catetory, string fieldName, DateTime startDate, DateTime endDate)
        {
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@StartDate", startDate);
            paraList.Add("@EndDate", endDate);

            ViewModels.MemberFieldViewModel fieldViewModel = new ViewModels.MemberFieldViewModel();
            fieldViewModel.Category = catetory;
            fieldViewModel.FieldName = fieldName;
            paraList.Add("@Type", catetory.ToString());
            fieldViewModel.StatisticList = new List<ViewModels.MemberFieldStatisticViewModel>();
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable dt = new DataTable();
            string Sql = @" SELECT '未訂閱' AS Name, Count(1) AS OrderCount, -1 AS Sort
                                FROM MemberShip WHERE ID NOT IN 
                                (
	                                SELECT Member_ID FROM EpaperSubscriber 
                                ) AND Convert(nvarchar(10), CreateTime, 111)>=@StartDate AND Convert(nvarchar(10), CreateTime, 111)<=@EndDate 
                                UNION 
                                SELECT etype.Name, 
	                                case when counts.OrderCount is not null then counts.OrderCount else 0 end as OrderCount ,
	                                etype.Sort
	                                FROM EpaperType etype
                                LEFT JOIN 
                                (
                                SELECT EpaperType_ID, OrderCount FROM (
                                SELECT EpaperType_ID, Count(1) AS OrderCount FROM EpaperSubscriberDetail
                                WHERE EpaperSubscriber_ID IN 
                                (
                                    SELECT ID FROM EpaperSubscriber WHERE 
	                                Member_ID in (select ID FROM MemberShip WHERE Convert(nvarchar(10), CreateTime, 111)>=@StartDate AND Convert(nvarchar(10), CreateTime, 111)<=@EndDate ) 
                                )
                                GROUP BY EpaperType_ID
                                ) AS Stat 
                                ) AS counts ON counts.EpaperType_ID=etype.ID
                                ORDER BY Sort ";
            dt = db.GetDataTable(Sql, paraList);

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ViewModels.MemberFieldStatisticViewModel fieldStatisticViewModel = new ViewModels.MemberFieldStatisticViewModel()
                    {
                        FieldValue = dt.Rows[i][0].ToString() == "" ? "未訂閱" : dt.Rows[i][0].ToString(),
                        Count = long.Parse(dt.Rows[i][1].ToString()),
                        Percentage = 0
                    };
                    fieldViewModel.StatisticList.Add(fieldStatisticViewModel);
                }
                long TotalCount = fieldViewModel.StatisticList.Sum(p => p.Count);
                if (TotalCount > 0)
                {
                    for (int i = 0; i < fieldViewModel.StatisticList.Count; i++)
                    {
                        fieldViewModel.StatisticList[i].Percentage = decimal.Round(100 * fieldViewModel.StatisticList[i].Count / TotalCount, 2);
                    }
                }
            }
            return fieldViewModel;
        }
        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static ViewModels.MemberFieldViewModel GetBirthdayStatisticsValue(ViewModels.FieldCategory catetory, string fieldName, DateTime startDate, DateTime endDate)
        {
            Dictionary<string, int[]> ageList = new Dictionary<string, int[]>();
            ageList.Add("6歲以下", new int[] { int.MinValue, 6 });
            ageList.Add("7~12歲", new int[] { 7, 12 });
            ageList.Add("13~15歲", new int[] { 13, 15 });
            ageList.Add("16~18歲", new int[] { 16, 18 });
            ageList.Add("19~20歲", new int[] { 19, 20 });
            ageList.Add("21~24歲", new int[] { 21, 24 });
            ageList.Add("25~29歲", new int[] { 25, 29 });
            ageList.Add("30~39歲", new int[] { 30, 39 });
            ageList.Add("40~54歲", new int[] { 40, 54 });
            ageList.Add("55~64歲", new int[] { 55, 64 });
            ageList.Add("65~80歲", new int[] { 65, 80 });
            ageList.Add("81歲以上", new int[] { 81, int.MaxValue });
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@StartDate", startDate);
            paraList.Add("@EndDate", endDate);

            ViewModels.MemberFieldViewModel fieldViewModel = new ViewModels.MemberFieldViewModel();
            fieldViewModel.Category = catetory;
            fieldViewModel.FieldName = fieldName;
            paraList.Add("@Type", catetory.ToString());

            fieldViewModel.StatisticList = new List<ViewModels.MemberFieldStatisticViewModel>();
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable dt = new DataTable();

            string cond = "";

            string Sql = @" SELECT SUM(1) AS Count FROM MemberShip WHERE {0}";
            //未填
            cond = " (Birthday='' OR Birthday IS NULL) AND Convert(nvarchar(10), MemberShip.CreateTime, 111)>=@StartDate AND Convert(nvarchar(10), MemberShip.CreateTime, 111)<=@EndDate  ";
            dt = db.GetDataTable(string.Format(Sql, cond), paraList);

            ViewModels.MemberFieldStatisticViewModel fieldStatisticViewModel = new ViewModels.MemberFieldStatisticViewModel()
            {
                FieldValue = "未填",
                Count = 0,
                Percentage = 0
            };
            if (dt != null && dt.Rows.Count > 0 && !string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
            {
                fieldStatisticViewModel.Count = long.Parse(dt.Rows[0][0].ToString());
            }
            fieldViewModel.StatisticList.Add(fieldStatisticViewModel);
            foreach (string keyName in ageList.Keys)
            {
                // 6歲以下
                cond = " (Birthday<>'' AND Birthday IS NOT NULL) AND Convert(nvarchar(10), MemberShip.CreateTime, 111)>=@StartDate AND Convert(nvarchar(10), MemberShip.CreateTime, 111)<=@EndDate";
                if (ageList[keyName][0] != int.MinValue)
                    cond += " AND year(convert(datetime, Birthday))<=year(getdate())-" + ageList[keyName][0];
                if (ageList[keyName][1] != int.MaxValue)
                    cond += " and year(convert(datetime, Birthday))>=year(getdate())-" + ageList[keyName][1];
                dt = db.GetDataTable(string.Format(Sql, cond), paraList);
                ViewModels.MemberFieldStatisticViewModel fieldAgeViewModelTerm = new ViewModels.MemberFieldStatisticViewModel()
                {
                    FieldValue = keyName,
                    Count = 0,
                    Percentage = 0
                };
                if (dt != null && dt.Rows.Count > 0 && !string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
                {
                    fieldAgeViewModelTerm.Count = long.Parse(dt.Rows[0][0].ToString());
                }
                fieldViewModel.StatisticList.Add(fieldAgeViewModelTerm);
            }

            long TotalCount = fieldViewModel.StatisticList.Sum(p => p.Count);
            if (TotalCount > 0)
            {
                for (int i = 0; i < fieldViewModel.StatisticList.Count; i++)
                {
                    fieldViewModel.StatisticList[i].Percentage = decimal.Round(100 * fieldViewModel.StatisticList[i].Count / TotalCount, 2);
                }
            }
            return fieldViewModel;
        }
        public static string GetMemberTypeName(WorkV3.Models.MemberType memberType)
{
    switch (memberType)
    {
        case WorkV3.Models.MemberType.FB:
            return "FB";
        case WorkV3.Models.MemberType.Web:
            return "網站";
        default:
            return "undefined";
    }
}
/// <summary>
/// 取得統計資料
/// </summary>
/// <returns></returns>
public static IEnumerable<ViewModels.MemberFieldViewModel> GetMemberFieldStatistics(long SiteID, DateTime startDate, DateTime endDate)
        {
            List<Models.MemberShipRegColumnSetModels> columnList = Models.DataAccess.MemberShipRegSetDAO.GetColumnItems(SiteID);
            List<ViewModels.MemberFieldViewModel> statisticlList = new List<ViewModels.MemberFieldViewModel>();
            statisticlList.Add(GetStatisticsValue(ViewModels.FieldCategory.MemberType, "類型", startDate, endDate));
            foreach (Models.MemberShipRegColumnSetModels columnModel in columnList)
            {
                if (!columnModel.IsOpen)
                    continue;
                ViewModels.FieldCategory fieldCategory = (ViewModels.FieldCategory)Enum.Parse(typeof(ViewModels.FieldCategory), columnModel.ColumnName, false);
                if (fieldCategory == ViewModels.FieldCategory.Career ||
                    fieldCategory == ViewModels.FieldCategory.Education ||
                    fieldCategory == ViewModels.FieldCategory.Marriage ||
                    fieldCategory == ViewModels.FieldCategory.Favority ||
                    fieldCategory == ViewModels.FieldCategory.Identity)
                {
                    statisticlList.Add(GetCategoryStatisticsValue(fieldCategory, columnModel.ColumnTitle, startDate, endDate));
                }
                else if(fieldCategory == ViewModels.FieldCategory.Birthday)
                {
                    statisticlList.Add(GetBirthdayStatisticsValue(fieldCategory, columnModel.ColumnTitle, startDate, endDate));
                }

                else if (fieldCategory == ViewModels.FieldCategory.OrderEpaper)
                {
                    statisticlList.Add(GetOrderEpaperStatisticsValue(fieldCategory, columnModel.ColumnTitle, startDate, endDate));
                }
                else
                {
                    statisticlList.Add(GetStatisticsValue(fieldCategory, columnModel.ColumnTitle, startDate, endDate));
                }
            }
            //statisticlList.Add(GetStatisticsValue(ViewModels.FieldCategory.MemberType, startDate, endDate));
            //statisticlList.Add(GetStatisticsValue(ViewModels.FieldCategory.Name, startDate, endDate));
            //statisticlList.Add(GetStatisticsValue(ViewModels.FieldCategory.Gender, startDate, endDate));
            //statisticlList.Add(GetStatisticsValue(ViewModels.FieldCategory.Email, startDate, endDate));
            //statisticlList.Add(GetCategoryStatisticsValue(ViewModels.FieldCategory.IdentityCategory, startDate, endDate));
            //statisticlList.Add(GetCategoryStatisticsValue(ViewModels.FieldCategory.FavorityCategory, startDate, endDate));
            return statisticlList;
        }
    }
}