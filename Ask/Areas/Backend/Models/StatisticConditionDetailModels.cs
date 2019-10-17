using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class StatisticConditionDetailModels
    {
        public string ID { get; set; }
        public string ConditionID { get; set; }
        public LogicType LogicType { get; set; } = LogicType.None;
        public string LogicTypeName { get; set; }
        public AnalysisType AnalysisType { get; set; } = AnalysisType.Page;
        public string AnalysisTypeName { get; set; }
        public string AnalysisItems { get; set; }
        public string AnalysisItemsContent { get; set; }
        public int Sort { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public string GetLogicTypeName(LogicType logicType)
        {
            if (logicType == LogicType.None)
                return "";
            return logicType.ToString();
        }
        public string GetAnalysisItemsContent(long siteID, AnalysisType analysisType, string analysisItems)
        {
            switch (analysisType)
            {
                case AnalysisType.Page:
                    if (string.IsNullOrEmpty(analysisItems))
                        return "全部選單";
                    else
                    {
                        string pathInfo = "";
                        string[] pagesArray = analysisItems.Split(';');
                        for (int i = 0; i < pagesArray.Length; i++)
                        {
                            //if (!string.IsNullOrEmpty(pagesArray[i]))
                            //{
                                MenusModels menu = DataAccess.StatisticConditionDAO.GetMenuInfo(long.Parse(pagesArray[i]));
                            if (menu == null)
                            {
                                PagesModels page = DataAccess.PagesDAO.GetPageInfo(siteID, long.Parse(pagesArray[i]));
                                if (page != null)
                                {
                                    pathInfo += (string.IsNullOrEmpty(pathInfo) ? "" : ">") + page.Title;
                                }
                            }
                            else
                                pathInfo += (string.IsNullOrEmpty(pathInfo) ? "" : ">") + menu.Title;
                            //}
                        }
                        return pathInfo;
                    }
                case AnalysisType.Age:
                    if (string.IsNullOrEmpty(analysisItems))
                        return "全部";
                    else
                    {
                        string ageInfo = "";
                        if (analysisItems.Contains("other"))
                        {
                            ageInfo += string.IsNullOrEmpty(ageInfo) ? "" : "、";
                            string[] ages =  analysisItems.Split(':');
                            ageInfo = "自訂:" + ages[1];
                        }
                        string[] pagesArray = analysisItems.Split(';');
                        for (int i = 0; i < pagesArray.Length; i++)
                        {
                            if (ViewModels.AnalysisPageLogViewModel.Ages.Keys.Contains(pagesArray[i]))
                            {
                                ageInfo += string.IsNullOrEmpty(ageInfo) ? "" : "、";
                                if (ViewModels.AnalysisPageLogViewModel.Ages[pagesArray[i]][0] == 0)
                                {
                                    ageInfo += string.Format("{0}歲以下",
                                        ViewModels.AnalysisPageLogViewModel.Ages[pagesArray[i]][1].ToString());

                                }
                                else if (ViewModels.AnalysisPageLogViewModel.Ages[pagesArray[i]][1] == 99)
                                {
                                    ageInfo += string.Format("{0}歲以上",
                                        ViewModels.AnalysisPageLogViewModel.Ages[pagesArray[i]][0].ToString());

                                }
                                else
                                {
                                    ageInfo += string.Format("{0}-{1}歲",
                                        ViewModels.AnalysisPageLogViewModel.Ages[pagesArray[i]][0].ToString(),
                                        ViewModels.AnalysisPageLogViewModel.Ages[pagesArray[i]][1].ToString());
                                }
                            }
                        }
                        return ageInfo;
                    }
                case AnalysisType.Career:
                case AnalysisType.Education:
                case AnalysisType.Favority:
                case AnalysisType.Identity:
                case AnalysisType.Marriage:
                    if (string.IsNullOrEmpty(analysisItems))
                        return "全部";
                    else
                    {
                        string itemInfo = "";
                        string[] pagesArray = analysisItems.Split(';');
                        for (int i = 0; i < pagesArray.Length; i++)
                        {
                            CategoryModels cate = DataAccess.CategoryDAO.GetItem(long.Parse(pagesArray[i]));

                            if (cate!= null)
                            {
                                itemInfo += string.IsNullOrEmpty(itemInfo) ? "" : "、";
                                itemInfo += cate.Title;
                            }
                        }
                        return itemInfo;
                    }
                case AnalysisType.Location:
                    int[] regionList = null;
                    if (analysisItems != string.Empty)
                        regionList = Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(analysisItems);
                    var regions = WorkV3.Models.DataAccess.WorldRegionDAO.GetRegions(regionList);
                    if (regions != null && regions.Count() > 0)
                    {
                        string itemInfo = "";
                        for (int i = 0; i < regions.Count(); i++)
                        {
                            itemInfo += regions.ElementAt(i).Name;
                        }
                        return itemInfo;
                    }
                    return analysisItems;
                case AnalysisType.Sex:
                case AnalysisType.Browser:
                case AnalysisType.Machine:
                    if (string.IsNullOrEmpty(analysisItems))
                        return "全部";
                    else
                    {
                        string itemInfo = "";
                        string[] pagesArray = analysisItems.Split(';');
                        return string.Join("、", pagesArray); 
                    }
                case AnalysisType.Manager:
                    if (string.IsNullOrEmpty(analysisItems))
                        return "全部";
                    else
                    {
                        string itemInfo = "";
                        string[] pagesArray = analysisItems.Split(';');
                        for (int i = 0; i < pagesArray.Length; i++)
                        {
                            MemberModels manager = DataAccess.ManagerDAO.GetItem(long.Parse(pagesArray[i]));

                            if (manager != null)
                            {
                                itemInfo += string.IsNullOrEmpty(itemInfo) ? "" : "、";
                                itemInfo += manager.Name;
                            }
                            if (i >= 3)
                            {
                                itemInfo += "...(" + pagesArray.Length.ToString() + ")";
                                break;
                            }
                        }
                        return itemInfo;
                    }
                case AnalysisType.OrderEpaper:
                    break;
            }
            return "";
        }
        public string GetAnalysisTypeName(AnalysisType AnalysisType)
        {
            switch (AnalysisType)
            {
                case AnalysisType.Page:
                    return "選單";
                case AnalysisType.Manager:
                    return "權限群組";
                case AnalysisType.Machine:
                    return "裝置";
                case AnalysisType.Browser:
                    return "瀏覽器";
                case AnalysisType.Sex:
                    return "性別";
                case AnalysisType.Age:
                    return "年紀";
                case AnalysisType.Location:
                    return "地點";
                case AnalysisType.Marriage:
                    return "婚姻狀況";
                case AnalysisType.Education:
                    return "學歷";
                case AnalysisType.Career:
                    return "職業";
                case AnalysisType.Identity:
                    return "身份";
                case AnalysisType.Favority:
                    return "喜好";
                case AnalysisType.OrderEpaper:
                    return "訂閱電子報";
            }
            return "?";
        }
    }
    public enum LogicType
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// AND
        /// </summary>
        AND = 1 ,
        /// <summary>
        /// OR
        /// </summary>
        OR  = 2
    }
    public enum AnalysisType
    {
        /// <summary>
        /// 選單
        /// </summary>
        Page = 0,
        /// <summary>
        /// 權限群組
        /// </summary>
        Manager = 1,
        /// <summary>
        /// 裝置
        /// </summary>
        Machine = 2,
        /// <summary>
        /// 瀏覽器
        /// </summary>
        Browser = 3,
        /// <summary>
        /// 性別
        /// </summary>
        Sex = 4,
        /// <summary>
        /// 年紀
        /// </summary>
        Age = 5,
        /// <summary>
        /// 地點
        /// </summary>
        Location = 6,
        /// <summary>
        /// 婚姻狀況
        /// </summary>
        Marriage = 7,
        /// <summary>
        /// 學歷
        /// </summary>
        Education = 8,
        /// <summary>
        /// 職業
        /// </summary>
        Career = 9,
        /// <summary>
        /// 身份
        /// </summary>
        Identity = 10,
        /// <summary>
        /// 喜好
        /// </summary>
        Favority = 11,
        /// <summary>
        /// 訂閱電子報
        /// </summary>
        OrderEpaper = 12
    }
}