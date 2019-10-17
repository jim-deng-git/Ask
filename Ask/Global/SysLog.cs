using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;
namespace WorkV3.Golbal
{
    public class SysLog
    {
        public static void SaveLog(SysActions _Actions, SysMgrNo? _MgrNo,string _ReMark,long? _SiteID =null, long? _MenuID = null, long? _SourceID = null)
        {

            SysLogModels data = new SysLogModels();
            data.Actions = (byte)_Actions;
            if (_MgrNo != null)
                data.MgrNo = (byte)_MgrNo;

            data.ReMark = _ReMark;
            data.SiteID = _SiteID;
            data.MenuID = _MenuID;
            data.SourceID = _SourceID;
            SysLogDAO.SaveInfo(data);         

        }
        #region Articles
        public static void SaveArticleEditLog(long ArticleID)
        {
            var articleInfo = ArticleDAO.GetItem(ArticleID);
            if (articleInfo != null)
            {
                var cardInfo = CardsDAO.GetByNo(articleInfo.CardNo);
                if (cardInfo != null && cardInfo.ZoneNo.HasValue)
                {
                    var zoneInfo = ZonesDAO.GetByNo(cardInfo.ZoneNo.Value);
                    if (zoneInfo != null)
                    {
                        SysLog.SaveLog(SysActions.Edit, SysMgrNo.Page, articleInfo.Title, articleInfo.SiteID, articleInfo.MenuID, zoneInfo.PageNo);
                    }
                }
            }
        }
        public static void SaveArticleDelLog(long ArticleID)
        {
            var articleInfo = ArticleDAO.GetItem(ArticleID);
            if (articleInfo != null)
            {
                var cardInfo = CardsDAO.GetByNo(articleInfo.CardNo);
                if (cardInfo != null && cardInfo.ZoneNo.HasValue)
                {
                    var zoneInfo = ZonesDAO.GetByNo(cardInfo.ZoneNo.Value);
                    if (zoneInfo != null)
                    {
                        SysLog.SaveLog(SysActions.Del, SysMgrNo.Page, articleInfo.Title, articleInfo.SiteID, articleInfo.MenuID, zoneInfo.PageNo);
                    }
                }
            }
        }
        #endregion
    }
}