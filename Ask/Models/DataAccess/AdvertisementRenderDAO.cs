using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using WorkV3.Areas.Backend.Models;

namespace WorkV3.Models.DataAccess
{
    /// <summary>
    /// 產生廣告
    /// </summary>
    public class AdvertisementRenderDAO
    {
        long _SiteID;
        long _PageNo;
        long _MenuID;
        string _DataType;
        int _StyleID = 1;
        int _CardStyleID = 1;
        public AdvertisementRenderDAO(long SiteID, long PageNo)
        {
            _SiteID = SiteID;
            _PageNo = PageNo;
            GetMenuIDAndDataType(SiteID, PageNo);
        }
        /// <summary>
        /// 根據DataType調整ads主檔內容
        /// For 單一DataType 多個廣告位置設定
        /// </summary>
        /// <param name="ads"></param>
        private void RenderWithDataType(ref IEnumerable<AdvertisementRenderIndex> ads)
        {
            if (string.IsNullOrWhiteSpace(_DataType))
                return;

            if (ads == null)
                return;

            switch (_DataType)
            {
                case WorkV3.Areas.Backend.Models.AreaSetDataType.Article:
                    if (AdvertisementRenderTools.CheckPostitionIsChild(_PageNo))
                    {
                        // 20180703 neil 因廣告區選擇已由主從式架構改為獨立選擇，所以移除 parentAreaSetID 部份
                        if(ads.Any(m => m.AreaSetChildType == WorkV3.Areas.Backend.Models.ChildType.Inside))
                        {
                            ads = ads.Where(m => m.AreaSetChildType == WorkV3.Areas.Backend.Models.ChildType.Inside);
                            return;
                        }
                    }
                    else
                    {
                        ads = ads.Where(m => m.AreaSetChildType == "");
                    }
                    break;
                case WorkV3.Areas.Backend.Models.AreaSetDataType.Event:
                    if (AdvertisementRenderTools.CheckPostitionIsChild(_PageNo))
                    {
                        if (ads.Any(m => m.AreaSetChildType == WorkV3.Areas.Backend.Models.ChildType.Inside))
                        {
                            ads = ads.Where(m => m.AreaSetChildType == WorkV3.Areas.Backend.Models.ChildType.Inside);
                            return;
                        }
                    }
                    else
                    {
                        ads = ads.Where(m => m.AreaSetChildType == "");
                    }
                    break;
                case WorkV3.Areas.Backend.Models.AreaSetDataType.Member:
                    string sn = AdvertisementRenderTools.QueryPageSNByPageNo(_PageNo);
                    if (!string.IsNullOrWhiteSpace(sn) && AdvertisementRenderTools.CheckChildTypeIsExist(sn))
                    {
                        //會員位置設定中, 當dbo.ChildType = null, 代表存會員主頁。在dbo.Pages當中, 會員主頁的SN為Desktop
                        //所以不是Desktop才需要去讀取ChildType部分
                        if (sn != WorkV3.Areas.Backend.Models.ChildType.Desktop)
                        {
                            ads = ads.Where(m => m.AreaSetChildType == WorkV3.Areas.Backend.Models.ChildType.Login);
                            return;
                        }
                        else
                        {
                            ads = ads.Where(m => m.AreaSetChildType == "");
                        }
                    }
                    else
                    {
                        ads = new List<AdvertisementRenderIndex>();
                    }
                    break;
            }
            //ads = ads.Any(m => m.ParentAreaSetID == null) ? ads.Where(m => m.ParentAreaSetID == null) : ads;
        }
        /// <summary>
        /// 插入廣告Zone
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public List<ZonesModels> InsertAdsZones(List<ZonesModels> datas)
        {
            // 備份
            var bk = datas; 
            if (string.IsNullOrEmpty(_DataType))
                return bk;

            //排序原有資料的Sort 並且和TempSort同步
            datas = datas.OrderBy(m => m.Sort).ToList(); 
            for (int i = 0; i < datas.Count; i++)
            {
                datas[i].TempSort = i + 1;
            }

            //取得廣告區主檔資訊
            IEnumerable<AdvertisementRenderIndex> ads = AdvertisementRenderTools.GetAdvertisementRender(_MenuID);
            //根據DataType調整ads主檔內容
            RenderWithDataType(ref ads);
            foreach (var item in ads)
            {
                int Position; //參考點 (同indexStr)
                string align; //位置代碼 ex: C or R or A
                string indexStr = AdvertisementRenderTools.GetPositionIndex(item.GroupPosition, _DataType);

                #region 檢查資料
                align = indexStr.Substring(0, 1);

                if (!string.IsNullOrWhiteSpace(indexStr))
                    indexStr = indexStr.Substring(1); //去位置代碼
                else
                    continue;

                if (!int.TryParse(indexStr, out Position))
                    continue;
                #endregion

                //20180509 上下橫幅的部分 Zone的StyleID要用6
                if (item.GroupPosition == DisplayAreaShardType.Top || item.GroupPosition == DisplayAreaShardType.Bottom)
                    _StyleID = 6;

                switch (_DataType)
                {
                    case WorkV3.Areas.Backend.Models.DataType.Intro: //首頁
                        if (Position == 3 || Position == 8)
                        {
                            if (Position == 3)
                                datas.Insert(0, GenZoneModel(item, Position));
                            else if (Position == 8)
                                datas.Insert(0, GenZoneModel(item, datas.Count >= 2 ? datas.Count - 2 : 0));

                            AdjustmentSort(datas, datas.Count >= 2 ? datas.Count - 2 : Position);
                            continue;
                        }
                        break;
                    case WorkV3.Areas.Backend.Models.DataType.Event: //活動
                        if(align == "R") //右側廣告
                        {
                            SetRightSideAdCard(datas, item, Position, WorkV3.Areas.Backend.Models.DataType.Event);
                            continue;
                        }
                        break;
                    case WorkV3.Areas.Backend.Models.DataType.Article: //文章
                        if (align == "R") //右側廣告
                        {
                            SetRightSideAdCard(datas, item, Position, WorkV3.Areas.Backend.Models.DataType.Article);
                            continue;
                        }
                        break;
                    case WorkV3.Areas.Backend.Models.DataType.ArticleIntro: //單頁

                        if (align == "R") //右側廣告
                        {
                            SetRightSideAdCard(datas, item, Position, WorkV3.Areas.Backend.Models.DataType.ArticleIntro);
                            continue;
                        }

                        //if (Position == 4 || Position == 8)
                        //{
                        //    if (Position == 4)
                        //        datas.Insert(0, GenZoneModel(item, Position));
                        //    else if (Position == 8)
                        //        datas.Insert(0, GenZoneModel(item, datas.Count >= 2 ? datas.Count - 2 : 0));
                        //    AdjustmentSort(datas, datas.Count >= 2 ? datas.Count - 2 : Position);
                        //    continue;
                        //}
                        break;
                }

                double enterIndex = SetReferencePoint(datas, Position);
                datas.Insert(0, GenZoneModel(item, enterIndex));
                AdjustmentSort(datas, enterIndex, Position);
            }

            datas = datas.OrderBy(m => m.TempSort).ToList();
            return datas;
        }
        /// <summary>
        /// 設定右側廣告Card
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="item"></param>
        /// <param name="Position"></param>
        private void SetRightSideAdCard(List<ZonesModels> datas, AdvertisementRenderIndex item, int Position,string TargetDataType)
        {
            foreach (var zone in datas)
            {
                long cardNo = AdvertisementRenderTools.QueryCardNoByZoneNoAndTypeStr(zone.No, TargetDataType);
                if (cardNo != 0)
                {
                    ZonesModels zonemodel = GenZoneModel(item, Position);
                    CardsModels cardmodel = AdvertisementRenderTools.GenCard(zonemodel).FirstOrDefault();

                    if (zone.CardsModels == null)
                        zone.CardsModels = new List<CardsModels>();

                    zone.CardsModels.Add(cardmodel);
                    zone.StyleID = 9;
                    break;
                }
            }
        }
        /// <summary>
        /// 參考點設定
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="Position"></param>
        /// <returns></returns>
        private double SetReferencePoint(List<ZonesModels> datas, int Position)
        {
            int[] ConVenCardIndex = GetConVenCardIndex(datas);
            double enterIndex = datas.Count * (double)Position / 10; //從position參數得出位置參考點 ex:置中會得到0.5。代表在原始20個zone當中,從第10個(20*0.5)插入
            if (enterIndex < datas.ElementAt(ConVenCardIndex[1]).TempSort && enterIndex < datas.ElementAt(ConVenCardIndex[2]).TempSort)
                enterIndex = enterIndex + 1;
            return enterIndex;
        }
        /// <summary>
        /// 取得常規Card在Zones當中的索引值
        /// </summary>
        /// <param name="zones"></param>
        /// <returns></returns>
        private int[] GetConVenCardIndex(List<ZonesModels> zones)
        {
            int[] array = new int[3] { 0,0,0};
            int count = 0;
            for (int i = 0; i < zones.Count(); i++)
            {
                if (count == 3)
                    break;

                var zone = zones.ElementAt(i);
                var card = CardsDAO.GetZoneData(zone.SiteID, zone.No).FirstOrDefault();
                if (card == null)
                    continue;

                if (card.CardsType.ToLower() == "header")
                {
                    array[0] = i;
                    count++;
                }
                else if(card.CardsType.ToLower() == "breadcrumbs")
                {
                    array[1] = i;
                    count++;
                }
                else if (card.CardsType.ToLower() == "footer")
                {
                    array[2] = i;
                    count++;
                }
            }
            return array;
        }
        /// <summary>
        /// 透過SiteID 與 PageNO取得上層的MenuID 與 DataType
        /// </summary>
        /// <param name="SiteID"></param>
        /// <param name="PageNo"></param>
        void GetMenuIDAndDataType(long SiteID, long PageNo)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"select MenuID,Menus.DataType from Pages inner join Menus on Menus.ID=Pages.MenuID where Pages.SiteID={SiteID} and Pages.[No]={PageNo}";
                var query = conn.ExecuteReader(sql);
                DataTable dt = new DataTable();
                dt.Load(query);
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    _MenuID = (long)dr["MenuID"];
                    _DataType = dr["DataType"] != null ? dr["DataType"].ToString().Trim() : string.Empty;
                }
            }
        }
        /// <summary>
        /// 產生Zone Model
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        ZonesModels GenZoneModel(AdvertisementRenderIndex item,double TempSort=0)
        {
            return new ZonesModels()
            {
                No = WorkLib.GetItem.NewSN(),
                SiteID = _SiteID,
                PageNo = _PageNo,
                StyleID = _StyleID,
                AreaSetID = item.AreaSetID,
                GroupPosition = item.GroupPosition,
                TempSort = TempSort,
                MenuID = item.MenuID,
                Sort = byte.MaxValue,
                ToCardStyleID = _CardStyleID
            };
        }
        /// <summary>
        /// 如有TempSort有重號,調整資料集排序
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="enterIndex"></param>
        /// <param name="Position"></param>
        void AdjustmentSort(List<ZonesModels> datas,double enterIndex,int Position=0)
        {
            if (datas.Where(m => m.TempSort == enterIndex).Any())
            {
                datas = datas.OrderBy(m => m.TempSort).ThenBy(m => m.Sort).ToList();

                //if (Position < 5)
                //{
                //    datas = datas.OrderBy(m => m.TempSort).ThenByDescending(m => m.Sort).ToList();
                //}else
                //{
                //    datas = datas.OrderBy(m => m.TempSort).ThenBy(m => m.Sort).ToList();
                //}

                for (int i = 0; i < datas.Count; i++)
                {
                    datas[i].TempSort = i + 1;
                }
            }
        }
    }
    /// <summary>
    /// 廣告相關 DA
    /// </summary>
    public class AdvertisementRenderTools
    {
        static string _AccountSetWhereCondition = "and IssueStart<=GETDATE() and IssueEnd>=GETDATE() and  AdsCustomize.IsIssue=1 and AdsCustomizeAccountSet.IsIssue=1";

        /// <summary>
        /// 透過Zone No和CardType查詢底下的Card No
        /// </summary>
        /// <param name="ZoneNo"></param>
        /// <returns></returns>
        public static long QueryCardNoByZoneNoAndTypeStr(long ZoneNo,string CardTypeStr)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                var sql = $"select No from Cards where ZoneNo={ZoneNo} and CardsType='{CardTypeStr.Replace("'","''")}'";
                var result = conn.Query<long>(sql);
                if (result.Any())
                    return result.ElementAt(0);
                else
                    return 0;
            }
        }
        /// <summary>
        /// 確認所在Page的位置是否在內頁
        /// </summary>
        /// <returns></returns>
        public static bool CheckPostitionIsChild(long PageNo)
        {
            if (PageNo == 0)
                return false;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"select SN from Pages where No={PageNo}";
                var query = conn.Query<string>(sql);
                if (query.Count() > 0)
                {
                    string sn = query.SingleOrDefault();
                    if (!string.IsNullOrWhiteSpace(sn))
                    {
                        var array = sn.Split('_');
                        if (array.Length == 2)
                            return array[1] == PageNo.ToString();
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// 透過PageNo查詢Pages.SN
        /// </summary>
        /// <param name="PageNo"></param>
        /// <returns></returns>
        public static string QueryPageSNByPageNo(long PageNo)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"select Pages.SN from Menus inner join Pages on Pages.MenuID = Menus.ID " +
                             $"where Pages.No = {PageNo}";
                var result = conn.Query<string>(sql);
                if (result.Any())
                    return result.FirstOrDefault();
                else
                    return string.Empty;
            }
        }
        /// <summary>
        /// 透過PageNo查詢所屬DataType
        /// </summary>
        /// <param name="PageNo"></param>
        /// <returns></returns>
        public static string QueryDataTypeByPageNo(long PageNo)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"select Menus.DataType from Menus inner join Pages on Pages.MenuID = Menus.ID " +
                             $"where Pages.No = {PageNo}";
                var result = conn.Query<string>(sql);
                if (result.Any())
                    return result.FirstOrDefault();
                else
                    return string.Empty;
            }
        }
        /// <summary>
        /// 透過PageNo取得ChildType
        /// </summary>
        /// <param name="PageNo"></param>
        /// <returns></returns>
        public static string GenChildTypeByPageNo(long PageNo)
        {
            string childtype = string.Empty;
            string dataTyp = QueryDataTypeByPageNo(PageNo);
            switch(dataTyp)
            {
                case WorkV3.Areas.Backend.Models.AreaSetDataType.Article:
                    if(CheckPostitionIsChild(PageNo))
                        childtype = WorkV3.Areas.Backend.Models.ChildType.Inside;
                    break;
                case WorkV3.Areas.Backend.Models.AreaSetDataType.Event:
                    if (CheckPostitionIsChild(PageNo))
                        childtype = WorkV3.Areas.Backend.Models.ChildType.Inside;
                    break;
                case WorkV3.Areas.Backend.Models.AreaSetDataType.Member:
                    var sn = QueryPageSNByPageNo(PageNo);
                    if (!string.IsNullOrWhiteSpace(sn) && CheckChildTypeIsExist(sn))
                        childtype = sn;
                    break;
                default:
                    break;
            }
            return childtype;
        }
        /// <summary>
        /// 檢查Menus.SN的值在常數(ChildType)中是否存在
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static bool CheckChildTypeIsExist(string sn)
        {
            sn = sn ?? string.Empty;
            Type tye = typeof(WorkV3.Areas.Backend.Models.ChildType);
            FieldInfo[] fields = tye.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            foreach (FieldInfo field in fields)
            {
                if(field.GetValue(null).ToString() == sn)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 查詢出 具有有效刊登時間之內+自訂廣告區設定刊登+刊登費用設定刊登 的 廣告區ID以及顯示位置資訊
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AdvertisementRenderIndex> GetAdvertisementRender(long? MenuID=null)
        {
            List<AdvertisementRenderIndex> list = new List<AdvertisementRenderIndex>();
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = @"
                            select
                            Advertisement.*,
                            AdsDisplayAreaSet.ID AS AreaSetID,
                            AdsDisplayAreaSet.ParentAdsDisplayAreaSetID AS ParentAreaSetID,
                            AdsDisplayAreaSet.ChildType AS AreaSetChildType,
                            AdsDisplayAreaSet.GroupPosition,
                            AdsDisplayAreaSet.MenuID

                            from Advertisement
                            inner join 
                            (
	                            select AdsCustomize.Advertisement_ID from AdsCustomize
	                            inner join AdsCustomizeAccountSet on AdsCustomizeAccountSet.AdsCustomize_ID=AdsCustomize.ID
	                            where 1=1 " + _AccountSetWhereCondition + @"
	                            group by AdsCustomize.Advertisement_ID
                            ) as AdsCustomize
                            on AdsCustomize.Advertisement_ID=Advertisement.ID
                            inner join AdsDisplayAreaSet on AdsDisplayAreaSet.Advertisement_ID=Advertisement.ID AND AdsDisplayAreaSet.GroupPosition != 'None'
                            ".Replace("\r\n", "").Replace("\t", "");
            DataTable datas = db.GetDataTable(sql);

            if (datas == null)
                return list;

            foreach (DataRow dr in datas.Rows)
            {
                list.Add(new AdvertisementRenderIndex
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
                    Remark = dr["Remark"] != null ? dr["Remark"].ToString().Trim() : string.Empty,
                    ComputerThirdPartyEmbedCode = dr["ComputerThirdPartyEmbedCode"] != null ? dr["ComputerThirdPartyEmbedCode"].ToString().Trim() : string.Empty,
                    PhoneThirdPartyEmbedCode = dr["PhoneThirdPartyEmbedCode"] != null ? dr["PhoneThirdPartyEmbedCode"].ToString().Trim() : string.Empty,

                    AreaSetID = (long)dr["AreaSetID"],
                    ParentAreaSetID = dr["ParentAreaSetID"] as long?,
                    AreaSetChildType = dr["AreaSetChildType"] != null ? dr["AreaSetChildType"].ToString().Trim() : string.Empty,
                    GroupPosition = dr["GroupPosition"].ToString().Trim(),
                    MenuID = (long)dr["MenuID"]
                });
            }

            if (MenuID != null)
            {
                //用使用者目前所在頁面MenuID篩選出目前頁面上被設定的廣告
                return list.Where(m => m.MenuID == MenuID);
            }

            return list;
        }
        /// <summary>
        /// 取得紀錄位置的值 (Attribute當中)
        /// </summary>
        /// <param name="ItemStr"></param>
        /// <param name="DataType"></param>
        /// <returns></returns>
        public static string GetPositionIndex(string ItemStr, string DataType)
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
                        return da.Description;
                }
                catch
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 透過顯示位置ID取得自訂廣告
        /// </summary>
        /// <param name="AreaSetID"></param>
        /// <returns></returns>
        public static IEnumerable<AdsCustomizeInfo> GetAdsCustomizeInfo(long AreaSetID)
        {
            List<AdsCustomizeInfo> list = new List<AdsCustomizeInfo>();
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = @"
                            select AdsCustomize.* 
                            from AdsDisplayAreaSet 
                            inner join Advertisement on Advertisement.ID=AdsDisplayAreaSet.Advertisement_ID
                            inner join AdsCustomize on AdsCustomize.Advertisement_ID=Advertisement.ID
                            inner join (
                                        select
                                        AdsCustomize.ID
                                        from AdsCustomize
                                        inner join AdsCustomizeAccountSet on AdsCustomizeAccountSet.AdsCustomize_ID=AdsCustomize.ID
                                        where 1=1 " + _AccountSetWhereCondition + @"
			                            group by AdsCustomize.ID
                            ) as AdsCustomizeFilter on AdsCustomizeFilter.ID=AdsCustomize.ID
                            where AdsDisplayAreaSet.ID = " + AreaSetID;
            DataTable datas = db.GetDataTable(sql);

            if (datas == null)
                return list;

            foreach (DataRow dr in datas.Rows)
            {
                list.Add(new AdsCustomizeInfo
                {
                    ID = (long)dr["ID"],
                    Advertisement_ID = (long)dr["Advertisement_ID"],
                    Description = dr["Description"] != null ? dr["Description"].ToString().Trim() : string.Empty,
                    ClickEvent = dr["ClickEvent"].ToString().Trim(),
                    PhonePicFollowPC = (bool)dr["PhonePicFollowPC"],
                    IsIssue = (bool)dr["IsIssue"],
                    PCPicture = dr["PCPicture"] != null ? dr["PCPicture"].ToString().Trim() : string.Empty,
                    MobilePicture = dr["MobilePicture"] != null ? dr["MobilePicture"].ToString().Trim() : string.Empty,
                    Sort = (int)dr["Sort"]
                });
            }

            return list;
        }
        /// <summary>
        /// 取得廣告相關資訊
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AdsCustomizeInfo GetADdata(CardsModels model)
        {
            //一個位置可為多個自訂廣告內容
            var AdsCustomizeInfo = GetAdsCustomizeInfo(model.AreaSetID);

            //隨機選擇自訂廣告
            Random r = new Random();
            int index = 0;
            if (AdsCustomizeInfo.Count()>0)
            {
                index = r.Next(0, AdsCustomizeInfo.Count());
            }
            AdsCustomizeInfo ADdata = AdsCustomizeInfo.ToList()[index];

            ADdata.AdvertisementDatas = GetItem<AdvertisementRenderIndex>("Advertisement", ADdata.Advertisement_ID);
            ADdata.AdsCustomizeToLinkDatas = GetItem<AdsCustomizeToLinkInfo>("AdsCustomizeToLink", ADdata.ID, "AdsCustomize_ID");
            ADdata.AdsCustomizeToVideoDatas = GetItem<AdsCustomizeToVideoModelInfo>("AdsCustomizeToVideo", ADdata.ID, "AdsCustomize_ID");

            string childtype = GenChildTypeByPageNo(model.PageNo);
            string Condition = string.Empty;
            //如果Pages.SN是Desktop就當作主檔處理
            // 20180703 neil 因為廣告區選擇不使用主從架構，所以把 parentAdsDisplayAreaSetID 部份移除
            if (!string.IsNullOrWhiteSpace(childtype) && childtype != WorkV3.Areas.Backend.Models.ChildType.Desktop)
                Condition = $" and (ChildType='{childtype.Replace("'","''")}')";
            else if(string.IsNullOrWhiteSpace(childtype))
                Condition = $" and ChildType is null ";


            ADdata.AdsDisplayAreaSetInfo = GetItem<AdsDisplayAreaSetInfo>("AdsDisplayAreaSet", ADdata.Advertisement_ID, "Advertisement_ID","and MenuID="+ model.MenuID + Condition);

            return ADdata;
        }
        /// <summary>
        /// 產生Card Model
        /// </summary>
        /// <param name="Zone"></param>
        /// <returns></returns>
        public static List<CardsModels> GenCard(ZonesModels Zone)
        {
            List<CardsModels> Cards = new List<CardsModels>();
            Cards.Add(new CardsModels()
            {
                No = WorkLib.GetItem.NewSN(),
                ZoneNo = Zone.No,
                CardsType = "AdvertisementRender",
                Status = 1,
                SiteID = Zone.SiteID,
                PageNo = Zone.PageNo,
                AreaSetID = (long)Zone.AreaSetID,
                StylesID = Zone.ToCardStyleID,
                AdvertisementMenuID = GetAdvertisementMenuID(Zone.SiteID),
                GroupPosition = Zone.GroupPosition,
                MenuID = Zone.MenuID,
                TempSort = Zone.TempSort
            });
            return Cards;
        }
        /// <summary>
        /// 查詢廣告的menu id
        /// </summary>
        /// <param name="SiteID"></param>
        /// <returns></returns>
        public static long GetAdvertisementMenuID(long SiteID)
        {
            long result = 0;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"select * from Menus where SN='Advertisement' and SiteID={SiteID}";
                var query = conn.ExecuteReader(sql);
                DataTable dt = new DataTable();
                dt.Load(query);
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    return (long)dr["ID"];
                }
            }

            return result;
        }
        /// <summary>
        /// 查詢資料
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <param name="WhereColumn"></param>
        /// <returns></returns>
        public static T GetItem<T>(string tableName, long id, string WhereColumn ="ID",string WhereCondition="")
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = string.Format("Select * From " + tableName + " Where " + WhereColumn + " = " + id + " {0}", WhereCondition);
                return conn.Query<T>(sql).FirstOrDefault();
            }
        }
        /// <summary>
        /// 使用者動作儲存
        /// </summary>
        /// <param name="AdsCustomizeID"></param>
        /// <param name="PageID"></param>
        /// <param name="Event"></param>
        /// <param name="MemberID"></param>
        public static void SetUserEventLog(long AdsCustomizeID,long PageID,string Event,long? MemberID)
        {
            if (AdsCustomizeID == 0 || PageID == 0 || string.IsNullOrWhiteSpace(Event))
                return;

            if (Event != UserEvent.Browsing && Event != UserEvent.Click)
                return;

            AdsStatisticsModel item = new AdsStatisticsModel()
            {
                AdsCustomizeID = AdsCustomizeID,
                PageID = PageID,
                SessionID = System.Web.HttpContext.Current.Session.SessionID,
                DeviceID = WorkV3.Areas.Backend.Models.DataAccess.AdvertisementDAO.GetCpuID(),
                Browser = System.Web.HttpContext.Current.Request.Browser.Browser,
                Event = Event,
                IP = WorkLib.GetItem.IPAddr(),
                IPNum = (long)WorkLib.GetItem.GetIPNum(),
                MemberID = MemberID ?? 0,
                RecordDay = DateTime.Now,
                RecordTime = DateTime.Now
            };

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("AdsStatistics");
            tableObj.GetDataFromObject(item);

            string check = "Select 1 From AdsStatistics " +
                $"Where AdsCustomizeID={item.AdsCustomizeID} and " +
                $"PageID={item.PageID} and " +
                $"SessionID='{item.SessionID}' and " +
                $"Event='{item.Event}' and " +
                $"RecordDay='{item.RecordDay.ToString("yyyy/MM/dd")}'";

            bool isNew = db.GetFirstValue(check) == null;
            if (isNew)
                tableObj.Insert();

        }
    }

    #region 廣告相關 Model
    /// <summary>
    /// 廣告區 Model
    /// </summary>
    public class AdvertisementRenderIndex
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        /// <summary>
        /// 廣告主 ID
        /// </summary>
        public long? Advertisers_ID { get; set; }
        /// <summary>
        /// 廣告區名稱
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 使用於電腦版
        /// </summary>
        public bool IsUseForComputer { get; set; }
        /// <summary>
        /// 電腦版尺寸_長
        /// </summary>
        public int? ComputerHeight { get; set; }
        /// <summary>
        /// 電腦版尺寸_寬
        /// </summary>
        public int? ComputerWidth { get; set; }
        /// <summary>
        /// 使用於手機板
        /// </summary>
        public bool IsUseForPhone { get; set; }
        /// <summary>
        /// 手機板尺寸_長
        /// </summary>
        public int? PhoneHeight { get; set; }
        /// <summary>
        /// 手機板尺寸_寬
        /// </summary>
        public int? PhoneWidth { get; set; }
        /// <summary>
        /// 類型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 電腦版第三方嵌入碼
        /// </summary>
        public string ComputerThirdPartyEmbedCode { get; set; }
        /// <summary>
        /// 手機版第三方嵌入碼
        /// </summary>
        public string PhoneThirdPartyEmbedCode { get; set; }
        /// <summary>
        /// 第三方廣告連結
        /// </summary>
        public string ThirdPartyLink { get; set; }

        public long AreaSetID { get; set; } //AdsDisplayAreaSet.ID
        public long? ParentAreaSetID { get; set; } //AdsDisplayAreaSet.ParentAdsDisplayAreaSetID
        public string AreaSetChildType { get; set; } //AdsDisplayAreaSet.ChildType
        public string GroupPosition { get; set; } //AdsDisplayAreaSet.GroupPosition
        public long MenuID { get; set; } //AdsDisplayAreaSet.MenuID
    }
    /// <summary>
    /// 自訂廣告 Model
    /// </summary>
    public class AdsCustomizeInfo
    {
        public long ID { get; set; }
        /// <summary>
        /// 廣告(區)主檔 ID
        /// </summary>
        public long Advertisement_ID { get; set; }
        /// <summary>
        /// 廣告主 ID
        /// </summary>
        public long? Advertisers_ID { get; set; }
        /// <summary>
        /// 廣告說明
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 點擊事件
        /// </summary>
        public string ClickEvent { get; set; }
        /// <summary>
        /// 是否手機版圖片引用電腦版
        /// </summary>
        public bool PhonePicFollowPC { get; set; }
        /// <summary>
        /// 是否刊登
        /// </summary>
        public bool IsIssue { get; set; }
        /// <summary>
        /// 電腦板圖片
        /// </summary>
        public string PCPicture { get; set; }
        /// <summary>
        /// 手機板圖片
        /// </summary>
        public string MobilePicture { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }

        public AdvertisementRenderIndex AdvertisementDatas { get; set; }
        public AdsCustomizeToLinkInfo AdsCustomizeToLinkDatas { get; set; }
        public AdsCustomizeToVideoModelInfo AdsCustomizeToVideoDatas { get; set; }
        public AdsDisplayAreaSetInfo AdsDisplayAreaSetInfo { get; set; }
    }
    /// <summary>
    /// 自訂廣告-點擊事件-連結 Model
    /// </summary>
    public class AdsCustomizeToLinkInfo
    {
        public long ID { get; set; }
        /// <summary>
        /// 自訂廣告管理 ID
        /// </summary>
        public long AdsCustomize_ID { get; set; }
        /// <summary>
        /// 網址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 是否另開
        /// </summary>
        public bool IsOpenNew { get; set; }
        public int Sort { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
    /// <summary>
    /// 自訂廣告-點擊事件-影片 Model
    /// </summary>
    public class AdsCustomizeToVideoModelInfo
    {
        public long ID { get; set; }
        /// <summary>
        /// 自訂廣告管理 ID
        /// </summary>
        public long AdsCustomize_ID { get; set; }
        public string Type { get; set; }
        public string Link { get; set; }
        public string PlayMode { get; set; }
        public bool IsAuto { get; set; }
        public bool IsRepeat { get; set; }
        public bool ScreenshotIsCustom { get; set; }
        public long? Screenshot { get; set; }
        /// <summary>
        /// 是否為蓋台廣告
        /// </summary>
        public bool IsCover { get; set; }
        /// <summary>
        /// 蓋台廣告秒數
        /// </summary>
        public int? CoverSecond { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public long SiteID { get; set; }
        public long MenuID { get; set; }
    }
    /// <summary>
    /// 顯示位置 Model
    /// </summary>
    public class AdsDisplayAreaSetInfo
    {
        public long ID { get; set; }
        /// <summary>
        /// 廣告區 ID
        /// </summary>
        public long Advertisement_ID { get; set; }
        /// <summary>
        /// 顯示位置
        /// </summary>
        public string GroupPosition { get; set; }
        public long SiteID { get; set; }
        public long MenuID { get; set; }
        /// <summary>
        /// 蓋台設定種類
        /// </summary>
        public string OverlayType { get; set; }
        /// <summary>
        /// 蓋台設定_蓋台機率
        /// </summary>
        public int? OverlayChance { get; set; }
        /// <summary>
        /// 蓋台設定_重複播放秒數
        /// </summary>
        public int? OverlayRepeatSeconds { get; set; }
        /// <summary>
        /// 蓋台設定_閒置秒數
        /// </summary>
        public int? OverlayIdleSeconds { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
    #endregion
}