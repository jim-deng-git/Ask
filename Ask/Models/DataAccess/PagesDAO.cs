using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Dapper;
using WorkV3.Common;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;
namespace WorkV3.Models.DataAccess
{
    public class PagesDAO
    {
        public static PagesModels GetPageInfo(long SiteID,string PageSN)
        {
            PagesModels nData = new PagesModels();
            string Sql = "Select * from Pages where SN=@SN and SiteID=@SiteID ";
            using (var SqlCn = new SqlConnection(WebInfo.Conn))
            {
                var res = SqlCn.Query<PagesModels>(
                    Sql,
                    new { SN = PageSN , SiteID = SiteID });
                nData = res.FirstOrDefault();
            }
            return nData;
        }
        public static SEOModels GetPageSEO(long SiteID, long PageNo, string pageTitle, long? Id = null)
        {
            CardsModels card = new CardsModels();
            SEOModels seo = new SEOModels();
            string Sql = @"SELECT  * FROM Cards WHERE ZoneNo IN
                            (
                                SELECT[No] FROM  Zones WHERE PageNo = @PageNo
                            ) AND CardsType Not IN('Header', 'BreadCrumbs', 'Footer') ";
            using (var SqlCn = new SqlConnection(WebInfo.Conn))
            {
                #region 取網頁SEO
                var res = SqlCn.Query<CardsModels>(
                    Sql,
                    new { PageNo = PageNo });
                card = res.FirstOrDefault();
                if (card != null)
                {
                    string tableName = card.CardsType;
                    if (card.CardsType == "Event" ||
                        card.CardsType == "PlainText" ||
                        card.CardsType == "Article" ||
                        card.CardsType == "ArticleIntro"||
                        //以下三個為 2018-06-14 加入的
                        card.CardsType == "Form"||
                       // card.CardsType == "Intro"||
                        card.CardsType == "Questionnaire"||
                        card.CardsType == "Seek"||
                         card.CardsType == "Reward" ||
                         card.CardsType == "Recruit"

                         //wei 20180911 新增Seek & Reward 
                         )
                    {
                        if (tableName == "Event")
                            tableName = "Events";
                        if (tableName == "Recruit")
                            tableName = "Recruits";

                        Sql = string.Format(@"SELECT * FROM SEO WHERE SourceNo IN (SELECT ID FROM {0} WHERE CardNo=@CardNo) ", tableName);
                        if (tableName == "Form")
                        {
                            Sql = @"SELECT * FROM SEO WHERE SourceNo IN (SELECT ID FROM FORM WHERE SourceID=@PageNo)";
                        }

                        if(tableName == "Recruits") //20190516 Nina 徵才需用ID取得SEO
                        {
                            Sql = $"SELECT * FROM SEO WHERE SourceNo = {(Id == null ? 0 : Id)}";
                        }

                        var pageseo = SqlCn.Query<SEOModels>(
                           Sql,
                           new { CardNo = card.No, PageNo=PageNo, SiteID = SiteID });
                        seo = pageseo.FirstOrDefault();
                        if (seo != null)
                            return seo;
                    }
                }
                #endregion
                #region 取單元SEO (未製作)
                #endregion
                #region 取全站SEO
                //var siteSEO = Areas.Backend.Models.DataAccess.SiteSeoSettingDAO.GetItem(SiteID);
                //if (siteSEO != null)
                //{
                //    seo = new SEOModels();
                //    seo.Title = siteSEO.Title;
                //    seo.Description = siteSEO.Description;
                //    seo.Author = siteSEO.Author;
                //    seo.Copyright = siteSEO.Copyright;
                //    seo.Keywords = siteSEO.Keywords;
                //    return seo;
                //}
                //Sql = string.Format(@"SELECT * FROM SEO WHERE SourceNo=@SiteID ", SiteID);
                //var siteseo = SqlCn.Query<SEOModels>(
                //   Sql,
                //   new { SiteID = SiteID });
                //seo = siteseo.FirstOrDefault();
                //if (seo != null)
                //{
                //    seo.Title = pageTitle;
                //    return seo;
                //}
                #endregion
            }

            return null;
        }
        public static ViewModels.SEORelationModel GetContentSEO(long SiteID, long MenuID, long PageNo)
        {
            ViewModels.SEORelationModel seoModel = new ViewModels.SEORelationModel();
            string newUploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(SiteID, MenuID).TrimEnd('/') + "/";
            //WorkLib.WriteLog.Write(true, MenuID.ToString());
            List<ZonesModels> zoneList = ZonesDAO.GetPageData(SiteID, PageNo);
            if (zoneList != null && zoneList.Count > 0)
            {
                for (int i = 0; i < zoneList.Count; i++)
                {
                    List<CardsModels> cardList = CardsDAO.GetZoneData(SiteID, zoneList[i].No);
                    if (cardList != null && cardList.Count > 0)
                    {
                        switch (cardList[0].CardsType)
                        {
                            case "Article":
                                ArticleModels articleModel = ArticleDAO.GetItemByCard(cardList[0].No);
                                
                                if (articleModel != null)
                                {
                                    var authors = ArticleDAO.GetItemPosters(articleModel.ID);
                                    if (authors != null && authors.Count() > 0)
                                    {
                                        foreach (ArticlePosterModels auhtor in authors)
                                        {
                                            seoModel.Author += auhtor.Name+";";
                                        }
                                        seoModel.Author = seoModel.Author.Trim(';');
                                    }
                                    IEnumerable<ArticleTypesModels> types = articleModel.GetTypes();
                                    seoModel.Keywords = string.Join(";", types.Select(t => t.Name));

                                    if (!string.IsNullOrEmpty(articleModel.Icon)) // 取得[內文/頁面細節/自行設定代表圖]
                                    {
                                        ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(articleModel.Icon);
                                        seoModel.SocialImage = newUploadUrl + imgModel.Img;
                                    }
                                    else
                                    {
                                        if (articleModel.CustomIcon) // 取得[內文 主影片 自行上傳圖片]
                                        {
                                            if (!string.IsNullOrEmpty(articleModel.VideoImg))
                                            {
                                                ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(articleModel.VideoImg);
                                                seoModel.SocialImage = newUploadUrl + imgModel.Img;
                                            }
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(articleModel.VideoImg))
                                                seoModel.SocialImage = articleModel.VideoImg; //[ 內文 主影片 影片截圖]
                                        }
                                    }
                                    var paragraphs = ParagraphDAO.GetItems(articleModel.ID);
                                    if (paragraphs != null && paragraphs.Count() > 0)
                                    {
                                        foreach (ParagraphModels paragraph in paragraphs)
                                        {
                                            //都沒取到圖, 則繼續取段落的圖
                                            if (string.IsNullOrEmpty(seoModel.SocialImage))
                                            {
                                                if (paragraph.MatchType == "img")
                                                {
                                                    IEnumerable<ResourceImagesModels> images = paragraph.GetImages().Where(m => m.IsShow);
                                                    if (images != null && images.Count() > 0)
                                                    {
                                                        ResourceImagesModels imgModel = images.FirstOrDefault();
                                                        seoModel.SocialImage = newUploadUrl + imgModel.Img;
                                                    }
                                                }
                                            }
                                            if (!string.IsNullOrWhiteSpace(paragraph.Contents))
                                            {
                                                if (string.IsNullOrEmpty(seoModel.Description))
                                                    seoModel.Description = paragraph.Contents.TrimTags().Truncate(100);
                                            }
                                        }
                                    }
                                }
                                break;
                            case "ArticleIntro":
                                ArticleIntroModels articleIntroModel = ArticleIntroDAO.GetItem(cardList[0].No);
                                if (articleIntroModel != null)
                                {
                                    ArticleSettingModels setting = ArticleSettingDAO.GetItem(MenuID);
                                    IEnumerable<ArticleTypesModels> types = ArticleTypesDAO.GetItems(MenuID);
                                    if (setting.Types != "all")
                                    {
                                        IEnumerable<long> allowTypeIds = setting.GetTypes();
                                        types = types.Where(t => allowTypeIds.Contains(t.ID));
                                        seoModel.Keywords = string.Join(",", types.Select(t => t.Name));
                                    }
                                    else
                                    {
                                        seoModel.Keywords = string.Join(",", types.Select(t => t.Name));
                                    }

                                    if (!string.IsNullOrEmpty(articleIntroModel.Icon)) // 取得[內文/頁面細節/自行設定代表圖]
                                    {
                                        ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(articleIntroModel.Icon);
                                        seoModel.SocialImage = newUploadUrl + imgModel.Img;
                                    }
                                    var paragraphs = ParagraphDAO.GetItems(articleIntroModel.ID);
                                    if (paragraphs != null && paragraphs.Count() > 0)
                                    {
                                        foreach (ParagraphModels paragraph in paragraphs)
                                        {
                                            //都沒取到圖, 則繼續取段落的圖
                                            if (string.IsNullOrEmpty(seoModel.SocialImage))
                                            {
                                                if (paragraph.MatchType == "img")
                                                {
                                                    IEnumerable<ResourceImagesModels> images = paragraph.GetImages().Where(m => m.IsShow);
                                                    if (images != null && images.Count() > 0)
                                                    {
                                                        ResourceImagesModels imgModel = images.FirstOrDefault();
                                                        seoModel.SocialImage = newUploadUrl + imgModel.Img;
                                                    }
                                                }
                                            }
                                            if (!string.IsNullOrWhiteSpace(paragraph.Contents))
                                            {
                                                if (string.IsNullOrEmpty(seoModel.Description))
                                                    seoModel.Description = paragraph.Contents.TrimTags().Truncate(100);
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            return seoModel;
        }
        /*
        public static string GetContentTypeKeyword(long SiteID, long MenuID, long PageNo)
        {
            string type_keywords = "";
            List<ZonesModels> zoneList = ZonesDAO.GetPageData(SiteID, PageNo);
            if (zoneList != null && zoneList.Count > 0)
            {
                for (int i = 0; i < zoneList.Count; i++)
                {
                    List<CardsModels> cardList = CardsDAO.GetZoneData(SiteID, zoneList[i].No);
                    if (cardList != null && cardList.Count > 0)
                    {
                        switch (cardList[0].CardsType)
                        {
                            case "Article":
                                ArticleModels articleModel = ArticleDAO.GetItemByCard(cardList[0].No);
                                if (articleModel != null)
                                {
                                    IEnumerable<ArticleTypesModels> types = articleModel.GetTypes();
                                    type_keywords = string.Join(";", types.Select(t => t.Name));
                                }
                                break;
                            case "ArticleIntro":
                                ArticleIntroModels articleIntroModel = ArticleIntroDAO.GetItem(cardList[0].No);
                                if (articleIntroModel != null)
                                {
                                    ArticleSettingModels setting = ArticleSettingDAO.GetItem(MenuID);
                                    IEnumerable<ArticleTypesModels> types = ArticleTypesDAO.GetItems(MenuID);
                                    if (setting.Types != "all")
                                    {
                                        IEnumerable<long> allowTypeIds = setting.GetTypes();
                                        types = types.Where(t => allowTypeIds.Contains(t.ID));
                                        type_keywords = string.Join(",", types.Select(t => t.Name));
                                    }
                                    else
                                    {
                                        type_keywords = string.Join(",", types.Select(t => t.Name));
                                    }
                                }
                                break;
                            case "Event":
                                EventModels eventModel = EventDAO.GetItemByCard(cardList[0].No);
                                if (eventModel != null)
                                {
                                    IEnumerable<EventTypesModels> types = eventModel.GetTypes();
                                    type_keywords = string.Join(";", types.Select(t => t.Name));
                                }
                                break;
                            case "Questionnaire":
                                QuestionnaireModel questionnaireModel = QuestionnaireDAO.GetItemByCardNo(cardList[0].No);
                                if (questionnaireModel != null)
                                {
                                    var selecttypes = questionnaireModel.GetTypes();
                                    var qTypes = QuestionnaireTypeDAO.GetItems();
                                    qTypes.Where(q => selecttypes.Contains(q.ID));
                                    type_keywords = string.Join(";", qTypes.Select(t => t.Name));
                                }
                                break;
                        }
                    }
                }
            }
            return type_keywords;
        }
        */
        public static string GetContentImage(long SiteID, long MenuID, long PageNo)
        {
            List<ZonesModels> zoneList = ZonesDAO.GetPageData(SiteID, PageNo);
            string newUploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(SiteID, MenuID).TrimEnd('/') + "/";

            if (zoneList != null && zoneList.Count > 0)
            {
                for (int i = 0; i < zoneList.Count; i++)
                {
                    List<CardsModels> cardList = CardsDAO.GetZoneData(SiteID, zoneList[i].No);
                    if (cardList != null && cardList.Count > 0)
                    {
                        switch (cardList[0].CardsType)
                        {
                            case "Article":
                                ArticleModels articleModel = ArticleDAO.GetItemByCard(cardList[0].No);
                                if (articleModel != null)
                                {
                                    if (!string.IsNullOrEmpty(articleModel.Icon)) // 取得[內文/頁面細節/自行設定代表圖]
                                    {
                                        ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(articleModel.Icon);
                                        return newUploadUrl + imgModel.Img;
                                    }
                                    else
                                    {
                                        if (articleModel.CustomIcon) // 取得[內文 主影片 自行上傳圖片]
                                        {
                                            if (!string.IsNullOrEmpty(articleModel.VideoImg))
                                            {
                                                ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(articleModel.VideoImg);
                                                return newUploadUrl + imgModel.Img;
                                            }
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(articleModel.VideoImg))
                                                return articleModel.VideoImg; //[ 內文 主影片 影片截圖]
                                        }
                                        //都沒取到圖, 則繼續取段落的圖
                                        var paragraphs = ParagraphDAO.GetItems(articleModel.ID);
                                        if (paragraphs != null && paragraphs.Count() > 0)
                                        {
                                            foreach (ParagraphModels paragraph in paragraphs)
                                            {
                                                if (paragraph.MatchType == "img")
                                                {
                                                    IEnumerable<ResourceImagesModels> images = paragraph.GetImages().Where(m => m.IsShow);
                                                    if (images != null && images.Count() > 0)
                                                    {
                                                        ResourceImagesModels imgModel = images.FirstOrDefault();
                                                        return newUploadUrl + imgModel.Img;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case "ArticleIntro":
                                ArticleIntroModels articleIntroModel = ArticleIntroDAO.GetItem(cardList[0].No);
                                if (articleIntroModel != null)
                                {
                                    if (!string.IsNullOrEmpty(articleIntroModel.Icon)) // 取得[內文/頁面細節/自行設定代表圖]
                                    {
                                        ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(articleIntroModel.Icon);
                                        return newUploadUrl + imgModel.Img;
                                    }
                                    else
                                    {
                                        //都沒取到圖, 則繼續取段落的圖
                                        var paragraphs = ParagraphDAO.GetItems(articleIntroModel.ID);
                                        if (paragraphs != null && paragraphs.Count() > 0)
                                        {
                                            foreach (ParagraphModels paragraph in paragraphs)
                                            {
                                                if (paragraph.MatchType == "img")
                                                {
                                                    IEnumerable<ResourceImagesModels> images = paragraph.GetImages().Where(m => m.IsShow);
                                                    if (images != null && images.Count() > 0)
                                                    {
                                                        ResourceImagesModels imgModel = images.FirstOrDefault();
                                                        return newUploadUrl + imgModel.Img;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //ArticleSettingModels newSetting = ArticleSettingDAO.GetItem(MenuID);
                                    //string img = articleIntroModel.GetFirstImg(newSetting);
                                    //if (!string.IsNullOrEmpty(img))
                                    //    return newUploadUrl + img;
                                }
                                break;
                        }
                    }
                }
            }
            return "";
        }
        public static string GetContentDesc(long SiteID, long MenuID, long PageNo)
        {
            List<ZonesModels> zoneList = ZonesDAO.GetPageData(SiteID, PageNo);
            string newUploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(SiteID, MenuID).TrimEnd('/') + "/";

            if (zoneList != null && zoneList.Count > 0)
            {
                for (int i = 0; i < zoneList.Count; i++)
                {
                    List<CardsModels> cardList = CardsDAO.GetZoneData(SiteID, zoneList[i].No);
                    if (cardList != null && cardList.Count > 0)
                    {
                        switch (cardList[0].CardsType)
                        {
                            case "Article":
                                ArticleModels articleModel = ArticleDAO.GetItemByCard(cardList[0].No);
                                if (articleModel != null)
                                {
                                    var paragraphs = ParagraphDAO.GetItems(articleModel.ID);
                                    if (paragraphs != null && paragraphs.Count() > 0)
                                    {
                                        foreach (ParagraphModels paragraph in paragraphs)
                                        {
                                            if (!string.IsNullOrWhiteSpace(paragraph.Contents))
                                            {
                                                return paragraph.Contents.TrimTags().Truncate(100);
                                            }
                                        }
                                    }
                                }
                                break;
                            case "ArticleIntro":
                                ArticleIntroModels articleIntroModel = ArticleIntroDAO.GetItem(cardList[0].No);
                                if (articleIntroModel != null)
                                {
                                    var paragraphs = ParagraphDAO.GetItems(articleIntroModel.ID);
                                    if (paragraphs != null && paragraphs.Count() > 0)
                                    {
                                        foreach (ParagraphModels paragraph in paragraphs)
                                        {
                                            if (!string.IsNullOrWhiteSpace(paragraph.Contents))
                                            {
                                                return paragraph.Contents.TrimTags().Truncate(100);
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            return "";
        }
    }
}