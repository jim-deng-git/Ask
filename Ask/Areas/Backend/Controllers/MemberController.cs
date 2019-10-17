using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Common;
using WorkLib;
using WorkV3.Golbal;
using WorkV3.Models;
using WorkV3.Models.DataAccess;
using WorkV3.Areas.Backend.Models.DataAccess;
using ICSharpCode.SharpZipLib.Zip;
using System.Data;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using WorkV3.Models.Interface;

namespace WorkV3.Areas.Backend.Controllers
{
    public class MemberController : Controller
    {
        private IEmailService emailService;

        string IdentityType = CategoryType.Identity.ToString();
        string FavorityType = CategoryType.Favority.ToString();
        string CareerType = CategoryType.Career.ToString();
        string EducationType = CategoryType.Education.ToString();
        string MarriageType = CategoryType.Marriage.ToString();
        string ChildrenStatusType = CategoryType.ChildrenStatus.ToString();
        string MilitaryServiceType = CategoryType.MilitaryService.ToString();

        public MemberController(IEmailService inEmailService)
        {
            emailService = inEmailService;
        }

        public ActionResult List(long siteId, long menuId, int? index, MemberShipSearchModels search)
        {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            if (Request.HttpMethod == "GET")
            {
                if (index == null)
                {
                    Utility.ClearSearchValue();
                }
                else
                {
                    MemberShipSearchModels prevSearch = Utility.GetSearchValue<MemberShipSearchModels>();
                    if (prevSearch != null)
                        search = prevSearch;
                }
            }
            else if (Request.HttpMethod == "POST")
            {
                Utility.SetSearchValue(search);
            }
            ViewBag.Search = search;

            ViewBag.IdentityCategories = Backend.Models.DataAccess.CategoryDAO.GetItems(IdentityType);
            ViewBag.FavorityCategories = Backend.Models.DataAccess.CategoryDAO.GetItems(FavorityType);

            WorkV3.Areas.Backend.Models.MemberShipRegSetModels regModel = LoadMemberShipSet(siteId);

            Pagination pagination = new Pagination
            {
                PageIndex = index ?? 1,
                PageSize = WebInfo.PageSize
            };

            int totalRecord;
            IEnumerable<MemberShipModels> items = MemberShipDAO.GetItems(search, pagination.PageSize, pagination.PageIndex, out totalRecord);
            pagination.TotalRecord = totalRecord;

            ViewBag.Pagination = pagination;
            ViewBag.FavorityType = FavorityType;
            ViewBag.IdentityType = IdentityType;
            return View(items);
        }

        [HttpPost]
        public void MemberShipDel(long siteId, long menuId, IEnumerable<long> ids)
        {
            MemberShipDAO.Delete(ids);
        }

        #region Edit
        [HttpGet]
        public ActionResult Edit(long siteId, long menuId, long? id)
        {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(siteId, menuId);
            ViewBag.UploadMemberPath = UpdFileInfo.GetVPathBySiteID(siteId, "Member");
            ViewBag.IdentityCategories = Backend.Models.DataAccess.CategoryDAO.GetItems(IdentityType);
            ViewBag.FavorityCategories = Backend.Models.DataAccess.CategoryDAO.GetItems(FavorityType);
            ViewBag.CareerCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(CareerType);
            ViewBag.MarriageCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(MarriageType);
            ViewBag.EducationCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(EducationType);
            ViewBag.ChildrenStatusCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(ChildrenStatusType);
            ViewBag.MilitaryServiceCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(MilitaryServiceType);

            WorkV3.Areas.Backend.Models.MemberShipRegSetModels regModel = LoadMemberShipSet(siteId);
            MemberShipModels item = null;
            if (id != null)
                item = MemberShipDAO.GetItem((long)id);

            if (item == null)
            {
                item = new MemberShipModels { ID = GetItem.NewSN(), CreateTime = DateTime.Now };
            }
            item.IdenityTypeList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems(item.ID, IdentityType);
            item.FavorityList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems(item.ID, FavorityType);

            if (!string.IsNullOrEmpty(item.DailyPhoto))
            {
                var DailyPhotoName = item.DailyPhoto.Split(',');
                string imgJson = "";
                for (int i = 0; i < DailyPhotoName.Length; i++)
                {
                    imgJson += JsonConvert.SerializeObject(new { ID = i, Img = DailyPhotoName[i] });
                    if (i != DailyPhotoName.Length - 1)
                        imgJson += ",";
                }

                ViewBag.Imgs = "[" + imgJson + "]";
            }

            return View(item);
        }

        private Areas.Backend.Models.MemberShipRegSetModels LoadMemberShipSet(long SiteID)
        {
            string loginText = "";
            Areas.Backend.Models.MemberShipRegSetModels MemberSetModel = Areas.Backend.Models.DataAccess.MemberShipRegSetDAO.GetItem(SiteID);
            if (MemberSetModel != null)
            {
                if (MemberSetModel.LoginTypeList.Count > 0)
                {
                    foreach (Areas.Backend.Models.MemberShipLoginType loginType in MemberSetModel.LoginTypeList)
                    {
                        switch (loginType)
                        {
                            case Areas.Backend.Models.MemberShipLoginType.Email:
                                loginText += "Email/";
                                break;
                            case Areas.Backend.Models.MemberShipLoginType.ID:
                                loginText += "身分證字號/";
                                break;
                            case Areas.Backend.Models.MemberShipLoginType.InputAccount:
                                loginText += "帳號/";
                                break;
                            case Areas.Backend.Models.MemberShipLoginType.Mobile:
                                loginText += "手機/";
                                break;
                        }
                    }
                    loginText = loginText.Trim('/');
                }
                ViewBag.MemberSetModel = MemberSetModel;
            }
            ViewBag.LoginPlaceHolder = loginText;

            return MemberSetModel;
        }
        [HttpPost]
        public ActionResult Edit(long siteId, long menuId, MemberShipModels item, bool VerifyStatus, string[] IdentityList, string[] FavorityList, string OrderEpaperTypeList, string[] Identity, int? IdentityMemberSession)
        {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            WorkV3.Areas.Backend.Models.MemberShipRegSetModels regModel = LoadMemberShipSet(siteId);
            ViewBag.IdentityCategories = Backend.Models.DataAccess.CategoryDAO.GetItems(IdentityType);
            ViewBag.FavorityCategories = Backend.Models.DataAccess.CategoryDAO.GetItems(FavorityType);
            ViewBag.CareerCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(CareerType);
            ViewBag.MarriageCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(MarriageType);
            ViewBag.EducationCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(EducationType);
            ViewBag.ChildrenStatusCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(ChildrenStatusType);
            ViewBag.MilitaryServiceCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(MilitaryServiceType);

            ViewBag.UploadMemberPath = UpdFileInfo.GetVPathBySiteID(siteId, "Member");

            item.SiteID = siteId;
            item.VerifyTime = "";
            item.LastResentVerifyMailTime = "";
            item.IdenityTypeList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems(item.ID, IdentityType);
            item.FavorityList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems(item.ID, FavorityType);
            item.CareerList = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(CareerType);
            item.MarriageList = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(MarriageType);
            item.EducationList = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(EducationType);


            if (item.MilitaryService == "505")
            {
                item.MilitaryService = "505," + item.MilitaryService_Y + ',' + item.MilitaryService_M;
            }

            if (!string.IsNullOrWhiteSpace(item.BankBook))
            {
                ResourceImagesModels imgModel = JsonConvert.DeserializeObject<ResourceImagesModels>(item.BankBook);
                if (imgModel.ID == 0)
                {
                    HttpPostedFileBase postedFile = Request.Files["fBankBook"];
                    if (postedFile == null || postedFile.ContentLength == 0)
                    {
                        item.BankBook = string.Empty;
                    }
                    else
                    {
                        item.BankBook = Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, siteId, "Member");
                    }
                }
                else
                {
                    item.BankBook = imgModel.Img;
                }
            }

            if (!string.IsNullOrWhiteSpace(item.IDcardPhoto_Front))
            {
                ResourceImagesModels imgModel = JsonConvert.DeserializeObject<ResourceImagesModels>(item.IDcardPhoto_Front);
                if (imgModel.ID == 0)
                {
                    HttpPostedFileBase postedFile = Request.Files["fIDcardPhoto_Front"];
                    if (postedFile == null || postedFile.ContentLength == 0)
                    {
                        item.IDcardPhoto_Front = string.Empty;
                    }
                    else
                    {
                        item.IDcardPhoto_Front = Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, siteId, "Member");
                    }
                }
                else
                {
                    item.IDcardPhoto_Front = imgModel.Img;
                }
            }

            if (!string.IsNullOrWhiteSpace(item.IDcardPhoto_Back))
            {
                ResourceImagesModels imgModel = JsonConvert.DeserializeObject<ResourceImagesModels>(item.IDcardPhoto_Back);
                if (imgModel.ID == 0)
                {
                    HttpPostedFileBase postedFile = Request.Files["fIDcardPhoto_Back"];
                    if (postedFile == null || postedFile.ContentLength == 0)
                    {
                        item.IDcardPhoto_Back = string.Empty;
                    }
                    else
                    {
                        item.IDcardPhoto_Back = Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, siteId, "Member");
                    }
                }
                else
                {
                    item.IDcardPhoto_Back = imgModel.Img;
                }
            }
            if (item.IDcardPhoto_Front == null && item.IDcardPhoto_Back == null)
            {
                item.IDcardPhoto = null;
            }
            else
            {
                item.IDcardPhoto = item.IDcardPhoto_Front + ',' + item.IDcardPhoto_Back;
            }

            if (VerifyStatus)//驗證
            {
                if (regModel.VerifyType == Models.MemberShipVerifyType.Email)
                {
                    if (string.IsNullOrWhiteSpace(item.VerifyTime))
                    {
                        item.VerifyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        if (item.Status == true)
                            item.Status = true;
                    }
                }

                else if (regModel.VerifyType == Models.MemberShipVerifyType.Mobile)
                {
                    item.VerifyTime = "";
                    item.MobileVerifyCode = "000000";
                    if (item.Status == true)
                        item.Status = true;
                }
            }
            else
            {
                item.VerifyTime = "";
                item.MobileVerifyCode = "NULL";
                if (item.Status == true)
                    item.Status = true;
                else
                    item.Status = false;
            }

            if (IdentityMemberSession == (int)MemberSession.不限制 || IdentityMemberSession == null)
            {
                item.ExpireDate = null;
            }
            if (item.sameAddress)
            {
                item.PermanentAddress = item.Address;
                item.PermanentRegions = item.Regions;
            }

            MemberShipDAO.SetItem(item, true);
            //if (IdentityList != null)
            //{
            //    WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.SetSelectedItems(item.ID, IdentityType, IdentityList.ToList());
            //}
            if (Identity != null)
            {
                WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.SetSelectedItems(item.ID, IdentityType, Identity.ToList());
            }
            if (FavorityList != null)
            {
                WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.SetSelectedItems(item.ID, FavorityType, FavorityList.ToList());
            }

            if (item.DailyPhoto != null)
            {
                var DailyPhotoName = item.DailyPhoto.Split(',');
                string imgJson = "";
                for (int i = 0; i < DailyPhotoName.Length; i++)
                {
                    imgJson += JsonConvert.SerializeObject(new { ID = i, Img = DailyPhotoName[i] });
                    if (i != DailyPhotoName.Length - 1)
                        imgJson += ",";
                }

                ViewBag.Imgs = "[" + imgJson + "]";
            }

            ViewBag.Exit = true;
            return View(item);
        }
        #endregion

        /// <summary>
        /// 取得會員到期日
        /// </summary>
        /// <param name="memberSession"></param>
        /// <returns></returns>
        public string GetExpireDate(int memberSession)
        {
            string result = "";
            DateTime now = DateTime.Now;
            switch (memberSession)
            {
                case (int)MemberSession.一日期:
                    now = now.AddDays(1);
                    break;
                case (int)MemberSession.一週期:
                    now = now.AddDays(7);
                    break;
                case (int)MemberSession.雙週期:
                    now = now.AddDays(14);
                    break;
                case (int)MemberSession.一月期:
                    now = now.AddMonths(1);
                    break;
                case (int)MemberSession.雙月期:
                    now = now.AddMonths(2);
                    break;
                case (int)MemberSession.一季期:
                    now = now.AddMonths(3);
                    break;
                case (int)MemberSession.四月期:
                    now = now.AddMonths(4);
                    break;
                case (int)MemberSession.半年期:
                    now = now.AddMonths(6);
                    break;
                case (int)MemberSession.一年期:
                    now = now.AddYears(1);
                    break;
                default:
                    break;
            }
            result = now.ToString(WebInfo.DateFmt);
            return result;
        }

        [HttpGet]
        public int CheckAccount(long siteId, string account, long? id)
        {
            return MemberShipDAO.CheckAccount(siteId, account, id) ? 1 : 0;
        }

        [HttpGet]
        public JsonResult CheckPushRecommandCode(long siteId, string PushRecommandCode, string LoginType, long? id)
        {
            MemberShipModels PushRecommandData = new MemberShipModels();

            if (!string.IsNullOrWhiteSpace(LoginType))
            {
                if (LoginType.IndexOf("帳號") >= 0)
                {
                    PushRecommandData = MemberShipDAO.CheckPushRecommandCode(siteId, "Account", PushRecommandCode);
                    if (PushRecommandData != null) return Json(new { CheckOK = true, Info = PushRecommandData }, JsonRequestBehavior.AllowGet);
                }
                if (LoginType.IndexOf("Email") >= 0)
                {
                    PushRecommandData = MemberShipDAO.CheckPushRecommandCode(siteId, "Email", PushRecommandCode);
                    if (PushRecommandData != null) return Json(new { CheckOK = true, Info = PushRecommandData }, JsonRequestBehavior.AllowGet);
                }
                if (LoginType.IndexOf("身分證字號") >= 0)
                {
                    PushRecommandData = MemberShipDAO.CheckPushRecommandCode(siteId, "IDCard", PushRecommandCode);
                    if (PushRecommandData != null) return Json(new { CheckOK = true, Info = PushRecommandData }, JsonRequestBehavior.AllowGet);
                }
                if (LoginType.IndexOf("手機") >= 0)
                {
                    PushRecommandData = MemberShipDAO.CheckPushRecommandCode(siteId, "Mobile", PushRecommandCode);
                    if (PushRecommandData != null) return Json(new { CheckOK = true, Info = PushRecommandData }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { CheckOK = false, Info = PushRecommandData }, JsonRequestBehavior.AllowGet);
            //return null;
        }

        public void ExportToXLS(long siteId, bool? IsPrivate)
        {
            MemberShipSearchModels search = new MemberShipSearchModels();
            if (Request.HttpMethod == "GET")
            {

                Utility.ClearSearchValue();
                //}
                //else
                //{
                //    MemberShipSearchModels prevSearch = Utility.GetSearchValue<MemberShipSearchModels>();
                //    if (prevSearch != null)
                //        search = prevSearch;
                //}
            }
            else if (Request.HttpMethod == "POST")
            {
                Utility.SetSearchValue(search);
            }
            WorkV3.Areas.Backend.Models.MemberShipRegSetModels regModel = LoadMemberShipSet(siteId);

            DataTable queryTable = MemberShipDAO.GetExportItems(search);
            DataTable memberTable = new DataTable();
            memberTable.Columns.Add(new DataColumn("序", typeof(string)));
            foreach (WorkV3.Areas.Backend.Models.MemberShipRegColumnSetModels colModel in regModel.RegColumnSets)
            {
                if (colModel.IsOpen && colModel.ColumnName != "Photo" && colModel.ColumnName != "OrderEpaper")
                {
                    string colName = colModel.ColumnTitle;
                    //if (colModel.ColumnName == "Career" ||
                    //    colModel.ColumnName == "Identity" ||
                    //    colModel.ColumnName == "Favority" ||
                    //    colModel.ColumnName == "Marriage" ||
                    //    colModel.ColumnName == "Education")
                    //{
                    //    if (colModel.ColumnName == "Career")
                    //    {
                    //        colName += "(單選：";
                    //        List<WorkV3.Areas.Backend.Models.CategoryModels> cateList = Backend.Models.DataAccess.CategoryDAO.GetItems(CareerType);
                    //        foreach(WorkV3.Areas.Backend.Models.CategoryModels cate in cateList)
                    //        {
                    //            if(cate.ShowStatus)
                    //            colName += cate.Title + "/";
                    //        }
                    //        colName = colName.Trim('/');
                    //        colName += ")";
                    //    }
                    //    if (colModel.ColumnName == "Marriage")
                    //    {
                    //        colName += "(單選：";
                    //        List<WorkV3.Areas.Backend.Models.CategoryModels> cateList = Backend.Models.DataAccess.CategoryDAO.GetItems(MarriageType);
                    //        foreach (WorkV3.Areas.Backend.Models.CategoryModels cate in cateList)
                    //        {
                    //            if (cate.ShowStatus)
                    //                colName += cate.Title + "/";
                    //        }
                    //        colName = colName.Trim('/');
                    //        colName += ")";
                    //    }
                    //    if (colModel.ColumnName == "Education")
                    //    {
                    //        colName += "(單選：";
                    //        List<WorkV3.Areas.Backend.Models.CategoryModels> cateList = Backend.Models.DataAccess.CategoryDAO.GetItems(EducationType);
                    //        foreach (WorkV3.Areas.Backend.Models.CategoryModels cate in cateList)
                    //        {
                    //            if (cate.ShowStatus)
                    //                colName += cate.Title + "/";
                    //        }
                    //        colName = colName.Trim('/');
                    //        colName += ")";
                    //    }
                    //    if (colModel.ColumnName == "Identity")
                    //    {
                    //        colName += "(複選：";
                    //        List<WorkV3.Areas.Backend.Models.CategoryModels> cateList = Backend.Models.DataAccess.CategoryDAO.GetItems(IdentityType);
                    //        foreach (WorkV3.Areas.Backend.Models.CategoryModels cate in cateList)
                    //        {
                    //            if (cate.ShowStatus)
                    //                colName += cate.Title + "/";
                    //        }
                    //        colName = colName.Trim('/');
                    //        colName += ")";
                    //    }
                    //    if (colModel.ColumnName == "Favority")
                    //    {
                    //        colName += "(複選：";
                    //        List<WorkV3.Areas.Backend.Models.CategoryModels> cateList = Backend.Models.DataAccess.CategoryDAO.GetItems(FavorityType);
                    //        foreach (WorkV3.Areas.Backend.Models.CategoryModels cate in cateList)
                    //        {
                    //            if (cate.ShowStatus)
                    //                colName += cate.Title + "/";
                    //        }
                    //        colName = colName.Trim('/');
                    //        colName += ")";
                    //    }
                    //}
                    //else if (colModel.ColumnName == "Sex")
                    //{
                    //    colName += "(單選：男/女)";
                    //}
                    memberTable.Columns.Add(new DataColumn(colName, typeof(string)));
                }
            }
            memberTable.Columns.Add(new DataColumn("會員狀態", typeof(string)));
            memberTable.Columns.Add(new DataColumn("推薦人代碼", typeof(string)));
            memberTable.Columns.Add(new DataColumn("邀請碼", typeof(string)));
            memberTable.Columns.Add(new DataColumn("類型", typeof(string)));

            int rowIndex = 0;
            foreach (DataRow row in queryTable.Rows)
            {
                rowIndex++;
                DataRow newRow = memberTable.NewRow();
                newRow["序"] = rowIndex.ToString();
                foreach (WorkV3.Areas.Backend.Models.MemberShipRegColumnSetModels colModel in regModel.RegColumnSets)
                {
                    if (colModel.IsOpen && colModel.ColumnName != "Photo" && colModel.ColumnName != "OrderEpaper")
                    {
                        if (queryTable.Columns.Contains(colModel.ColumnName))
                        {
                            newRow[colModel.ColumnTitle] = row[colModel.ColumnName].ToString();
                            if (colModel.ColumnName == "Birthday")
                            {
                                if (!string.IsNullOrEmpty(newRow[colModel.ColumnTitle].ToString()))
                                    newRow[colModel.ColumnTitle] = DateTime.Parse(newRow[colModel.ColumnTitle].ToString()).ToString("yyyy/MM/dd");
                            }
                            if (IsPrivate.HasValue && IsPrivate.Value)
                            {
                                if (colModel.ColumnName == "Name" || colModel.ColumnName == "Email" || colModel.ColumnName == "IDCard"
                                    || colModel.ColumnName == "Mobile" || colModel.ColumnName == "Phone")
                                {
                                    if (!string.IsNullOrEmpty(newRow[colModel.ColumnTitle].ToString()))
                                        newRow[colModel.ColumnTitle] = PrivateData(newRow[colModel.ColumnTitle].ToString());
                                }
                            }
                        }
                        else
                        {

                            if (colModel.ColumnName == "Career")
                            {
                                string colValue = "";
                                var SelectedList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems((long)row["ID"], CareerType);
                                if (SelectedList != null && SelectedList.Count() > 0)
                                {
                                    foreach (Models.CategoryModels selCate in SelectedList)
                                    {
                                        colValue += selCate.Title + "/";
                                    }
                                }
                                colValue = colValue.Trim('/');
                                newRow[colModel.ColumnTitle] = colValue;
                            }

                            if (colModel.ColumnName == "Identity")
                            {
                                string colValue = "";
                                var SelectedList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems((long)row["ID"], IdentityType);
                                if (SelectedList != null && SelectedList.Count() > 0)
                                {
                                    foreach (Models.CategoryModels selCate in SelectedList)
                                    {
                                        colValue += selCate.Title + "/";
                                    }
                                }
                                colValue = colValue.Trim('/');
                                newRow[colModel.ColumnTitle] = colValue;
                            }

                            if (colModel.ColumnName == "Favority")
                            {
                                string colValue = "";
                                var SelectedList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems((long)row["ID"], FavorityType);
                                if (SelectedList != null && SelectedList.Count() > 0)
                                {
                                    foreach (Models.CategoryModels selCate in SelectedList)
                                    {
                                        colValue += selCate.Title + "/";
                                    }
                                }
                                colValue = colValue.Trim('/');
                                newRow[colModel.ColumnTitle] = colValue;
                            }

                            if (colModel.ColumnName == "Marriage")
                            {
                                string colValue = "";
                                var SelectedList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems((long)row["ID"], MarriageType);
                                if (SelectedList != null && SelectedList.Count() > 0)
                                {
                                    foreach (Models.CategoryModels selCate in SelectedList)
                                    {
                                        colValue += selCate.Title + "/";
                                    }
                                }
                                colValue = colValue.Trim('/');
                                newRow[colModel.ColumnTitle] = colValue;
                            }
                            if (colModel.ColumnName == "Education")
                            {
                                string colValue = "";
                                var SelectedList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems((long)row["ID"], EducationType);
                                if (SelectedList != null && SelectedList.Count() > 0)
                                {
                                    foreach (Models.CategoryModels selCate in SelectedList)
                                    {
                                        colValue += selCate.Title + "/";
                                    }
                                }
                                colValue = colValue.Trim('/');
                                newRow[colModel.ColumnTitle] = colValue;
                            }
                        }
                    }
                }
                newRow["會員狀態"] = !string.IsNullOrEmpty(row["Status"].ToString()) ? (string.IsNullOrEmpty(row["VerifyTime"].ToString()) ? "未驗證" : "正常") : "停權";
                newRow["推薦人代碼"] = row["PushRecommandCode"].ToString();
                newRow["邀請碼"] = row["RecommandCode"].ToString();
                newRow["類型"] = GetMemberTypeName((WorkV3.Models.MemberType)int.Parse(row["MemberType"].ToString()));
                memberTable.Rows.Add(newRow);
                //}
            }

            WorkV3.Models.SitesModels siteModel = WorkV3.Models.DataAccess.SitesDAO.GetInfo(siteId);
            if (siteModel.SN.Contains("華山") ||
                siteModel.SN.Contains("huashan"))
            {
                memberTable.Columns.Remove("邀請碼");
                memberTable.Columns.Remove("推薦人代碼");
            }
            WorkLib.uXls.DTExpExcel(memberTable, string.Format("{0}", "會員資料"), "會員資料");
        }

        public string GetMemberTypeName(WorkV3.Models.MemberType memberType)
        {
            switch (memberType)
            {
                case WorkV3.Models.MemberType.FB:
                    return "FB";
                case WorkV3.Models.MemberType.Web:
                    return "網站";
                default:
                    return "undefined";
            }
        }

        private string PrivateData(string value)
        {
            string privateData = "";
            if (value.Length > 0)
            {
                int totalLength = value.Length;
                if (totalLength > 0)
                    privateData = value.Substring(0, 1);

                if (totalLength > 2)
                {
                    privateData += "".PadLeft(totalLength - 2, '*');
                    privateData += value.Substring(value.Length - 1);
                }
                else
                {
                    privateData += "*";
                }
            }
            return privateData;
        }
        #region 匯入功能
        [HttpGet]
        public ActionResult Import(long siteId, long menuId)
        {
            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/') + "/";

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            return View();
        }

        [HttpPost]
        public ActionResult Import(long siteId, long menuId, HttpPostedFileBase fFile)
        {
            string uploadUrl = Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/') + "/";
            string viewUrl = Golbal.UpdFileInfo.GetUPathByMenuID(siteId, menuId).TrimEnd('/') + "/";

            string fileName = "";
            if (fFile?.ContentLength > 0)
            {
                fileName = Golbal.UpdFileInfo.SaveFilesByMenuID(fFile, siteId, menuId);
            }
            IEnumerable<WorkV3.Areas.Backend.Models.MemberShipRegColumnSetModels> fields = WorkV3.Areas.Backend.Models.DataAccess.MemberShipRegSetDAO.GetColumnItems(siteId);

            string message = "";
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.UploadUrl = uploadUrl;
            ViewBag.Message = message;
            return View();
        }
        private string needValueSymble = "(*)";
        public FileResult ImportTmpl(long siteId, long menuId)
        {
            IEnumerable<WorkV3.Areas.Backend.Models.MemberShipRegColumnSetModels> fields = WorkV3.Areas.Backend.Models.DataAccess.MemberShipRegSetDAO.GetColumnItems(siteId);
            var ListIdentityCategories = Backend.Models.DataAccess.CategoryDAO.GetItems(IdentityType);
            var ListFavorityCategories = Backend.Models.DataAccess.CategoryDAO.GetItems(FavorityType);
            var ListCareerCategories = Backend.Models.DataAccess.CategoryDAO.GetItems(CareerType);
            var ListMarriageCategories = Backend.Models.DataAccess.CategoryDAO.GetItems(MarriageType);
            var ListEducationCategories = Backend.Models.DataAccess.CategoryDAO.GetItems(EducationType);
            ViewData["MemberFields"] = fields;
            ViewData["IdentityCategories"] = ListIdentityCategories;
            ViewData["FavorityCategories"] = ListFavorityCategories;
            ViewData["CareerCategories"] = ListCareerCategories;
            ViewData["MarriageCategories"] = ListMarriageCategories;
            ViewData["EducationCategories"] = ListEducationCategories;

            List<string> columnList = new List<string>();
            List<string> columnSampleRowList = new List<string>();
            // string html = Utility.GetViewHtml(this, "ImportTmpl", null);
            foreach (WorkV3.Areas.Backend.Models.MemberShipRegColumnSetModels item in fields)
            {
                string columnTitle = "", sampleData = "";
                if (item.IsOpen && item.ColumnName != "Photo")
                {
                    columnTitle += item.ColumnTitle;
                    if (item.ColumnName == "Career")
                    {
                        string listItems = "";
                        foreach (WorkV3.Areas.Backend.Models.CategoryModels cate in ListCareerCategories)
                        {
                            if (cate.ShowStatus)
                            {
                                listItems += cate.Title + ";";
                                if (string.IsNullOrEmpty(sampleData))
                                    sampleData = cate.Title;
                            }
                        }
                        columnTitle += string.Format("：單選({0})", listItems);
                    }
                    else if (item.ColumnName == "Birthday")
                    {
                        columnTitle += "：2001/03/09";
                        sampleData = "2001/03/09";
                    }
                    else if (item.ColumnName == "Identity")
                    {
                        string listItems = "";
                        foreach (WorkV3.Areas.Backend.Models.CategoryModels cate in ListIdentityCategories)
                        {
                            if (cate.ShowStatus)
                            {
                                listItems += cate.Title + ";";
                            }
                        }
                        columnTitle += string.Format("：複選({0})", listItems);
                        sampleData = listItems;
                    }
                    else if (item.ColumnName == "Sex")
                    {
                        columnTitle += "：單選(男;女)";
                        sampleData = "男";
                    }
                    else if (item.ColumnName == "Favority")
                    {
                        string listItems = "";
                        foreach (WorkV3.Areas.Backend.Models.CategoryModels cate in ListFavorityCategories)
                        {
                            if (cate.ShowStatus)
                            {
                                listItems += cate.Title + ";";
                            }
                        }
                        columnTitle += string.Format("：複選({0})", listItems);
                        sampleData = listItems;
                    }
                    else if (item.ColumnName == "Marriage")
                    {
                        string listItems = "";
                        foreach (WorkV3.Areas.Backend.Models.CategoryModels cate in ListMarriageCategories)
                        {
                            if (cate.ShowStatus)
                            {
                                listItems += cate.Title + ";";
                                if (string.IsNullOrEmpty(sampleData))
                                    sampleData = cate.Title;
                            }
                        }
                        columnTitle += string.Format("：單選({0})", listItems);
                    }
                    else if (item.ColumnName == "Education")
                    {
                        string listItems = "";
                        foreach (WorkV3.Areas.Backend.Models.CategoryModels cate in ListEducationCategories)
                        {
                            if (cate.ShowStatus)
                            {
                                listItems += cate.Title + ";";
                                if (string.IsNullOrEmpty(sampleData))
                                    sampleData = cate.Title;
                            }
                        }
                        columnTitle += string.Format("：單選({0})", listItems);
                    }
                    else if (item.ColumnName == "Address")
                    {
                        List<Models.MemberShipRegOptionModels> optionList = null;

                        if (!string.IsNullOrEmpty(item.OtherOption.Trim()))
                            optionList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.MemberShipRegOptionModels>>(item.OtherOption.Trim());
                        var addressOption = optionList.Where(o => o.Selected);
                        if (addressOption.First().Value == "Global")
                        {
                            columnList.Add("國家" + (item.IsNeedValue ? needValueSymble : ""));
                            columnSampleRowList.Add("台灣");
                            columnList.Add("省/州" + (item.IsNeedValue ? needValueSymble : ""));
                            columnSampleRowList.Add("");
                            columnList.Add("縣/市" + (item.IsNeedValue ? needValueSymble : ""));
                            columnSampleRowList.Add("台北市");
                            columnList.Add("行政區" + (item.IsNeedValue ? needValueSymble : ""));
                            columnSampleRowList.Add("信義區");
                        }
                        else
                        {
                            columnList.Add("縣/市" + (item.IsNeedValue ? needValueSymble : ""));
                            columnSampleRowList.Add("台北市");
                            columnList.Add("行政區" + (item.IsNeedValue ? needValueSymble : ""));
                            columnSampleRowList.Add("信義區");
                        }
                        sampleData = "市府路1號";
                    }
                    else
                    {
                        switch (item.ColumnName)
                        {
                            case "Company":
                                sampleData = "XX公司";
                                break;
                            case "Email":
                            case "EmergencyEmail":
                                sampleData = "ABC@kidtech.com";
                                break;
                            case "Mobile":
                            case "EmergencyMobile":
                                sampleData = "0911123123";
                                break;
                            case "Name":
                            case "EmergencyName":
                                sampleData = "王大明";
                                break;
                            case "Phone":
                            case "EmergencyPhone":
                                sampleData = "021234567";
                                break;
                            case "IDCard":
                                sampleData = "A123456789";
                                break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(columnTitle))
                {
                    if (item.IsNeedValue)
                    {
                        columnTitle += needValueSymble;
                    }
                    columnList.Add(columnTitle);
                    columnSampleRowList.Add(sampleData.TrimEnd(';'));
                }
            }

            string title = "會員名單-匯入名單範本";
            //title = $"{ title }.xlsx";

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            byte[] fileContent = CreateNPOIFile(title, columnList, columnSampleRowList);
            return File(fileContent, "application/vnd.ms-excel", $"{ title }.xls");
            //return File(System.Text.Encoding.UTF8.GetBytes(html), "application/vnd.ms-excel", title);
        }
        private byte[] CreateNPOIFile(string ReportTitle, List<string> columnList, List<string> columnSampleRowList)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();

            DocumentSummaryInformation documentSummary = PropertySetFactory.CreateDocumentSummaryInformation();
            documentSummary.Company = "KSR";
            workbook.DocumentSummaryInformation = documentSummary;

            SummaryInformation summaryInfo = PropertySetFactory.CreateSummaryInformation();
            summaryInfo.Subject = ReportTitle;
            summaryInfo.Title = ReportTitle;
            summaryInfo.Author = "KSR";
            workbook.SummaryInformation = summaryInfo;


            NPOI.SS.UserModel.IFont font16 = workbook.CreateFont();
            font16.FontHeightInPoints = 16;
            NPOI.SS.UserModel.IFont font12 = workbook.CreateFont();
            font12.FontHeightInPoints = 12;
            NPOI.SS.UserModel.IFont font10 = workbook.CreateFont();
            font10.FontHeightInPoints = 10;

            //HSSFCellStyle titleStyle = (workbook.CreateCellStyle() as HSSFCellStyle);
            //titleStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            //titleStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
            //titleStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            //titleStyle.SetFont(font16);

            HSSFCellStyle sheetDefaultStyle = (workbook.CreateCellStyle() as HSSFCellStyle);
            var format = workbook.CreateDataFormat();
            sheetDefaultStyle.DataFormat = format.GetFormat("text");

            HSSFCellStyle rowStyleHead = (workbook.CreateCellStyle() as HSSFCellStyle);
            rowStyleHead.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            rowStyleHead.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            rowStyleHead.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            rowStyleHead.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            rowStyleHead.WrapText = true;
            rowStyleHead.ShrinkToFit = true;
            rowStyleHead.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            rowStyleHead.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            rowStyleHead.SetFont(font12);

            HSSFCellStyle rowStyleData = (workbook.CreateCellStyle() as HSSFCellStyle);
            rowStyleData.CloneStyleFrom(rowStyleHead);
            rowStyleData.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
            rowStyleData.SetFont(font10);

            int rowIndex = 0;

            HSSFSheet worksheet = (workbook.CreateSheet(ReportTitle.Replace("/", ".")) as HSSFSheet);
            worksheet.SetMargin(NPOI.SS.UserModel.MarginType.TopMargin, 0.5);
            worksheet.SetMargin(NPOI.SS.UserModel.MarginType.LeftMargin, 0.5);
            worksheet.SetMargin(NPOI.SS.UserModel.MarginType.RightMargin, 0.5);
            worksheet.SetMargin(NPOI.SS.UserModel.MarginType.BottomMargin, 0.5);
            worksheet.FitToPage = false;
            worksheet.SetZoom(1, 1);


            HSSFFooter sheetfooter = (HSSFFooter)worksheet.Footer;
            sheetfooter.Center = string.Format("{0}/{1}", HSSFFooter.Page, HSSFFooter.NumPages);
            //HSSFRow titleexcelrow = (worksheet.CreateRow(rowIndex) as HSSFRow);
            //titleexcelrow.CreateCell(0, NPOI.SS.UserModel.CellType.String).SetCellValue(ReportTitle);
            //titleexcelrow.Cells[0].CellStyle = titleStyle;
            //titleexcelrow.Height = 3 * 256;
            //rowIndex++;

            HSSFRow headexcelrow = (worksheet.CreateRow(rowIndex) as HSSFRow);
            int cellIndex = 0;
            headexcelrow.CreateCell(cellIndex, NPOI.SS.UserModel.CellType.String).SetCellValue("帳號" + needValueSymble);
            headexcelrow.Height = 4 * 256;
            headexcelrow.Cells[cellIndex].CellStyle = rowStyleHead;
            cellIndex++;
            worksheet.SetColumnWidth(cellIndex, 20 * 256);
            worksheet.SetDefaultColumnStyle(cellIndex, sheetDefaultStyle);

            headexcelrow.CreateCell(cellIndex, NPOI.SS.UserModel.CellType.String).SetCellValue("密碼" + needValueSymble);
            headexcelrow.Height = 4 * 256;
            headexcelrow.Cells[cellIndex].CellStyle = rowStyleHead;
            cellIndex++;
            worksheet.SetColumnWidth(cellIndex, 20 * 256);
            worksheet.SetDefaultColumnStyle(cellIndex, sheetDefaultStyle);
            foreach (string key in columnList)
            {
                headexcelrow.CreateCell(cellIndex, NPOI.SS.UserModel.CellType.String).SetCellValue(key);
                headexcelrow.Height = 4 * 256;
                headexcelrow.Cells[cellIndex].CellStyle = rowStyleHead;
                cellIndex++;
                worksheet.SetColumnWidth(cellIndex, 20 * 256);
                worksheet.SetDefaultColumnStyle(cellIndex, sheetDefaultStyle);
            }

            rowIndex++;
            HSSFRow dataexcelrow = (worksheet.CreateRow(rowIndex) as HSSFRow);
            cellIndex = 0;
            dataexcelrow.CreateCell(cellIndex, NPOI.SS.UserModel.CellType.String).SetCellValue("act01");
            dataexcelrow.Height = 2 * 256;
            dataexcelrow.Cells[cellIndex].CellStyle = rowStyleData;
            cellIndex++;

            dataexcelrow.CreateCell(cellIndex, NPOI.SS.UserModel.CellType.String).SetCellValue("012345");
            dataexcelrow.Height = 2 * 256;
            dataexcelrow.Cells[cellIndex].CellStyle = rowStyleData;
            cellIndex++;
            foreach (string columnSampleRow in columnSampleRowList)
            {
                dataexcelrow.CreateCell(cellIndex, NPOI.SS.UserModel.CellType.String).SetCellValue(columnSampleRow);
                dataexcelrow.Height = 2 * 256;
                dataexcelrow.Cells[cellIndex].CellStyle = rowStyleData;
                cellIndex++;
            }
            System.IO.MemoryStream excelStream = new System.IO.MemoryStream();
            workbook.Write(excelStream);
            return excelStream.ToArray();
        }
        private string CheckZoneAddressFullInfo(string OtherOption, string country, string state, string city, string district)
        {
            List<Models.MemberShipRegOptionModels> optionList = null;

            if (!string.IsNullOrEmpty(OtherOption))
                optionList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.MemberShipRegOptionModels>>(OtherOption.Trim());
            else
                return "";
            List<int> regions = new List<int>();
            var addressOption = optionList.Where(o => o.Selected);
            if (addressOption.First().Value == "Global")
            {
                WorldRegionModels countryModel = GetRegionModel(country);
                if (countryModel == null)
                    return "";
                regions.Add(countryModel.ID);
                WorldRegionModels lvModel1 = GetRegionModel(countryModel.ID, state);
                if (lvModel1 != null)
                {
                    regions.Add(lvModel1.ID);
                    WorldRegionModels lvModel2 = GetRegionModel(lvModel1.ID, city);
                    if (lvModel2 != null)
                    {
                        regions.Add(lvModel2.ID);
                        WorldRegionModels lvModel3 = GetRegionModel(lvModel2.ID, district);
                        if (lvModel3 != null)
                        {
                            regions.Add(lvModel3.ID);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(district))
                                return "";
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(city))
                            return "";
                        WorldRegionModels lvModel3 = GetRegionModel(lvModel1.ID, district);
                        if (lvModel3 != null)
                        {
                            regions.Add(lvModel3.ID);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(district))
                                return "";
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(state))
                        return "";
                    lvModel1 = GetRegionModel(countryModel.ID, city);
                    if (lvModel1 != null)
                    {
                        regions.Add(lvModel1.ID);
                        WorldRegionModels lvModel2 = GetRegionModel(lvModel1.ID, district);
                        if (lvModel2 != null)
                        {
                            regions.Add(lvModel2.ID);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(district))
                                return "";
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(city))
                            return "";
                    }
                }
            }
            else
            {
                WorldRegionModels countryModel = GetRegionModel("台灣");
                if (countryModel == null)
                    return "";
                WorldRegionModels cityModel = GetRegionModel(countryModel.ID, city);
                if (cityModel == null)
                    return "";
                var districtModel = GetRegionModel(cityModel.ID, district);
                if (districtModel == null)
                    return "";
                regions.Add(countryModel.ID);
                regions.Add(cityModel.ID);
                regions.Add(districtModel.ID);
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(regions);
        }
        private WorldRegionModels GetRegionModel(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            WorldRegionModels searchModel = WorldRegionDAO.GetRegionByName(name);
            return searchModel;
        }
        private WorldRegionModels GetRegionModel(int parentID, string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            var childModels = WorldRegionDAO.GetRegionsByParentId(parentID);
            if (childModels == null || childModels.Count() <= 0) // 無子項
                return null;
            var searchModel = childModels.Where(p => p.Name == name);
            if (searchModel == null || searchModel.Count() <= 0)
                return null;
            return searchModel.First();
        }
        #endregion
        #region 寄信功能
        [HttpGet]
        public ActionResult MemberSendMail(long siteId, long menuId)
        {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.MailID = WorkLib.GetItem.NewSN();
            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/') + "/";
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public string MemberSendMail(long siteId, long menuId, long mailId, MemberShipMailModel mail, string[] fileList, string recipientIds, string recipientEmails, bool isTest)
        {
            if (mail == null || string.IsNullOrWhiteSpace(mail.MailBody))
                return "Mail 為 NULL";

            if (isTest && string.IsNullOrWhiteSpace(recipientEmails))
                return "測試郵件缺少收信人";

            if (!isTest && string.IsNullOrWhiteSpace(recipientIds))
                return "未選擇郵件發送用戶";

            SitesModels site = SitesDAO.GetInfo(siteId);

            System.Collections.ArrayList emailFiles = new System.Collections.ArrayList();
            List<ResourceFilesModels> files = new List<ResourceFilesModels>();
            if (fileList?.Length > 0)
            {
                string uploadPath = Golbal.UpdFileInfo.GetUPathByMenuID(siteId, menuId).TrimEnd('\\') + "\\";
                foreach (string item in fileList)
                {
                    ResourceFilesModels file = JsonConvert.DeserializeObject<ResourceFilesModels>(item);
                    files.Add(file);
                    emailFiles.Add(uploadPath + file.FileInfo);
                }
            }
            mail.Files = JsonConvert.SerializeObject(files);


            string mailSubject = mail.MailSubject.Replace("[WebsiteName]", site.Title).Replace("[SendDate]", DateTime.Now.ToString(WebInfo.DateFmt));
            string mailBody = mail.MailBody.Replace("[WebsiteName]", site.Title).Replace("[SendDate]", DateTime.Now.ToString(WebInfo.DateFmt));


            if (isTest)
            {
                SendMailTest(mail, mailSubject, mailBody, emailFiles, recipientEmails, siteId);
                return "測試郵件發送成功";
            }
            mailBody += "<img style='display:none' alt='' src='[GetEmail]'/>";
            mail.ID = mailId;//WorkLib.GetItem.NewSN();
            MemberShipMailDAO.SetItem(mail);
            MailSend(mail, mailSubject, mailBody, emailFiles, recipientIds, siteId);
            return "郵件發送成功";
        }
        [HttpGet]
        public ActionResult TestEmail()
        {
            return View();
        }
        private void SendMailTest(WorkV3.Models.MemberShipMailModel mail, string mailSubject, string mailBody, System.Collections.ArrayList fileList, string recipientEmails, long siteId)
        {
            string SitesTitle = SitesDAO.GetInfo(siteId).Title;//網站名稱
            string[] emails = recipientEmails.Split(';');
            foreach (string email in emails)
            {
                emailService.SendMailWithFiles(siteId, email, email, mailSubject, mailBody, fileList, mail.SenderEmail, SitesTitle);
            }
        }
        private void MailSend(WorkV3.Models.MemberShipMailModel mail, string mailSubject, string mailBody, System.Collections.ArrayList fileList, string recipientIds, long siteId)
        {
            //IEnumerable<long> recipientIdList = recipientIds.Split(',').Select(id => long.Parse(id));
            //IEnumerable<WorkV3.Models.MemberShipModels> participants = MemberShipDAO.GetInfoItems(recipientIdList);
            string rootUrl = Utility.GetRootUrl() + "/";
            string SitesTitle = SitesDAO.GetInfo(siteId).Title;//網站名稱
            foreach (string memberID in recipientIds.Split(','))
            {
                WorkV3.Models.MemberShipModels member = MemberShipDAO.GetItem(long.Parse(memberID));

                if (member == null)
                    continue;

                string receiveUrl = $"{ rootUrl }Member/MailRead/{ mail.ID }?itemId={ member.ID }";
                Dictionary<string, string> replaceFields = new Dictionary<string, string>();
                replaceFields.Add("[MemberName]", member.Name);
                replaceFields.Add("[MemberID]", member.Account);
                //replaceFields.Add("[WebsiteName]", DateTime.Now.ToString("yyyy/MM/dd")); // 在傳進來的己先取代, 
                //replaceFields.Add("[SendDate]", DateTime.Now.ToString("yyyy/MM/dd"));  // 在傳進來的己先取代, 
                replaceFields.Add("[RegStatus]", member.Status ? (string.IsNullOrEmpty(member.VerifyTime) ? "未驗證" : "正常") : "停權");
                replaceFields.Add("[GetEmail]", receiveUrl);
                string email, mailToName, subject, body;

                email = member.Email;
                mailToName = member.Name;
                body = mailBody;
                subject = mailSubject;
                foreach (string key in replaceFields.Keys)
                {
                    body = body.Replace(key, replaceFields[key]);
                    subject = subject.Replace(key, replaceFields[key]);
                }

                emailService.SendMailWithFiles(siteId, email, mailToName, subject, body, fileList, mail.SenderEmail, SitesTitle);
                MemberShipMailDAO.WriteLog(mail.ID, member.ID);
            }
        }

        [HttpGet]
        public ActionResult MailLog(long siteId)
        {
            ViewBag.SiteID = siteId;
            IEnumerable<WorkV3.Models.MemberShipMailModel> items = MemberShipMailDAO.GetItems();
            return View(items);
        }

        [HttpGet]
        public ActionResult MailView(long siteId, long id)
        {
            //long menuId = RewardDAO.GetMenuID(siteId);
            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathBySiteID(siteId, "Member").TrimEnd('/') + "/";

            WorkV3.Models.MemberShipMailModel item = MemberShipMailDAO.GetItem(id);
            return View(item);
        }

        [HttpGet]
        public ActionResult MailLogList(long siteId, long id)
        {
            IEnumerable<WorkV3.Models.MemberShipMailLogModel> logs = MemberShipMailDAO.GetMailLogs(id);
            ViewBag.MemberShipMailLogInfos = MemberShipMailDAO.GetMemberMailLogItems(id);

            ViewBag.SiteID = siteId;
            return View(logs);
        }
        #endregion
        #region 追踪標記

        [HttpGet]
        public ActionResult AddFlag(long siteId)
        {
            ViewBag.SiteID = siteId;
            ViewBag.Flags = UserFlagDAO.GetFlags(siteId);
            return View();
        }

        [HttpPost]
        public void AddFlag(long siteId, long[] MemberShipIds, string[] flags)
        {
            if (MemberShipIds == null || MemberShipIds.Length == 0 || flags == null || flags.Length == 0)
                return;

            foreach (long MemberShipId in MemberShipIds)
            {
                foreach (string flag in flags)
                {
                    UserFlagDAO.SetItem(siteId, flag, MemberShipId.ToString());
                }
            }
        }

        [HttpGet]
        public ActionResult ModifyFlag(long siteId, long MemberShipId)
        {
            IEnumerable<string> flags = UserFlagDAO.GetFlags(siteId, MemberShipId.ToString());

            ViewBag.SiteID = siteId;
            ViewBag.MemberShipId = MemberShipId;
            ViewBag.AllFlags = UserFlagDAO.GetFlags(siteId);
            return View(flags);
        }

        [HttpPost]
        public void ModifyFlag(long siteId, long MemberShipId, string[] flags)
        {
            IEnumerable<string> oldFlags = UserFlagDAO.GetFlags(siteId, MemberShipId.ToString());

            if (flags == null)
                flags = new string[] { };
            if (oldFlags == null)
                oldFlags = new string[] { };

            IEnumerable<string> addFlags = flags.Except(oldFlags);
            foreach (string flag in addFlags)
            {
                UserFlagDAO.SetItem(siteId, flag, MemberShipId.ToString());
            }

            IEnumerable<string> delFlags = oldFlags.Except(flags);
            foreach (string flag in delFlags)
            {
                UserFlagDAO.Delete(siteId, flag, MemberShipId.ToString());
            }
        }
        #endregion

        #region 批次處理
        [HttpGet]
        public ActionResult ModifyStatus(long siteId)
        {
            ViewBag.SiteID = siteId;
            var regSetModel = MemberShipRegSetDAO.GetItem(siteId);

            ViewBag.MemberShipVerifyType = regSetModel.VerifyType;
            //ViewBag.Flags = UserFlagDAO.GetFlags(siteId);
            return View();
        }

        [HttpPost]
        public string ModifyStatus(long siteId, long[] MemberShipIds, bool? VerifyStatus, bool? MemberStatus, string StatusNote)
        {
            ViewBag.SiteID = siteId;
            var regSetModel = MemberShipRegSetDAO.GetItem(siteId);

            ViewBag.MemberShipVerifyType = regSetModel.VerifyType;
            if (MemberShipIds == null || MemberShipIds.Length == 0 || (!VerifyStatus.HasValue && !MemberStatus.HasValue))
                return "";

            int successCount = 0;
            foreach (long MemberShipId in MemberShipIds)
            {
                bool isSuccess = MemberDAO.ChangeMemberShipStatusAndVerifyStatus(MemberShipId, siteId, MemberStatus, VerifyStatus, StatusNote);
                if (isSuccess)
                    successCount++;
            }
            return successCount.ToString();
        }
        #endregion

        #region client 跨頁操作功能
        string sessionFixID = "action_{0}_{1}";
        public ActionResult AllClientSelectList(long siteId, string menuId, string actionType, DateTime? StartRegTime, DateTime? EndRegTime, DateTime? StartLoginTime, DateTime? EndLoginTime, string[] FavorityList, string[] IdenityList)
        {
            MemberShipSearchModels search = new MemberShipSearchModels();
            search.Sort = "";
            search.SiteID = siteId;
            search.StartRegTime = StartRegTime;
            search.EndRegTime = EndRegTime;
            search.StartLoginTime = StartLoginTime;
            search.EndLoginTime = EndLoginTime;
            search.FavorityList = FavorityList;
            search.IdenityList = IdenityList;

            List<string> items = MemberShipDAO.GetItemsAll(search);


            List<string> selectList = new List<string>();
            if (items != null)
            {
                selectList = SetSelectSessionList(menuId, actionType, items);
            }
            return Json(selectList, JsonRequestBehavior.AllowGet);
        }
        public List<string> GetSelectSessionList(string menuID, string actionType)
        {
            string sessionID = string.Format(sessionFixID, actionType, menuID);
            List<string> selectList = new List<string>();
            if (Session[sessionID] != null)
            {
                selectList = (List<string>)Session[sessionID];
            }
            return selectList;
        }
        public List<string> SetSelectSessionList(string menuID, string actionType, List<string> selectIds)
        {
            string sessionID = string.Format(sessionFixID, actionType, menuID);
            List<string> selectList = GetSelectSessionList(menuID, actionType);
            foreach (string selectId in selectIds)
            {
                if (string.IsNullOrEmpty(selectId))
                    continue;
                if (selectList.Contains(selectId))
                    continue;
                selectList.Add(selectId);
            }
            Session[sessionID] = selectList;
            return selectList;
        }
        #endregion

        #region 匯出 zip
        public FileResult BatExportZip(long siteId, long menuId, string idsStr, bool? IsPrivate)
        {
            SitesModels siteModel = SitesDAO.GetInfo(siteId);
            string uploadUrl = Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/');
            string fileUploadPath = System.Web.Configuration.WebConfigurationManager.AppSettings["WebUpdPath"];

            // 取得 excel 檔
            DataTable MemberItem = GetMemberItem(siteId, idsStr, IsPrivate);
            var excelFile = ((FileContentResult)MemberItemExport(MemberItem)).FileContents;


            //Zip檔名
            string zipType = string.IsNullOrEmpty(idsStr) ? "All" : "Bat";
            string zipFileName = $"會員資料-{zipType}-{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip";

            byte[] result = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (ZipOutputStream zipStream = new ZipOutputStream(ms))
                {
                    // 把 excel 檔塞進 zip 檔
                    ZipEntry excelFileEntry = new ZipEntry($"會員資料表.xls");
                    excelFileEntry.DateTime = DateTime.Now;

                    zipStream.PutNextEntry(excelFileEntry);
                    zipStream.Write(excelFile, 0, excelFile.Length);

                    int idx = 1;
                    foreach (DataRow dr in MemberItem.Rows)
                    {
                        string currentFolderName = $"{idx.ToString("000")}";

                        string MemberAllFileStr = dr["銀行存摺"] + "," + dr["上傳身分證"] + "," + dr["生活照"];
                        var MemberAllFile = MemberAllFileStr.Split(',');
                        foreach (string fileName in MemberAllFile)
                        {
                            try
                            {
                                string filePath = $"{fileUploadPath}\\{siteModel.SN}\\Member\\{fileName}";
                                var appendFile = System.IO.File.ReadAllBytes(filePath);
                                // 把上傳檔案依照資料夾塞進 zip 檔
                                ZipEntry entry = new ZipEntry($"{currentFolderName}/{fileName}");
                                entry.DateTime = DateTime.Now;

                                zipStream.PutNextEntry(entry);
                                zipStream.Write(appendFile, 0, appendFile.Length);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        idx++;
                    }

                    zipStream.CloseEntry();
                    zipStream.IsStreamOwner = false;
                    zipStream.Finish();
                    zipStream.Close();
                    ms.Position = 0;

                    result = ms.ToArray();
                }
            }

            return File(result, "application/zip", zipFileName);
        }

        public FileResult MemberItemExport(DataTable memberTable)
        {
            ViewData["MemberItem"] = memberTable;
            string html = Utility.GetViewHtml(this, "MemberItemExport", null);
            return File(System.Text.Encoding.UTF8.GetBytes(html), "application/vnd.ms-excel");
        }

        public DataTable GetMemberItem(long siteId, string idsStr, bool? privacy)
        {
            WorkV3.Areas.Backend.Models.MemberShipRegSetModels regModel = LoadMemberShipSet(siteId);

            DataTable memberTable = new DataTable();
            DataTable queryTable = new DataTable();
            if (string.IsNullOrEmpty(idsStr))
            {
                MemberShipSearchModels search = new MemberShipSearchModels();
                search.SiteID = siteId;
                queryTable = MemberShipDAO.GetExportItems(search);
            }
            else
            {
                IEnumerable<long> ids = idsStr.Split(',').Select(i => Int64.Parse(i));
                queryTable = MemberShipDAO.GetExportItemsByID(ids);
            }

            memberTable.Columns.Add(new DataColumn("序", typeof(string)));
            foreach (WorkV3.Areas.Backend.Models.MemberShipRegColumnSetModels colModel in regModel.RegColumnSets)
            {
                if (colModel.IsOpen && colModel.ColumnName != "Photo" && colModel.ColumnName != "OrderEpaper")
                {
                    string colName = colModel.ColumnTitle;
                    memberTable.Columns.Add(new DataColumn(colName, typeof(string)));
                }
            }
            memberTable.Columns.Add(new DataColumn("會員狀態", typeof(string)));
            memberTable.Columns.Add(new DataColumn("推薦人代碼", typeof(string)));
            memberTable.Columns.Add(new DataColumn("邀請碼", typeof(string)));
            memberTable.Columns.Add(new DataColumn("類型", typeof(string)));

            int rowIndex = 0;
            foreach (DataRow row in queryTable.Rows)
            {
                rowIndex++;
                DataRow newRow = memberTable.NewRow();
                newRow["序"] = rowIndex.ToString();
                foreach (WorkV3.Areas.Backend.Models.MemberShipRegColumnSetModels colModel in regModel.RegColumnSets)
                {
                    if (colModel.IsOpen && colModel.ColumnName != "Photo" && colModel.ColumnName != "OrderEpaper")
                    {
                        if (queryTable.Columns.Contains(colModel.ColumnName))
                        {
                            newRow[colModel.ColumnTitle] = row[colModel.ColumnName].ToString();
                            if (colModel.ColumnName == "Birthday")
                            {
                                if (!string.IsNullOrEmpty(newRow[colModel.ColumnTitle].ToString()))
                                    newRow[colModel.ColumnTitle] = DateTime.Parse(newRow[colModel.ColumnTitle].ToString()).ToString("yyyy/MM/dd");
                            }
                            if (privacy.HasValue && privacy.Value)
                            {
                                if (colModel.ColumnName == "Name" || colModel.ColumnName == "Email" || colModel.ColumnName == "IDCard"
                                    || colModel.ColumnName == "Mobile" || colModel.ColumnName == "Phone")
                                {
                                    if (!string.IsNullOrEmpty(newRow[colModel.ColumnTitle].ToString()))
                                        newRow[colModel.ColumnTitle] = PrivateData(newRow[colModel.ColumnTitle].ToString());
                                }
                            }
                        }
                        else
                        {

                            if (colModel.ColumnName == "Career")
                            {
                                string colValue = "";
                                var SelectedList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems((long)row["ID"], CareerType);
                                if (SelectedList != null && SelectedList.Count() > 0)
                                {
                                    foreach (Models.CategoryModels selCate in SelectedList)
                                    {
                                        colValue += selCate.Title + "/";
                                    }
                                }
                                colValue = colValue.Trim('/');
                                newRow[colModel.ColumnTitle] = colValue;
                            }

                            if (colModel.ColumnName == "Identity")
                            {
                                string colValue = "";
                                var SelectedList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems((long)row["ID"], IdentityType);
                                if (SelectedList != null && SelectedList.Count() > 0)
                                {
                                    foreach (Models.CategoryModels selCate in SelectedList)
                                    {
                                        colValue += selCate.Title + "/";
                                    }
                                }
                                colValue = colValue.Trim('/');
                                newRow[colModel.ColumnTitle] = colValue;
                            }

                            if (colModel.ColumnName == "Favority")
                            {
                                string colValue = "";
                                var SelectedList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems((long)row["ID"], FavorityType);
                                if (SelectedList != null && SelectedList.Count() > 0)
                                {
                                    foreach (Models.CategoryModels selCate in SelectedList)
                                    {
                                        colValue += selCate.Title + "/";
                                    }
                                }
                                colValue = colValue.Trim('/');
                                newRow[colModel.ColumnTitle] = colValue;
                            }

                            if (colModel.ColumnName == "Marriage")
                            {
                                string colValue = "";
                                var SelectedList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems((long)row["ID"], MarriageType);
                                if (SelectedList != null && SelectedList.Count() > 0)
                                {
                                    foreach (Models.CategoryModels selCate in SelectedList)
                                    {
                                        colValue += selCate.Title + "/";
                                    }
                                }
                                colValue = colValue.Trim('/');
                                newRow[colModel.ColumnTitle] = colValue;
                            }
                            if (colModel.ColumnName == "Education")
                            {
                                string colValue = "";
                                var SelectedList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems((long)row["ID"], EducationType);
                                if (SelectedList != null && SelectedList.Count() > 0)
                                {
                                    foreach (Models.CategoryModels selCate in SelectedList)
                                    {
                                        colValue += selCate.Title + "/";
                                    }
                                }
                                colValue = colValue.Trim('/');
                                newRow[colModel.ColumnTitle] = colValue;
                            }
                        }
                    }
                }
                newRow["會員狀態"] = !string.IsNullOrEmpty(row["Status"].ToString()) ? (string.IsNullOrEmpty(row["VerifyTime"].ToString()) ? "未驗證" : "正常") : "停權";
                newRow["推薦人代碼"] = row["PushRecommandCode"].ToString();
                newRow["邀請碼"] = row["RecommandCode"].ToString();
                newRow["類型"] = GetMemberTypeName((WorkV3.Models.MemberType)int.Parse(row["MemberType"].ToString()));
                memberTable.Rows.Add(newRow);
            }
            return memberTable;
        }
        #endregion
    }
}