using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Common;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Models.Interface;
using WorkV3.Areas.Backend.ViewModels.Intro;

namespace WorkV3.Areas.Backend.Controllers
{
    public class IntroController : Controller
    {
        private IPageRepository<WorkV3.Models.PagesModels> pageRepository;
        private string homePageSN = "Index";

        public IntroController(IPageRepository<WorkV3.Models.PagesModels> inPageRepository)
        {
            pageRepository = inPageRepository;
        }

        public ActionResult List(long siteId, long menuId)
        {
            WorkV3.Models.PagesModels pageModel;
            try
            {
                pageModel = pageRepository.GetItem(menuId, "MenuID");
            }
            catch(Exception)
            {
                pageModel = pageRepository.GetItem(new Dictionary<string, object> {
                    { "SiteID", siteId },
                    { "SN", homePageSN }
                });
            }
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.PageNo = pageModel.No;

            var query = new Query();
            query.Where.Add(new QWhere("SiteID", COperator.Equal, siteId));
            query.Where.Add(new QWhere("Title", COperator.NoEqual, " "));
            var forms = WorkV3.Models.FormDAO.Instance.Get(query);

            ViewBag.FormItem = forms;

            var model = new IntroListVIewModel();
            model.Cards = CardsDAO.GetZoneByPageNo(siteId, pageModel.No);
            model.SiteID = siteId;
            model.MenuID = menuId;
            model.PageNo = pageModel.No;
            model.Forms = forms;
            model.FormTitle = pageModel.Title;

            return View(model);
        }

        public ActionResult Edit(long No)
        {
            var model = CardsDAO.GetByNo(No);
            if (model == null)
                return RedirectToAction("List");

            if (string.IsNullOrWhiteSpace(model.Descriptions))
                model.Descriptions = "請填入內文";
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CardsModels model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsValid = false;
                return View(model);
            }

            var strDescriptions = model.Descriptions.ReplaceEnterToBr().TrimTags();
            if (strDescriptions == "請填入內文")
                model.Descriptions = "";

            DateTime now = DateTime.Now;
            model.Modifier = MemberDAO.SysCurrent.Id;
            model.ModifyTime = now;

            int result = CardsDAO.UpdateTitleAndDescriptions(model.No, model.Title, model.Descriptions);
            if (result == 0)
                ViewBag.EditResult = "儲存發生錯誤";
            else
                ViewBag.EditResult = "儲存成功";

            return View(model);
        }

        [HttpPost]
        public int ChangeStatus(long No, int Status)
        {
            return CardsDAO.UpdateStatus(No, Status);
        }

        [HttpPost]
        public void Sort(IEnumerable<SortItem> items) {
            CardsDAO.Sort(items);
        }

        #region 新增ADD
        public ActionResult AddMode(long SiteID, long MenuID, long PageNo)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.PageID = PageNo;
            ViewBag.DefaultZoneName = GetDefaultSN(SiteID, PageNo);

            var siteItem = WorkV3.Models.DataAccess.SitesDAO.GetInfo(SiteID);

            List<CardsTypeModels> CList = CardsTypeDAO.GetData();

            //20190715 Nina 徵才站台只顯示文章集合卡、徵才集合卡、文字卡
            if(siteItem.SN.ToLower() == "career")
                CList = CList.Where(m => m.Code == "ArticleSet" || m.Code == "RecruitSet" || m.Code == "PlainText").ToList();

            ViewBag.CardsTypeList = CList.Where(dr => dr.isIndexCards).ToList();

            return View();
        }
        // GET: BackEnd
        [HttpPost]
        public ActionResult AddMode(long SiteID, long MenuID, long PageNo, string DataType, string ZoneSN)
        {

            List<CardsTypeModels> CList = CardsTypeDAO.GetData();
            ViewBag.CardsTypeList = CList.Where(dr => dr.isIndexCards).ToList();
            if (string.IsNullOrEmpty(DataType))
            {
                return new HttpNotFoundResult();
            }
            var selectCardsType = CList.Where(dr => dr.isIndexCards && dr.Code == DataType);
            if (selectCardsType == null || selectCardsType.Count() <= 0)
            {
                return new HttpNotFoundResult();
            }
            var zoneModels = WorkV3.Models.DataAccess.ZonesDAO.GetPageData(SiteID, PageNo);
            WorkV3.Areas.Backend.Models.ZonesModels zModel = new ZonesModels();
            zModel.No = WorkLib.GetItem.NewSN();
            zModel.Ver = 1;
            zModel.SiteID = SiteID;
            zModel.PageNo = PageNo;
            zModel.SN = ZoneSN;
            zModel.Sort = Convert.ToByte(zoneModels.Count - 1 < 0 ? 1 : zoneModels.Count);
            zModel.StyleID = 1;
            zModel.Creator = MemberDAO.SysCurrent.Id;
            zModel.CreateTime = DateTime.Now;
            WorkV3.Areas.Backend.Models.CardsModels cModel = new CardsModels();
            cModel.No = WorkLib.GetItem.NewSN();
            cModel.Lang = "zh-Hant";
            cModel.Ver = 1;
            cModel.ZoneNo = zModel.No;
            cModel.SN = selectCardsType.First().Code;
            //cModel.Title = ZoneSN;
            cModel.Title = selectCardsType.First().Title;
            cModel.CardsType = selectCardsType.First().Code;
            cModel.StylesID = 1;
            cModel.Status = 1;
            cModel.Creator = MemberDAO.SysCurrent.Id;
            cModel.CreateTime = DateTime.Now;
            WorkV3.Areas.Backend.Models.DataAccess.ZonesDAO.SetZoneInfo(zModel);
            WorkV3.Areas.Backend.Models.DataAccess.CardsDAO.SetCardInfo(cModel);


            TempData["refreshData"] = 1;

            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.PageID = PageNo;
            ViewBag.DefaultZoneName = GetDefaultSN(SiteID, PageNo);
            return View();
            //Response.Redirect(Request.RawUrl);
        }
        private string GetDefaultSN(long SiteID, long PageNo)
        {
            int defaultZoneNo = 1;
            string DefaultZoneName = "zone";
            var zoneList = WorkV3.Models.DataAccess.ZonesDAO.GetPageData(SiteID, PageNo);
            if (zoneList != null && zoneList.Count > 0)
            {
                var zones = zoneList.Where(p => !string.IsNullOrEmpty(p.SN) && p.SN.ToLower().StartsWith("zone"));
                if (zones != null && zones.Count() > 0)
                {
                    var orderzones = zones.OrderByDescending(s => s.SN);
                    foreach (var orderzone in orderzones)
                    {
                        try
                        {
                            int zoneNo = int.Parse(orderzone.SN.Substring(4));
                            defaultZoneNo = zoneNo + 1;
                            break;
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }
            DefaultZoneName = DefaultZoneName + defaultZoneNo.ToString("00");
            return DefaultZoneName;
        }
        [HttpGet]
        public bool CheckSN(long SiteID, long PageNo, long? ZoneNo, string SN)
        {
            var zoneList = WorkV3.Models.DataAccess.ZonesDAO.GetPageData(SiteID, PageNo);
            if (zoneList != null && zoneList.Count > 0)
            {
                var zones = zoneList.Where(p => !string.IsNullOrEmpty(p.SN) && p.SN.ToLower() == SN.ToLower());
                if (zones != null && zones.Count() > 0)
                {
                    foreach (var zone in zones)
                    {
                        if (ZoneNo.HasValue)
                        {
                            if (ZoneNo.Value == zone.No)
                            {
                                continue;
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        [HttpPost]
        public ActionResult Del(IEnumerable<long> ids)
        {
            WorkV3.ViewModels.AjaxResponseViewModel response = new WorkV3.ViewModels.AjaxResponseViewModel();

            try
            {
                List<long> CardIDList = new List<long>();
                foreach (long ZoneID in ids)
                {
                    var zoneModel = WorkV3.Areas.Backend.Models.DataAccess.ZonesDAO.GetByNo(ZoneID);
                    if (zoneModel != null)
                    {
                        var cardModels = WorkV3.Areas.Backend.Models.DataAccess.CardsDAO.GetZoneData(zoneModel.SiteID, zoneModel.No);
                        if (cardModels != null && cardModels.Count > 0)
                        {
                            CardIDList.AddRange(cardModels.Select(c => c.No));
                        }
                    }
                }
                CommonDA.Delete("Cards", "No", CardIDList);
                CommonDA.Delete("Zones", "No", ids);
            }
            catch (Exception ex)
            {
                response.Success = false;
            }

            return Json(response);
        }
    }
}