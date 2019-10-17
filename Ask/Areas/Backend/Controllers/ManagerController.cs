using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WorkLib;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Common;
using WorkV3.Models.Repository;

namespace WorkV3.Areas.Backend.Controllers
{
    public class ManagerController : Controller
    {
        private CompanyRepository comRepo = new CompanyRepository();

        // GET: Backend/Manager
        public ActionResult Manager(int? index, MemberSearch search, long siteId = 0)
        {
            IEnumerable<GroupModels> groups = GroupDAO.GetItems();
            foreach (GroupModels group in groups)
            {
                group.SetPermissionsForAllSites(1);
            }

            Pagination pagination = new Pagination
            {
                PageIndex = index ?? 1,
                PageSize = WebInfo.PageSize
            };

            if (Request.HttpMethod == "GET")
            {
                if (index == null)
                {
                    Utility.ClearSearchValue();
                    Session[$"ExportSearch"] = null;
                }
                else
                {
                    MemberSearch prevSearch = Utility.GetSearchValue<MemberSearch>();
                    if (prevSearch != null)
                        search = prevSearch;
                }
            }
            else if (Request.HttpMethod == "POST")
            {
                Utility.SetSearchValue(search);
                Session[$"ExportSearch"] = search;
            }

            int totalRecord;

            List<MemberModels> items = ManagerDAO.GetItems(pagination.PageSize, pagination.PageIndex, out totalRecord, search);
            pagination.TotalRecord = totalRecord;

            ViewBag.Pagination  = pagination;
            ViewBag.SiteID      = siteId;
            ViewBag.Groups      = groups;
            ViewBag.Search      = search;

            return View(items);
        }

        public ActionResult Edit(long? ID, long siteId = 0)
        {
            MemberModels m = new MemberModels();
            if (ID.HasValue)
            {
                m = ManagerDAO.GetItem((long)ID);
                ViewBag.IsNew = false;
            }
            else
            {
                ViewBag.IsNew = true;
            }

            var group = GroupDAO.GetItems();
            ViewBag.group = group;

            ViewBag.ID = ID ?? 0;
            ViewBag.UploadUrl = GetItem.ViewUpdUrl().TrimEnd('/') + "/";
            ViewBag.SiteID = siteId;
            return View(m);
        }

        [HttpPost]
        public ActionResult Edit(MemberModels model, IEnumerable<MemberToCompanyModel> companyItems, long siteId = 0)
        {
            ////密碼
            //string hidpassword = Request["hidpassword"];
            //string hashKey = uRandom.GetRandomCode(10);

            //if (!string.IsNullOrEmpty(hidpassword))
            //{
            //    model.HashKey = hashKey;
            //    model.HashPwd = HashWord.EncryptSHA256(hidpassword, hashKey);
            //}

            ViewBag.Exit = true;

            HttpPostedFileBase imgFile = model.imgFile;
            if (imgFile != null && imgFile.ContentLength > 0)
            {
                string Path = string.Format("{0}/{1}", GetItem.UpdPath(), "Manager");
                if (!System.IO.Directory.Exists(Path))
                    System.IO.Directory.CreateDirectory(Path);
                string saveName = WorkV3.Golbal.UpdFileInfo.SaveFiles(imgFile, Path);

                model.Img = saveName;
            }
            ManagerDAO.SetItem(model);
            //ManagerDAO.SetMemberToCompany(model.Id, companyItems); 20190912 Joe 問題單，目前尚無MemberToCompany這張表，故先註解

            var group = GroupDAO.GetItems();
            ViewBag.group = group;
            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathBySiteID(siteId).TrimEnd('/') + "/";
            ViewBag.SiteID = siteId;

            return View(model);
        }

        public ActionResult ManagerDetail(long memberId)
        {
            WorkV3.Areas.Backend.Models.MemberModels member = MemberDAO.GetItem(memberId);

            return View(member);
        }

        public ActionResult ChangePassword(long memberId)
        {
            return View(memberId);
        }

        public ActionResult AddCompany()
        {
            ViewBag.Companys = comRepo.GetItems(new Dictionary<string, object> { { "IsIssue", 1 } });
            return View();
        }

        [HttpPost]
        public void ChangePassword(long memberId, string password)
        {
            MemberModels member = MemberDAO.GetItem(memberId);
            member.Password = password;

            ManagerDAO.SetItem(member);
        }

        [HttpPost]
        public void DeleteManager(IEnumerable<long> ids)
        {
            ManagerDAO.DeleteManager(ids);
        }

        /// <summary>
        /// 上傳檔案(單檔)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadPersonPhotoFile(HttpPostedFileBase File, long memberId)
        {
            if (File != null && File.ContentLength > 0)
            {
                string Path = string.Format("{0}/{1}", GetItem.UpdPath(), "Manager");
                if (!System.IO.Directory.Exists(Path))
                    System.IO.Directory.CreateDirectory(Path);
                string saveName = WorkV3.Golbal.UpdFileInfo.SaveFiles(File, Path);

                if (memberId != 0)
                {
                    MemberModels mem = MemberDAO.GetItem(memberId);
                    string sql = "UPDATE Member  SET Img=@Img WHERE LoginID=@ID";

                    SQLData.Database db = new SQLData.Database(WebInfo.Conn);
                    SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
                    paraList.Add("@ID", mem.LoginID);
                    paraList.Add("@Img", saveName);
                    int exeCount = db.ExecuteNonQuery(sql, paraList);
                }
            }
            return Json("success");
        }

        public int checkPasswordSimilarity(long memberId, string password)
        {
            IEnumerable<string> siteSns = WorkV3.Models.DataAccess.SitesDAO.GetDatas().Select(x => x.SN);
            string strSites = String.Join("|", WorkV3.Models.DataAccess.SitesDAO.GetDatas().Select(x => x.SN).ToArray());
            MemberModels member = MemberDAO.GetItem(memberId);
            string pattern = $@"^(?i:({strSites}))(?:20[0-9]{{2}})$";
            Regex regex = new Regex(pattern);

            bool retValue = regex.Match(password).Success;

            return retValue ? 1 : 0;
        }

        public void Sort(List<SortItem> items)
        {
            CommonDA.Sort("Member", items, "");
        }

        public FileResult Export(bool? privacy)
        {
            MemberSearch search = Session[$"ExportSearch"] as MemberSearch;
            if (search == null)
                search = new MemberSearch();

            ViewData["Info"] = ManagerDAO.GetAll(search);

            ViewData["Privacy"] = privacy ?? false;

            string html = Utility.GetViewHtml(this, "Export", null);
            
            string title = $"管理者名單{DateTime.Now.ToString("yyyyMMdd")}.xls";
            return File(System.Text.Encoding.UTF8.GetBytes(html), "application/vnd.ms-excel", title);
        }

        public string CheckEmail(string email)
        {
            bool retValue = MemberDAO.IsEmailExist(email);
            return retValue ? "1" : "0";
        }

        public string CheckUserID(string userID)
        {
            bool retValue = MemberDAO.IsUserIdExist(userID);
            return retValue ? "1" : "0";
        }
    }
}