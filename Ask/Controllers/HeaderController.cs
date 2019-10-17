using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Models;
using WorkV3.Models.DataAccess;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkLib;
using WorkV3.Common;
using WorkV3.Models.Repository;

namespace WorkV3.Controllers
{
    public class HeaderController : Controller
    {
        // GET: Header
        public ActionResult Index(CardsModels model)
        {
            WorkV3.Common.SitePage curPage = WorkV3.Models.DataAccess.CardsDAO.GetPage(model.No);
            ViewBag.SiteID = curPage.SiteID;
            List<MenusModels> Menus = Models.DataAccess.MenusDAO.GetFrontMenus(curPage.SiteID);
            SitesModels SitesInfo = SitesDAO.GetInfo(curPage.SiteID);
            WorkV3.Areas.Backend.Models.DataAccess.KeywordDAO keywordDao = new Areas.Backend.Models.DataAccess.KeywordDAO();    // neil 20180331 新增關鍵字搜尋
            ViewBag.SitesInfo = SitesInfo;

            #region WebLang
            List<SiteLangMenuModel> SiteLang = new List<SiteLangMenuModel>();
            if (uCheck.IsNumeric(curPage.SiteID))
            {
                if (SitesInfo.IsLang)//有開放再抓語言別資料
                {
                    SiteLang = SiteLangMenuDAO.GetDatas(curPage.SiteID);
                }
            }
            ViewBag.SiteLang = SiteLang;
            #endregion

            #region Socal
            List<SitesModels.socialSettingCont> social = new List<SitesModels.socialSettingCont>();
            if (SitesInfo != null)
            {
                if (!string.IsNullOrEmpty(SitesInfo.socialSetting))
                {
                    social = JsonConvert.DeserializeObject<List<SitesModels.socialSettingCont>>(SitesInfo.socialSetting);
                }
            }
            ViewBag.social = social;
            #endregion
            #region  會員功能機制

            Areas.Backend.Models.MemberShipRegSetModels MemberSetModel = Areas.Backend.Models.DataAccess.MemberShipRegSetDAO.GetItem(model.SiteID);
            bool IsEnabledMember = false;
            if (MemberSetModel != null)
            {
                IsEnabledMember = MemberSetModel.IsOpenReg;
            }
            ViewBag.IsEnabledMember = IsEnabledMember;
            #endregion

            ViewBag.SearchPage = SearchResultDAO.GetSearchPage(curPage.SiteID);
            ViewBag.VPath = WorkV3.Golbal.UpdFileInfo.GetVPathBySiteID(curPage.SiteID,"Header");
            ViewBag.VMemberPath = WorkV3.Golbal.UpdFileInfo.GetVPathBySiteID(curPage.SiteID, "Member");
            ViewBag.Keywords = keywordDao.GetAllKeywords(true);
            ViewBag.VMenuPath= Golbal.UpdFileInfo.GetVPathBySiteID(curPage.SiteID, "Menus");
            int style = model.StylesID;            
            return View("Style" + style, Menus);
        }
    }
}