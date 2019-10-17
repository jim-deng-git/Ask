using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Common;

namespace WorkV3.Areas.Backend.Controllers
{
    public class LogController : Controller
    {
        // GET: Backend/Log
        public ActionResult Log(int? index, SysLogSearchModel search)
        {
            if (Request.HttpMethod == "GET")
            {
                if (index == null)
                {
                    Utility.ClearSearchValue();
                    Session["LogSearch"] = null;
                }
                else
                {
                    SysLogSearchModel prevSearch = Utility.GetSearchValue<SysLogSearchModel>();
                    if (prevSearch != null)
                        search = prevSearch;
                }
            }
            else if (Request.HttpMethod == "POST")
            {
                Utility.SetSearchValue(search);
                Session["LogSearch"] = search;
            }

            string KW = search.KW ?? "";
            Pagination pagination = new Pagination
            {
                PageIndex = index ?? 1,
                PageSize = WebInfo.PageSize
            };

            string SDate = !string.IsNullOrEmpty(search.SDate) ? search.SDate : "1911/1/1";
            string EDate = !string.IsNullOrEmpty(search.EDate) ? search.EDate : "9999/12/31";

            int totalRecord;

            List<SysLogModels> items = SysLogDAO.GetItems(pagination.PageSize, pagination.PageIndex, out totalRecord, Convert.ToDateTime(SDate), Convert.ToDateTime(EDate), KW, search.Actions, search.MemberId);
            foreach (SysLogModels item in items)
            {
                string siteSN = "", pageSN = "";
                if(item.SiteID.HasValue)
                {
                    WorkV3.Models.SitesModels siteModel = WorkV3.Models.DataAccess.SitesDAO.GetInfo(item.SiteID.Value);
                    if (siteModel != null)
                    {
                        siteSN = siteModel.SN;

                        if (item.SourceID.HasValue)
                        {
                            WorkV3.Areas.Backend.Models.PagesModels pageModel = WorkV3.Areas.Backend.Models.DataAccess.PagesDAO.GetPageInfo(item.SiteID.Value, item.SourceID.Value);
                            if (pageModel != null)
                            {
                                List<WorkV3.Areas.Backend.ViewModels.CardsViewModel> zoneModel = WorkV3.Areas.Backend.Models.DataAccess.CardsDAO.GetZoneByPageNo(item.SiteID.Value, item.SourceID.Value);
                                if (zoneModel.Count > 0)
                                {
                                    bool HasCardContent = true;
                                    foreach (WorkV3.Areas.Backend.ViewModels.CardsViewModel cardModel in zoneModel)
                                    {
                                        switch (cardModel.CardsType)
                                        {
                                            case "Article":
                                                var articleModel = WorkV3.Models.DataAccess.ArticleDAO.GetItemByCard(cardModel.No);
                                                if (articleModel == null)
                                                {
                                                    HasCardContent = false;
                                                }
                                                break;
                                        }
                                        if (!HasCardContent)
                                            break;
                                    }
                                    if (HasCardContent)
                                        pageSN = pageModel.SN;
                                    else
                                        item.IsDeleted = true;
                                }
                                else
                                {
                                    item.IsDeleted = true;
                                }
                            }
                            else
                            {
                                if (item.MgrNo == (byte)WorkV3.Golbal.SysMgrNo.Page)
                                {
                                    item.IsDeleted = true;
                                }
                            }
                        }
                        
                    }
                }
                if (!string.IsNullOrEmpty(siteSN) && !string.IsNullOrEmpty(pageSN))
                    item.PageUrl = Url.Action("Index", "Home", new { SiteSN = siteSN, PageSN = pageSN, area = string.Empty });
            }
            pagination.TotalRecord = totalRecord;

            ViewBag.Pagination = pagination;
            ViewBag.Search = search;

            return View(items);
        }

        public FileResult Export(bool? privacy)
        {
            SysLogSearchModel search = Session[$"ExportSearch"] as SysLogSearchModel;
            if (search == null)
                search = new SysLogSearchModel();


            string SDate = !string.IsNullOrEmpty(search.SDate) ? search.SDate : "1911/1/1";
            string EDate = !string.IsNullOrEmpty(search.EDate) ? search.EDate : "9999/12/31";
            int totalRecord = 0;
            ViewData["Info"] = SysLogDAO.GetItems(int.MaxValue, 1, out totalRecord, Convert.ToDateTime(SDate), Convert.ToDateTime(EDate), search.KW, search.Actions, search.MemberId);

            ViewData["Privacy"] = privacy ?? false;

            string html = Utility.GetViewHtml(this, "Export", null);

            string title = $"Log{DateTime.Now.ToString("yyyyMMdd")}.xls";
            return File(System.Text.Encoding.UTF8.GetBytes(html), "application/vnd.ms-excel", title);
        }
    }
}