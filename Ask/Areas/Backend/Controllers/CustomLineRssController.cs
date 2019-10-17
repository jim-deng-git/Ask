using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Common;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;
using Newtonsoft.Json;
using WorkLib;

namespace WorkV3.Areas.Backend.Controllers
{
    public class CustomLineRssController : Controller
    {
        // GET: Backend/CustomLineRss
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(long siteId, int? index, CustomLineNewsSearchModels search)
        {
            ViewBag.SiteID = siteId;
            if (Request.HttpMethod == "GET")
            {
                if (index == null)
                    WorkV3.Common.Utility.ClearSearchValue();
                else
                {
                    CustomLineNewsSearchModels prevSearch = WorkV3.Common.Utility.GetSearchValue<CustomLineNewsSearchModels>();
                    if (prevSearch != null)
                        search = prevSearch;
                }
            }
            else if (Request.HttpMethod == "POST")
            {
                WorkV3.Common.Utility.SetSearchValue(search);
            }
            ViewBag.Search = search;

            Pagination pagination = new Pagination
            {
                PageIndex = index ?? 1,
                PageSize = WebInfo.PageSize
            };

            int totalRecord;
            IEnumerable<CustomLineNewsModels> items = CustomLineNewsDAO.GetItems(search, pagination.PageSize, pagination.PageIndex, out totalRecord);
            pagination.TotalRecord = totalRecord;

            ViewBag.Pagination = pagination;
            return View(items);
        }

        public ActionResult EditList(long siteId, long? SourceID, int? index, CustomLineNewsSearchModels search)
        {
            ViewBag.SiteID = siteId;
            ViewBag.sId = SourceID;
            //Pagination pagination = new Pagination
            //{
            //    PageIndex = index ?? 1,
            //    PageSize = WebInfo.PageSize
            //};

            //int totalRecord;
            //IEnumerable<CustomLineNewsModels> items = CustomLineNewsDAO.GetItems(search, pagination.PageSize, pagination.PageIndex, out totalRecord);
            //pagination.TotalRecord = totalRecord;

            //ViewBag.Pagination = pagination;
            //return View(items);
            return View();

        }

        public ActionResult AddData(long siteId,long? SourceID, string key, int? index, CustomLineNewsSearchModels search)
        {
            ViewBag.SiteID = siteId;
           

            ViewBag.MenuList = CustomLineNewsDAO.GetMenusItems(siteId);


            if (Request.HttpMethod == "GET")
            {
                if (Request["isReturn"] != null)
                    ViewBag.Exit = true;

                if (index == null)
                    WorkV3.Common.Utility.ClearSearchValue();
                else
                {
                    CustomLineNewsSearchModels prevSearch = WorkV3.Common.Utility.GetSearchValue<CustomLineNewsSearchModels>();
                    if (prevSearch != null)
                        search = prevSearch;
                }
            }
            else if (Request.HttpMethod == "POST")
            {
                if (SourceID > 0)
                {
                    ViewBag.SourceID = SourceID;
                    //ViewBag.SelectDate = search.SelectDate;
                }
                else
                {
                    ViewBag.SelectDate = Request["txtSelectDate"];                    
                }
                WorkV3.Common.Utility.SetSearchValue(search);
            }

            ViewBag.Search = search;

            Pagination pagination = new Pagination
            {
                PageIndex = index ?? 1,
                PageSize = WebInfo.PageSize
            };

            int totalRecord;
            IEnumerable<CustomLineNewsDataModels> items = CustomLineNewsDAO.GetDataSearchItems(search, pagination.PageSize, pagination.PageIndex, out totalRecord);
            pagination.TotalRecord = totalRecord;

            ViewBag.Pagination = pagination;
            return View(items);
        }

        [HttpPost]
        public ActionResult AddDataInsert(long siteId)
        {
            long SelectMenuID = long.Parse(Request["SelectMenuID"].ToString());
            long SourceID = 0;
            string SelectDate = Request["SelectDate"].ToString();

            if (uCheck.IsNumeric(Request["SourceID"])){
                SourceID = long.Parse(Request["SourceID"].ToString());
            }

            string _SelectIDs = Request["SelectIDs"];
            IEnumerable<long> SelectIDs = Newtonsoft.Json.JsonConvert.DeserializeObject<long[]>(_SelectIDs);
            CustomLineNewsDAO.InsertLineNewsData(siteId, SelectDate, SourceID, SelectMenuID, SelectIDs);

           
            ViewBag.SiteID = siteId;

            return RedirectToAction("AddData", "CustomLineRss", new { siteId = siteId, SourceID = SourceID, SelectMenuID = SelectMenuID, isReturn = 1 });
        }

        public JsonResult GetSelectDateIsExists(long SiteID ,string SelectDate)
        {
            bool RT = CustomLineNewsDAO.isExistSelectDate(SiteID, SelectDate);
            return Json(RT, JsonRequestBehavior.AllowGet);
        }
    }
}