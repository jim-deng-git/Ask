using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Areas.Backend.ViewModels;
using WorkV3.Common;

namespace WorkV3.Areas.Backend.Controllers
{
    public class AdvertisementController : Controller
    {
        #region 廣告區管理
        /// <summary>
        /// 廣告區管理 Index
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="siteId"></param>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public ActionResult Index(AdvertisementSearchModel search, int? page, long siteId = 1, long menuId = 2000)
        {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;

            if (Request.HttpMethod == "GET")
            {
                if (page == null)
                    WorkV3.Common.Utility.ClearSearchValue();
                else
                {
                    AdvertisementSearchModel prevSearch = WorkV3.Common.Utility.GetSearchValue<AdvertisementSearchModel>();
                    if (prevSearch != null)
                        search = prevSearch;
                }
            }
            else if (Request.HttpMethod == "POST")
            {
                WorkV3.Common.Utility.SetSearchValue(search);
                ViewBag.IsSearchMode = "IsSearchMode";
            }
            ViewBag.Search = search;

            List<AdvertisementModel> list = new List<AdvertisementModel>();
            Pagination pagination = new Pagination
            {
                PageIndex = page ?? 1,
                PageSize = WebInfo.PageSize
            };
            int totalRecord = 0;
            list = AdvertisementDAO.GetAdvertisement(search,pagination.PageSize, pagination.PageIndex, siteId, out totalRecord);
            pagination.TotalRecord = totalRecord;
            ViewBag.Pagination = pagination;


            return View(list);
        }
        /// <summary>
        /// 廣告區編輯
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="menuId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AdvertisementEdit(long siteId, long menuId, long? id)
        {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;

            AdvertisementModel item = null;
            if (id != null)
                item = AdvertisementDAO.GetAdvertisementItem((long)id);
            if (item == null)
                item = new AdvertisementModel { ID = WorkLib.GetItem.NewSN() };

            return View(item);
        }
        /// <summary>
        /// 廣告區編輯 POST
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult AdvertisementEdit(AdvertisementModel model)
        {
            AdvertisementDAO.SetAdvertisementItem(model);
            ViewBag.SiteID = model.SiteID;
            ViewBag.MenuID = model.MenuID;
            ViewBag.Exit = true;

            return View(model);
        }
        /// <summary>
        /// 廣告區刪除
        /// </summary>
        /// <param name="ids"></param>
        [HttpPost]
        public void AdvertisementDel(IEnumerable<long> ids)
        {
            AdvertisementDAO.AdvertisementDelete(ids);
        }
        #endregion

        #region 自訂廣告管理
        /// <summary>
        /// 自訂廣告管理 Index
        /// </summary>
        /// <param name="Advertisement_ID">廣告區 ID</param>
        /// <returns></returns>
        public ActionResult AdsCustomizeIndex(AdsCustomizeSearchModel search, long siteId, long menuId, long Advertisement_ID, int? page)
        {
            if (Request.HttpMethod == "GET")
            {
                if (page == null)
                    WorkV3.Common.Utility.ClearSearchValue();
                else
                {
                    AdsCustomizeSearchModel prevSearch = WorkV3.Common.Utility.GetSearchValue<AdsCustomizeSearchModel>();
                    if (prevSearch != null)
                        search = prevSearch;
                }
            }
            else if (Request.HttpMethod == "POST")
            {
                WorkV3.Common.Utility.SetSearchValue(search);
                ViewBag.IsSearchMode = "IsSearchMode";
            }
            ViewBag.Search = search;

            List<AdsCustomizeModel> list = new List<AdsCustomizeModel>();
            Pagination pagination = new Pagination
            {
                PageIndex = page ?? 1,
                PageSize = WebInfo.PageSize
            };
            int totalRecord = 0;
            list = AdvertisementDAO.GetAdsCustomize(search,pagination.PageSize, pagination.PageIndex, Advertisement_ID,siteId, out totalRecord);
            pagination.TotalRecord      = totalRecord;
            ViewBag.Pagination          = pagination;

            ViewBag.UploadUrl           = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);
            ViewBag.Advertisement_ID    = Advertisement_ID;
            ViewBag.SiteID              = siteId;
            ViewBag.MenuID              = menuId;

            AdvertisementModel ad = AdvertisementDAO.GetAdvertisementItem(Advertisement_ID);
            ViewBag.HasComputerVer      = ad.IsUseForComputer;

            return View(list);
        }
        /// <summary>
        /// 自訂廣告管理 變更刊登狀態
        /// </summary>
        /// <returns></returns>
        public void AdsCustomizeToggleIssue(long id)
        {
            AdvertisementDAO.ToggleIssue(id, "AdsCustomize");
        }
        /// <summary>
        /// 自訂廣告管理 排序設定
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public void AdsCustomizeSort(IEnumerable<SortItem> items)
        {
            AdvertisementDAO.SortEdit(items, "AdsCustomize");
        }
        /// <summary>
        /// 自訂廣告管理 編輯
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="menuId"></param>
        /// <param name="Advertisement_ID">廣告區 ID</param>
        /// <param name="id">AdsCustomize.ID 自訂廣告管理ID</param>
        /// <returns></returns>
        public ActionResult AdsCustomizeEdit(long siteId, long menuId, long Advertisement_ID, long? id)
        {
            ViewBag.IsEdit = false;
            AdsCustomizeModel item = null;
            if (id != null) //如果有傳 id 進來的話
            {
                item = AdvertisementDAO.GetAdsCustomizeItem((long)id);

                // 20180621 neil 
                // 如果 item 有抓到東西，代表本來就存進資料庫裡
                if (item != null)
                {
                    ViewBag.IsEdit = true;
                }
                // 如果 item 沒有抓到東西，代表目前的 id 是之前產生的，但尚未存入資料庫。
                // 主要用處是在自訂廣告編輯頁時會增加點擊事件、廣告主以及費用設定三項資料，
                // 因為上述三項資料和本筆資料的 table 不同加上儲存時會重新整理本頁，所以原先設定是需先存入，取得本筆資料 ID 後才能夠新增上述三項資料，
                // 但流程上貌似不順，所以改為在本頁編輯時，要能夠新增上述資料，新增後本頁重新整理時會再回傳之前產生的 id ，再用來抓取之前儲存的資料。
                else
                {
                    string clickEvent = "";
                    AdsCustomizeToVideoModel video = AdvertisementDAO.GetAdsCustomizeVideoItem((long)id);
                    AdsCustomizeToLinkModel link = AdvertisementDAO.GetAdsCustomizeLinkItem((long)id);
                    if (video != null)
                        clickEvent = "Video";
                    else if (link != null)
                        clickEvent = "Link";

                    item = new AdsCustomizeModel
                    {
                        ID = (long)id,
                        Advertisement_ID = Advertisement_ID,
                        IsIssue = true,
                        ClickEvent = clickEvent
                    };
                }
            }
            if (item == null)
                item = new AdsCustomizeModel
                {
                    ID = WorkLib.GetItem.NewSN(),
                    Advertisement_ID = Advertisement_ID,
                    ClickEvent = ClickEvent.None,
                    IsIssue = true
                };

            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);
            ViewBag.Advertisement_ID = Advertisement_ID;
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            return View(item);
        }
        /// <summary>
        /// 自訂廣告管理 編輯 (POST)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult AdsCustomizeEdit(AdsCustomizeModel model)
        {
            AdsCustomizeEditSaveImage(model.SiteID, model.MenuID, model);

            AdvertisementDAO.SetCustomizeItem(model);
            ViewBag.SiteID = model.SiteID;
            ViewBag.MenuID = model.MenuID;
            ViewBag.Advertisement_ID = model.Advertisement_ID;
            ViewBag.IsEdit = true;
            ViewBag.Exit = true;

            return View(model);
        }
        /// <summary>
        /// 回傳自訂廣告的連結資料
        /// </summary>
        /// <param name="adsCustomizeId"></param>
        /// <returns></returns>
        public string GetLnkInfo(long adsCustomizeId)
        {
            List<dynamic> info = AdvertisementDAO.GetAdsCustomizeLinkInfo(adsCustomizeId).ToList();

            JsonResult jr = Json(info);
            return new JavaScriptSerializer().Serialize(jr.Data);
        }
        /// <summary>
        /// 回傳自訂廣告的刊登時間和費用設定資料
        /// </summary>
        /// <param name="adsCustomizeId"></param>
        /// <returns></returns>
        public string GetAdsCustomizeAccountSetInfo(long adsCustomizeId)
        {
            return new JavaScriptSerializer().Serialize(Json(AdvertisementDAO.QueryAccountSetByAdsCustomizeID(adsCustomizeId)).Data);
        }
        void AdsCustomizeEditSaveImage(long siteId, long menuId, AdsCustomizeModel item)
        {
            if (!string.IsNullOrWhiteSpace(item.PCPicture))
            {
                ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(item.PCPicture);
                if (imgModel.ID == 0)
                { 
                    // 新上傳的圖片
                    HttpPostedFileBase postedFile = Request.Files["fPCPicture"];
                    string postedFileBase64 = Request.Form["fPCPictureBase64"];
                    string postedFileBase64_Resize = Request.Form["fPCPictureBase64_Resize"];
                    if (postedFile == null || postedFile.ContentLength == 0)
                    {
                        item.PCPicture = string.Empty;
                    }
                    else
                    {
                        string saveName = WorkV3.Golbal.UpdFileInfo.SaveFilesByMenuID(postedFile, siteId, menuId, postedFileBase64, postedFileBase64_Resize);
                        imgModel.ID = 1;
                        imgModel.Img = saveName;

                        item.PCPicture = Newtonsoft.Json.JsonConvert.SerializeObject(imgModel);
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(item.MobilePicture))
            {
                ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(item.MobilePicture);
                if (imgModel.ID == 0)
                {
                    // 新上傳的圖片
                    HttpPostedFileBase postedFile = Request.Files["fMobilePicture"];
                    string postedFileBase64 = Request.Form["fMobilePictureBase64"];
                    string postedFileBase64_Resize = Request.Form["fMobilePictureBase64_Resize"];
                    if (postedFile == null || postedFile.ContentLength == 0)
                    {
                        item.MobilePicture = string.Empty;
                    }
                    else
                    {
                        string saveName = WorkV3.Golbal.UpdFileInfo.SaveFilesByMenuID(postedFile, siteId, menuId, postedFileBase64, postedFileBase64_Resize);
                        imgModel.ID = 1;
                        imgModel.Img = saveName;

                        item.MobilePicture = Newtonsoft.Json.JsonConvert.SerializeObject(imgModel);
                    }
                }
            }
        }
        /// <summary>
        /// 自訂廣告區刪除
        /// </summary>
        /// <param name="ids"></param>
        [HttpPost]
        public void AdsCustomizeDel(IEnumerable<long> ids)
        {
            AdvertisementDAO.AdsCustomizeDelete(ids);
        }
        /// <summary>
        /// 自訂廣告複製
        /// </summary>
        /// <param name="ids">被複製自訂廣告的ID</param>
        /// <param name="siteId"></param>
        /// <param name="menuId"></param>
        /// <param name="AdvertisementID">廣告(區)主檔 ID</param>
        [HttpPost]
        public void AdsCustomizeCopy(IEnumerable<long> ids, long siteId, long menuId, long AdvertisementID)
        {
            WorkV3.Areas.Backend.Models.DataAccess.AdvertisementDAO.AdsCustomizeCopy(ids, siteId, menuId, AdvertisementID);
        }
        /// <summary>
        /// 新增廣告主
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAdvertisers(long siteId)
        {
            IEnumerable<AdvertisersModel> advertisers = AdvertisementDAO.GetAllAdvertisers();
            ViewBag.SiteID = siteId;
            ViewBag.Advertisers = advertisers;
            return View();
        }
        [HttpPost]
        public ActionResult FastCreateAdvertiser(long SiteID,string CompanyName)
        {
            long NewID = WorkLib.GetItem.NewSN();
            AdvertisersModel create = new AdvertisersModel()
            {
                ID = NewID,
                SiteID = SiteID,
                CompanyName = CompanyName,
                ContactPerson = "",
                JobTitle = "",
                ContactPhone = "",
                IsIssue = true,
                Sort = int.MaxValue
            };
            try
            {
                WorkV3.Areas.Backend.Models.DataAccess.AdvertisementDAO.SetAdvertisersItem(create);
            }
            catch
            {
                return Json("undefined");
            }
            return Json(create);
        }
        /// <summary>
        /// 加入廣告主
        /// </summary>
        /// <param name="adsCustomizeId"></param>
        /// <param name="advertisersId"></param>
        [HttpPost]
        public void saveAdvertisers(long adsCustomizeId, long advertisersId)
        {
            AdvertisementDAO.saveAdvertisers(adsCustomizeId, advertisersId);
        }
        /// <summary>
        /// 廣告主搜尋
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string AdvertisersSearch(string key,long siteId)
        {
            var result = AdvertisementDAO.SearchAdvertisersItems(key, siteId);
            return Newtonsoft.Json.JsonConvert.SerializeObject(result, new Golbal.Int64Converter());
        }
        /// <summary>
        /// 刪除廣告主(自訂廣告區部分)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public void DelAdvertiserByAdvertisement(long AdsCustomize_ID)
        {
            AdvertisementDAO.RemoveAdvertisersInAdsCustomize(AdsCustomize_ID);
        }
        /// <summary>
        /// 刊登時間和費用設定
        /// </summary>
        /// <returns></returns>
        public ActionResult AdsCustomizeAccountSetEdit(long AdsCustomize_ID,long? id)
        {
            AdsCustomizeAccountSet item = null;
            if (id != null)
                item = AdvertisementDAO.GetAdsCustomizeAccountItem((long)id);
            if (item == null)
                item = new AdsCustomizeAccountSet
                {
                    ID = WorkLib.GetItem.NewSN(),
                    AdsCustomize_ID = AdsCustomize_ID,
                    IsIssue = true
                };


            return View(item);
        }
        /// <summary>
        /// 刊登時間和費用設定 POST
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult AdsCustomizeAccountSetEdit(AdsCustomizeAccountSet model)
        {
            AdvertisementDAO.SetAdsCustomizeAccount(model);
            ViewBag.Exit = true;
            return View(model);
        }

        public string IsDurationOverlapped(long ID, long AdsCustomize_ID, DateTime startTime, DateTime endTime)
        {
            List<AdsCustomizeAccountSet> accountSet = new List<AdsCustomizeAccountSet>();
            AdsCustomizeModel adsCustomizeObj = AdvertisementDAO.GetAdsCustomizeItem(AdsCustomize_ID);
            if (adsCustomizeObj == null)
                accountSet = AdvertisementDAO.QueryAccountSetByAdsCustomizeID(AdsCustomize_ID);
            else
                accountSet = adsCustomizeObj.AdsCustomizeAccountSet.ToList();


            foreach (var item in accountSet)
            {
                // 區間不完整不計算
                if (item.IssueStart == null || item.IssueEnd == null)
                {
                    continue;
                }

                // 如果本身儲存時沒有修改時間的話不計算
                if (item.ID == ID)
                {
                    continue;
                }

                List<DateTime> timePoints = new List<DateTime>();
                timePoints.Add(startTime);
                timePoints.Add(endTime);
                timePoints.Add((DateTime)item.IssueStart);
                timePoints.Add((DateTime)item.IssueEnd);

                if (AdvertisementDAO.IsDurationOverlapped(startTime, endTime, (DateTime)item.IssueStart, (DateTime)item.IssueEnd))
                    return "1";
            }

            return "0";
        }

        /// <summary>
        /// 刪除刊登費用
        /// </summary>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public void AccountDel(IEnumerable<long> ids)
        {
            AdvertisementDAO.AccountDel(ids);
        }
        /// <summary>
        /// 點擊事件_連結 編輯
        /// </summary>
        /// <param name="AdsCustomize_ID"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult LinkEdit(long AdsCustomize_ID)
        {
            AdsCustomizeToLinkModel item = AdvertisementDAO.GetAdsCustomizeLinkItem(AdsCustomize_ID);
            if (item == null)
                item = new AdsCustomizeToLinkModel
                {
                    ID = WorkLib.GetItem.NewSN(),
                    AdsCustomize_ID = AdsCustomize_ID
                };
            return View(item);
        }
        /// <summary>
        /// 點擊事件_連結 編輯 POST
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult LinkEdit(AdsCustomizeToLinkModel model)
        {
            AdvertisementDAO.SetAdsCustomizeLinkEdit(model);
            ViewBag.Exit = true;
            return View(model);
        }
        /// <summary>
        /// 點擊事件_影片 編輯
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="menuId"></param>
        /// <param name="AdsCustomize_ID"></param>
        /// <returns></returns>
        public ActionResult VideoEdit(long siteId, long menuId, long AdsCustomize_ID)
        {
            AdsCustomizeToVideoModel item = AdvertisementDAO.GetAdsCustomizeVideoItem(AdsCustomize_ID);
            if (item == null)
            {
                item = new AdsCustomizeToVideoModel
                {
                    ID = WorkLib.GetItem.NewSN(),
                    AdsCustomize_ID = AdsCustomize_ID
                };
            }

            ViewBag.SiteID      = siteId;
            ViewBag.MenuID      = menuId;
            ViewBag.UploadUrl   = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);

            if (item.Screenshot==null)
                item.Screenshot = WorkLib.GetItem.NewSN();

            return View(item);
        }
        /// <summary>
        /// 點擊事件_影片 編輯 POST
        /// </summary>
        /// <param name="model"></param>
        /// <param name="imgs"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult VideoEdit(AdsCustomizeToVideoModel model, string[] imgs)
        {
            AdvertisementDAO.SetAdsCustomizeVideoEdit(model);
            ViewBag.SiteID = model.SiteID;
            ViewBag.MenuID = model.MenuID;
            ViewBag.Exit = true;
            return View(model);
        }
        /// <summary>
        /// 刪除點擊事件-影片
        /// </summary>
        /// <param name="AdsCustomize_ID"></param>
        [HttpPost]
        public void DelVideoByAdsCustom(long AdsCustomize_ID)
        {
            AdvertisementDAO.DelVideoByAdsCustom(AdsCustomize_ID);
        }
        /// <summary>
        /// 刪除點擊事件-連結
        /// </summary>
        /// <param name="AdsCustomize_ID"></param>
        [HttpPost]
        public void DelLinksByAdsCustom(long AdsCustomize_ID)
        {
            AdvertisementDAO.DelLinksByAdsCustom(AdsCustomize_ID);
        }
        #endregion

        #region 顯示位置
        /// <summary>
        /// 顯示位置 Index
        /// </summary>
        /// <param name="Advertisement_ID"></param>
        /// <returns></returns>
        public ActionResult AdsDisplaySetting(long Advertisement_ID,long SiteID)
        {
            //產生tree
            List<AdsDisplayAreaTrees> tree = AdvertisementDAO.GetDisplayAreaTree(Advertisement_ID, SiteID).OrderBy(m => m.AreaID).ThenBy(m => m.Sort).ToList();
            ViewBag.Advertisement_ID = Advertisement_ID;

            return View(new AdsDisplayAreaSetIndexModel() { tree = tree });
        }
        /// <summary>
        /// 顯示位置的設定
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Advertisement_ID"></param>
        /// <param name="siteId"></param>
        /// <param name="menuId"></param>
        /// <param name="DataType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AdsDisplayPositionAdd(string Title, long Advertisement_ID, long siteId, long menuId, string DataType)
        {            
            IEnumerable<AdsDisplayAreaSetModel> items = AdvertisementDAO.GetAdsDisplayPositionItems(Advertisement_ID, menuId);

            AdsPositionSetViewModel model = new AdsPositionSetViewModel();

            model.ListGroup = items.Where(m => m.ChildType == null).Select(m => m.GroupPosition).ToArray();
            model.InsideGroup = items.Where(m => m.ChildType == "Inside").Select(m => m.GroupPosition).ToArray();
            model.LoginGroup = items.Where(m => m.ChildType == "Login").Select(m => m.GroupPosition).ToArray();
            model.SiteId = siteId;
            model.MenuId = menuId;
            model.DataType = DataType;
            model.AdvertisementId = Advertisement_ID;

            if(items.Any(m => m.GroupPosition == "Overlay" && m.ChildType == null))
            {
                var item = items.Where(m => m.GroupPosition == "Overlay").FirstOrDefault();
                model.ListOverlayType = item.OverlayType;
                model.ListOverlayIdleSeconds = item.OverlayIdleSeconds;
                model.ListOverlayRepeatSeconds = item.OverlayRepeatSeconds / 60;
                model.ListOverlayChance = item.OverlayChance;
            }

            if(items.Any(m => m.GroupPosition == "Overlay" && m.ChildType == "Inside"))
            {
                var item = items.Where(m => m.GroupPosition == "Overlay").FirstOrDefault();
                model.InsideOverlayType = item.OverlayType;
                model.InsideOverlayIdleSeconds = item.OverlayIdleSeconds;
                model.InsideOverlayRepeatSeconds = item.OverlayRepeatSeconds / 60;
                model.InsideOverlayChance = item.OverlayChance;
            }

            if(items.Any(m => m.GroupPosition == "Overlay" && m.ChildType == "Login"))
            {
                var item = items.Where(m => m.GroupPosition == "Overlay").FirstOrDefault();
                model.LoginOverlayType = item.OverlayType;
                model.LoginOverlayIdleSeconds = item.OverlayIdleSeconds;
                model.LoginOverlayRepeatSeconds = item.OverlayRepeatSeconds / 60;
                model.LoginOverlayChance = item.OverlayChance;
            }

            ViewBag.Title = string.Format("{0} 廣告區顯示設定", Title);
            return View("AdsDisplayPositionAdd_"+ DataType, model);
        }
        /// <summary>
        /// 顯示位置的設定 POST
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult AdsDisplayPositionAdd(AdsPositionSetViewModel model)
        {
            string msg = string.Empty;
            if (model.SiteId != 0)
                AdvertisementDAO.SetAdsDisplayPositionItem(model, out msg);
            ViewBag.Exit = true;
            ViewBag.ResultMsg = msg;
            return View("AdsDisplayPositionAdd_"+ model.DataType, model);
        }
        /// <summary>
        /// 初始化位置設定的附屬資料
        /// </summary>
        /// <param name="model"></param>
        /// <param name="DataType"></param>
        private void initailAreaSetChildModel(AdsDisplayAreaSetModel model,string DataType)
        {
            switch(DataType)
            {
                case AreaSetDataType.Article:
                    if (model.InsideAreaSet == null)
                    {
                        model.InsideAreaSet = new AdsDisplayAreaSetModel()
                        {
                            ID = WorkLib.GetItem.NewSN(),
                            ParentAdsDisplayAreaSetID = model.ID,
                            ChildType = WorkV3.Areas.Backend.Models.ChildType.Inside,
                            Advertisement_ID = model.Advertisement_ID,
                            MenuID = model.MenuID
                        };
                    }
                    break;
                case AreaSetDataType.Event:
                    if (model.InsideAreaSet == null)
                    {
                        model.InsideAreaSet = new AdsDisplayAreaSetModel()
                        {
                            ID = WorkLib.GetItem.NewSN(),
                            ParentAdsDisplayAreaSetID = model.ID,
                            ChildType = WorkV3.Areas.Backend.Models.ChildType.Inside,
                            Advertisement_ID = model.Advertisement_ID,
                            MenuID = model.MenuID
                        };
                    }
                    break;
                case AreaSetDataType.Member:
                    if (model.LoginAreaSet == null)
                    {
                        model.LoginAreaSet = new AdsDisplayAreaSetModel()
                        {
                            ID = WorkLib.GetItem.NewSN(),
                            ParentAdsDisplayAreaSetID = model.ID,
                            ChildType = WorkV3.Areas.Backend.Models.ChildType.Login,
                            Advertisement_ID = model.Advertisement_ID,
                            MenuID = model.MenuID
                        };
                    }
                    break;
            }
        }
        #endregion
    }
}