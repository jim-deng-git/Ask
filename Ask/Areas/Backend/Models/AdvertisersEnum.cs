using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 刊登種類
    /// </summary>
    public class IssueSetType
    {
        /// <summary>
        /// 刊登期間計價
        /// </summary>
        [Display(Name = "刊登期間<br />計費")]
        public const string IssueTime = "IssueTime";
        /// <summary>
        /// 點擊瀏覽計價
        /// </summary>
        [Display(Name = "點擊瀏覽<br />計費")]
        public const string Click = "Click";
        /// <summary>
        /// 免費
        /// </summary>
        [Display(Name = "免費")]
        public const string Free = "Free";
    }
    /// <summary>
    /// 幣別
    /// </summary>
    public class CurrencyItem
    {
        /// <summary>
        /// 台幣
        /// </summary>
        [Display(Name = "NT$")]
        public const string NTD = "NTD";
        /// <summary>
        /// 美元
        /// </summary>
        [Display(Name = "US$")]
        public const string USD = "USD";
    }
    /// <summary>
    /// For搜尋: 刊登費用類型
    /// </summary>
    public class CostSearchType
    {
        /// <summary>
        /// 總額篩選
        /// </summary>
        public const string Total = "Total";
        /// <summary>
        /// 每月金額篩選
        /// </summary>
        public const string Month = "Month";
        /// <summary>
        /// 每日金額篩選
        /// </summary>
        public const string Day = "Day";

    }
    /// <summary>
    /// 點擊事件開啟類型
    /// </summary>
    public class ClickEvent
    {
        /// <summary>
        /// 連結
        /// </summary>
        public const string Link = "Link";
        /// <summary>
        /// 影片
        /// </summary>
        public const string Video = "Video";
        /// <summary>
        /// 無開啟
        /// </summary>
        public const string None = "None";

    }
    /// <summary>
    /// 影片類型
    /// </summary>
    public class VideoType
    {
        public const string Youtube = "Youtube";
        public const string Vimeo = "Vimeo";
        public const string Custom = "Custom";
    }
    /// <summary>
    /// 蓋台設定種類
    /// </summary>
    public class OverlayType
    {
        /// <summary>
        /// 廣告出現機率(進入網頁時)
        /// </summary>
        public const string Chance = "Chance";
        /// <summary>
        /// 只有第一次出現，除非清除瀏覽紀錄才會再出現
        /// </summary>
        public const string Once = "Once";
        /// <summary>
        /// 第一次出現，過了N秒後再出現
        /// </summary>
        public const string AfrerSec = "AfrerSec";
        /// <summary>
        /// 使用者閒置在頁面上N秒後出現
        /// </summary>
        public const string AfterIdle = "AfterIdle";
    }
    /// <summary>
    /// 廣告區類型
    /// </summary>
    public class AdvertisementType
    {
        /// <summary>
        /// 第三方廣告
        /// </summary>
        public const string third_party = "thirdparty";
        /// <summary>
        /// 自訂廣告
        /// </summary>
        public const string custom = "custom";
    }
    /// <summary>
    /// 用於位置設定的DataType
    /// </summary>
    public class AreaSetDataType
    {
        /// <summary>
        /// 文章
        /// </summary>
        public const string Article = "Article";
        /// <summary>
        /// 單頁
        /// </summary>
        public const string ArticleIntro = "ArticleIntro";
        /// <summary>
        /// 首頁
        /// </summary>
        public const string Intro = "Intro";
        /// <summary>
        /// 活動
        /// </summary>
        public const string Event = "Event";
        /// <summary>
        /// 問卷
        /// </summary>
        public const string Questionnaire = "Questionnaire";
        /// <summary>
        /// 會員
        /// </summary>
        public const string Member = "Member";

    }
    /// <summary>
    /// 顯示位置子資料類型
    /// </summary>
    public class ChildType
    {
        /// <summary>
        /// 內頁位置
        /// </summary>
        public const string Inside = "Inside";
        /// <summary>
        /// 會員 - 會員登入
        /// </summary>
        public const string Login = "Login"; //from Pages.SN's value
        /// <summary>
        /// 會員 - 會員主頁
        /// </summary>
        public const string Desktop = "Desktop"; //from Pages.SN's value
    }
    /// <summary>
    /// 使用者操作事件
    /// </summary>
    public class UserEvent
    {
        /// <summary>
        /// 點擊
        /// </summary>
        public const string Click = "Click";
        /// <summary>
        /// 瀏覽
        /// </summary>
        public const string Browsing = "Browsing";
    }
    /// <summary>
    /// Menu.DataType的資料
    /// </summary>
    public class DataType
    {
        public const string Intro = "Intro";
        public const string Event = "Event";
        public const string Article = "Article";
        public const string ArticleIntro = "ArticleIntro";
    }
    //顯示位置代碼說明：---------------------------------------------------------
    //第一碼 => C中間 R右  A共用類
    //第二碼 => 1~10(Header~Footer) 之間 的插入索引數值

    /// <summary>
    /// 顯示位置類型(單頁ArticleIntro)
    /// </summary>
    public class DisplayAreaTypeArticleIntro
    {
        /// <summary>
        /// 無
        /// </summary>
        [Display(Name = "無", Description = "N")]
        public const string None = "None";
        /// <summary>
        /// 上橫幅
        /// </summary>
        [Display(Name = "上橫幅", Description = "C4")]
        public const string Top = "Top";
        /// <summary>
        /// 下橫幅
        /// </summary>
        [Display(Name = "下橫幅", Description = "C9")]
        public const string Bottom = "Bottom";
        /// <summary>
        /// 右一橫幅
        /// </summary>
        [Display(Name = "右一橫幅", Description = "R1")]
        public const string Right_1 = "Right_1";
        /// <summary>
        /// 右二橫幅
        /// </summary>
        [Display(Name = "右二橫幅", Description = "R3")]
        public const string Right_2 = "Right_2";
        /// <summary>
        /// 右三橫幅
        /// </summary>
        [Display(Name = "右三橫幅", Description = "R5")]
        public const string Right_3 = "Right_3";
        /// <summary>
        /// 右四橫幅
        /// </summary>
        [Display(Name = "右四橫幅", Description = "R7")]
        public const string Right_4 = "Right_4";
        /// <summary>
        /// 蓋台廣告
        /// </summary>
        [Display(Name = "蓋台廣告", Description = "A0")]
        public const string Overlay = DisplayAreaShardType.Overlay;
    }
    /// <summary>
    /// 顯示位置類型(單頁Single)
    /// </summary>
    public class DisplayAreaTypeSingle
    {
        /// <summary>
        /// 無
        /// </summary>
        [Display(Name = "無", Description = "N")]
        public const string None = "None";
        /// <summary>
        /// 上橫幅
        /// </summary>
        [Display(Name = "上橫幅", Description = "C4")]
        public const string Top = "Top";
        /// <summary>
        /// 下橫幅
        /// </summary>
        [Display(Name = "下橫幅", Description = "C8")]
        public const string Bottom = "Bottom";
        /// <summary>
        /// 右一橫幅
        /// </summary>
        [Display(Name = "右一橫幅", Description = "R1")]
        public const string Right_1 = "Right_1";
        /// <summary>
        /// 右二橫幅
        /// </summary>
        [Display(Name = "右二橫幅", Description = "R3")]
        public const string Right_2 = "Right_2";
        /// <summary>
        /// 右三橫幅
        /// </summary>
        [Display(Name = "右三橫幅", Description = "R5")]
        public const string Right_3 = "Right_3";
        /// <summary>
        /// 右四橫幅
        /// </summary>
        [Display(Name = "右四橫幅", Description = "R7")]
        public const string Right_4 = "Right_4";
        /// <summary>
        /// 蓋台廣告
        /// </summary>
        [Display(Name = "蓋台廣告", Description = "A0")]
        public const string Overlay = DisplayAreaShardType.Overlay;
    }
    /// <summary>
    /// 顯示位置類型(首頁)
    /// </summary>
    public class DisplayAreaTypeIntro
    {
        /// <summary>
        /// 無
        /// </summary>
        [Display(Name = "無", Description = "N")]
        public const string None = "None";
        /// <summary>
        /// 上一橫幅
        /// </summary>
        [Display(Name = "上一橫幅", Description = "C3")]
        public const string Top_1 = "Top_1";
        /// <summary>
        /// 上二橫幅
        /// </summary>
        [Display(Name = "上二橫幅", Description = "C4")]
        public const string Top_2 = "Top_2";
        /// <summary>
        /// 中一橫幅
        /// </summary>
        [Display(Name = "中一橫幅", Description = "C5")]
        public const string Middle_1 = "Middle_1";
        /// <summary>
        /// 中二橫幅
        /// </summary>
        [Display(Name = "中二橫幅", Description = "C6")]
        public const string Middle_2 = "Middle_2";
        /// <summary>
        /// 下一橫幅
        /// </summary>
        [Display(Name = "下一橫幅", Description = "C7")]
        public const string Bottom_1 = "Bottom_1";
        /// <summary>
        /// 下二橫幅
        /// </summary>
        [Display(Name = "下二橫幅", Description = "C8")]
        public const string Bottom_2 = "Bottom_2";
        /// <summary>
        /// 蓋台廣告
        /// </summary>
        [Display(Name = "蓋台廣告", Description = "A0")]
        public const string Overlay = DisplayAreaShardType.Overlay;
    }
    /// <summary>
    /// 顯示位置類型(文章)
    /// </summary>
    public class DisplayAreaTypeArticle
    {
        /// <summary>
        /// 無
        /// </summary>
        [Display(Name = "無", Description = "N")]
        public const string None = "None";
        /// <summary>
        /// 上橫幅
        /// </summary>
        [Display(Name = "上橫幅", Description = "C4")]
        public const string Top = "Top";
        /// <summary>
        /// 下橫幅
        /// </summary>
        [Display(Name = "下橫幅", Description = "C9")]
        public const string Bottom = "Bottom";
        /// <summary>
        /// 右一橫幅
        /// </summary>
        [Display(Name = "右一橫幅", Description = "R1")]
        public const string Right_1 = "Right_1";
        /// <summary>
        /// 右二橫幅
        /// </summary>
        [Display(Name = "右二橫幅", Description = "R3")]
        public const string Right_2 = "Right_2";
        /// <summary>
        /// 右三橫幅
        /// </summary>
        [Display(Name = "右三橫幅", Description = "R5")]
        public const string Right_3 = "Right_3";
        /// <summary>
        /// 右四橫幅
        /// </summary>
        [Display(Name = "右四橫幅", Description = "R7")]
        public const string Right_4 = "Right_4";
        /// <summary>
        /// 蓋台廣告
        /// </summary>
        [Display(Name = "蓋台廣告", Description = "A0")]
        public const string Overlay = DisplayAreaShardType.Overlay;
    }
    /// <summary>
    /// 顯示位置類型(活動)
    /// </summary>
    public class DisplayAreaTypeEvent
    {
        /// <summary>
        /// 無
        /// </summary>
        [Display(Name = "無", Description = "N")]
        public const string None = "None";
        /// <summary>
        /// 上橫幅
        /// </summary>
        [Display(Name = "上橫幅", Description = "C4")]
        public const string Top = "Top";
        /// <summary>
        /// 下橫幅
        /// </summary>
        [Display(Name = "下橫幅", Description = "C9")]
        public const string Bottom = "Bottom";
        /// <summary>
        /// 右一橫幅
        /// </summary>
        [Display(Name = "右一橫幅", Description = "R1")]
        public const string Right_1 = "Right_1";
        /// <summary>
        /// 右二橫幅
        /// </summary>
        [Display(Name = "右二橫幅", Description = "R3")]
        public const string Right_2 = "Right_2";
        /// <summary>
        /// 右三橫幅
        /// </summary>
        [Display(Name = "右三橫幅", Description = "R5")]
        public const string Right_3 = "Right_3";
        /// <summary>
        /// 右四橫幅
        /// </summary>
        [Display(Name = "右四橫幅", Description = "R7")]
        public const string Right_4 = "Right_4";
        /// <summary>
        /// 蓋台廣告
        /// </summary>
        [Display(Name = "蓋台廣告", Description = "A0")]
        public const string Overlay = DisplayAreaShardType.Overlay;
    }
    /// <summary>
    /// 顯示位置類型(問卷)
    /// </summary>
    public class DisplayAreaTypeQuestionnaire
    {
        /// <summary>
        /// 無
        /// </summary>
        [Display(Name = "無", Description = "N")]
        public const string None = "None";
        /// <summary>
        /// 上橫幅
        /// </summary>
        [Display(Name = "上橫幅", Description = "C4")]
        public const string Top = "Top";
        /// <summary>
        /// 下橫幅
        /// </summary>
        [Display(Name = "下橫幅", Description = "C8")]
        public const string Bottom = "Bottom";
        /// <summary>
        /// 蓋台廣告
        /// </summary>
        [Display(Name = "蓋台廣告", Description = "A0")]
        public const string Overlay = DisplayAreaShardType.Overlay;
    }
    /// <summary>
    /// 顯示位置類型(會員)
    /// </summary>
    public class DisplayAreaTypeMember
    {
        /// <summary>
        /// 無
        /// </summary>
        [Display(Name = "無", Description = "N")]
        public const string None = "None";
        /// <summary>
        /// 上橫幅
        /// </summary>
        [Display(Name = "上橫幅", Description = "C4")]
        public const string Top = "Top";
        /// <summary>
        /// 中一橫幅
        /// </summary>
        [Display(Name = "中一橫幅", Description = "C5")]
        public const string Middle_1 = "Middle_1";
        /// <summary>
        /// 中二橫幅
        /// </summary>
        [Display(Name = "中二橫幅", Description = "C6")]
        public const string Middle_2 = "Middle_2";
        /// <summary>
        /// 下橫幅
        /// </summary>
        [Display(Name = "下橫幅", Description = "C8")]
        public const string Bottom = "Bottom";
        /// <summary>
        /// 蓋台廣告
        /// </summary>
        [Display(Name = "蓋台廣告", Description = "A0")]
        public const string Overlay = DisplayAreaShardType.Overlay;
    }
    /// <summary>
    /// 顯示位置共用類型
    /// </summary>
    public class DisplayAreaShardType
    {
        public const string Overlay = "Overlay";
        public const string Top = "Top";
        public const string Bottom = "Bottom";
    }
}