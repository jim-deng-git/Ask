using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Common;

namespace WorkV3.Controllers
{
    public class PageCommentsController : Controller
    {
        // GET: PageComments
        [HttpGet]
        public PartialViewResult Index(ViewModels.PageCommentViewModel PageComment)
        {
            return PartialView("_PageComments", PageComment);
        }
        // GET: PageComments
        [HttpPost]
        public PartialViewResult Index(ViewModels.PageCommentViewModel PageComment, string PageSN, string Url, string PosterName, string CommentContent)
        {
            long? memberID = (long?)null;
            if (PageComment.ReplyType == ViewModels.CommentType.MemberOnly )
            {
                Member curUser = Member.Current;
                if (curUser == null)
                {
                    //RedirectToAction("Index", "Home", new { siteSn = PageCache.SiteSN, pageSn = "Index" });
                    Response.Redirect(Url); //若不重導, 返回時, 它會呈現空白頁面, 暫時找不到解法.... charlie_shan 2018/02/20
                    return PartialView("_PageComments", PageComment);
                }
                memberID = curUser.ID;
            }
            if (PageComment.ReplyType == ViewModels.CommentType.FB ||
                PageComment.ReplyType == ViewModels.CommentType.Closed)
            {
                //RedirectToAction("Index", "Home", new { siteSn = PageCache.SiteSN, pageSn = "Index" });
                Response.Redirect(Url); //若不重導, 返回時, 它會呈現空白頁面, 暫時找不到解法.... charlie_shan 2018/02/20
                return PartialView("_PageComments", PageComment);
            }
            Models.PageCommentsModels comment = new Models.PageCommentsModels();
            comment.ID = WorkLib.GetItem.NewSN();
            comment.PageSN = PageSN;
            comment.PostDate = DateTime.Now;
            comment.PosterName = PosterName;
            comment.MemberShipID = memberID;
            comment.ShowStatus = true;
            comment.Title = "";
            comment.IP = WorkLib.GetItem.IPAddr();
            comment.IPNum = ((long)WorkLib.GetItem.GetIPNum()).ToString();
            comment.CommentContent = CommentContent;
            Models.DataAccess.PageCommentsDAO.SetItem(comment);
           // ViewBag.Exit = true;
            Response.Redirect(Url); //若不重導, 返回時, 它會呈現空白頁面, 暫時找不到解法.... charlie_shan 2018/02/20
            return PartialView("_PageComments", PageComment);
        }
        public JsonResult GetComments(string SiteID, string PageSN, int? index, int rowCount)
        {
            int totalRecord = 0;
            int pageIndex = 0;
            if (index.HasValue)
                pageIndex = index.Value;
            ViewModels.PageCommentListViewModel commentViewModel = new ViewModels.PageCommentListViewModel();
            IEnumerable<Models.PageCommentsModels> items = Models.DataAccess.PageCommentsDAO.GetItems(long.Parse(SiteID), PageSN, pageIndex, rowCount, out totalRecord);
            
            commentViewModel.TotalRowCount = totalRecord;
            commentViewModel.PageIndex = index.HasValue?index.Value:0;
            commentViewModel.CommentList = items;
            return Json(commentViewModel);
        }
        public JsonResult GetSingleComment(string SiteID, string ID)
        {
            Models.PageCommentsModels item = Models.DataAccess.PageCommentsDAO.GetSingleItem(long.Parse(SiteID), ID);
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PostComments(ViewModels.CommentType ReplyType, string PageSN , string Url, string PosterName, string CommentContent)
        {
            CommentPostHandlerResult result = new CommentPostHandlerResult();
            long? memberID = (long?)null;
            Member curUser = Member.Current;
            if (ReplyType == ViewModels.CommentType.MemberOnly)
            {
                if (curUser == null)
                {
                    result.Result = HandlerResult.MustLogin;
                    result.Message = "請先登入會員或會員登入時效已過期!";
                    return Json(result);
                }
                memberID = curUser.ID;
            }
            if (curUser != null)
            {
                memberID = curUser.ID;
            }
            if (ReplyType == ViewModels.CommentType.FB ||
                ReplyType == ViewModels.CommentType.Closed)
            {
                result.Result = HandlerResult.NotSupport;
                result.Message = "系統不提供回文功能!";
                return Json(result);
            }
            if (Models.DataAccess.PageCommentsDAO.IsPostCommentOverTime(PageSN, memberID))
            {
                result.Result = HandlerResult.NoRepeat;
                result.Message = "您已「回文」過!";
                return Json(result);
            }
            Models.PageCommentsModels comment = new Models.PageCommentsModels();
            comment.ID = WorkLib.GetItem.NewSN();
            comment.PageSN = PageSN;
            comment.PostDate = DateTime.Now;
            comment.PosterName = PosterName;
            comment.MemberShipID = memberID;
            comment.ShowStatus = true;
            comment.Title = "";
            comment.IP = WorkLib.GetItem.IPAddr();
            comment.IPNum = ((long)WorkLib.GetItem.GetIPNum()).ToString();
            comment.CommentContent = CommentContent;
            Models.DataAccess.PageCommentsDAO.SetItem(comment);
            result.Result = HandlerResult.Success;
            result.Message = "回文成功";
            return Json(result);
        }
        public JsonResult PostEditComments(ViewModels.CommentType ReplyType, string ID, string SiteID, string PosterName, string CommentContent)
        {
            CommentPostHandlerResult result = new CommentPostHandlerResult();
            long? memberID = (long?)null;
            Member curUser = Member.Current;
            if (ReplyType == ViewModels.CommentType.MemberOnly)
            {
                if (curUser == null)
                {
                    result.Result = HandlerResult.MustLogin;
                    result.Message = "請先登入會員或會員登入時效已過期!";
                    return Json(result);
                }
                memberID = curUser.ID;
            }
            if (curUser != null)
            {
                memberID = curUser.ID;
            }
            Models.PageCommentsModels comment = Models.DataAccess.PageCommentsDAO.GetSingleItem(long.Parse(SiteID), ID);
            comment.PosterName = PosterName;
            comment.CommentContent = CommentContent;
            Models.DataAccess.PageCommentsDAO.SetItem(comment);
            result.Result = HandlerResult.Success;
            result.Message = "儲存成功";
            return Json(result);
        }

        public JsonResult DeleteComments(ViewModels.CommentType ReplyType, string ID)
        {
            CommentPostHandlerResult result = new CommentPostHandlerResult();
            long? memberID = (long?)null;
            Member curUser = Member.Current;
           
                if (curUser == null)
                {
                    result.Result = HandlerResult.MustLogin;
                    result.Message = "請先登入會員或會員登入時效已過期!";
                    return Json(result);
                }
                memberID = curUser.ID;
           bool IsDelete =  Models.DataAccess.PageCommentsDAO.DeleteItem(long.Parse(ID));
            if (IsDelete)
            {
                result.Result = HandlerResult.Success;
                result.Message = "刪除成功";
            }
            else
            {
                result.Result = HandlerResult.Fail;
                result.Message = "刪除失敗";
            }
            return Json(result);
        }
        public JsonResult PostReplyComments(ViewModels.CommentType ReplyType, string PageSN, string Url, long ParentID, string PosterName, string CommentContent)
        {
            CommentPostHandlerResult result = new CommentPostHandlerResult();
            long? memberID = (long?)null;
            Member curUser = Member.Current;
            if (ReplyType == ViewModels.CommentType.MemberOnly)
            {
                if (curUser == null)
                {
                    result.Result = HandlerResult.MustLogin;
                    result.Message = "請先登入會員或會員登入時效已過期!";
                    return Json(result);
                }
                memberID = curUser.ID;
            }
            if (curUser != null)
            {
                memberID = curUser.ID;
            }
            if (ReplyType == ViewModels.CommentType.FB ||
                ReplyType == ViewModels.CommentType.Closed)
            {
                result.Result = HandlerResult.NotSupport;
                result.Message = "系統不提供回文功能!";
                return Json(result);
            }
            if (Models.DataAccess.PageCommentsDAO.IsPostCommentOverTime(PageSN, memberID))
            {
                result.Result = HandlerResult.NoRepeat;
                result.Message = "您已「回文」過!";
                return Json(result);
            }
            Models.PageCommentsModels comment = new Models.PageCommentsModels();
            comment.ID = WorkLib.GetItem.NewSN();
            comment.PageSN = PageSN;
            comment.ParentID = ParentID;
            comment.PostDate = DateTime.Now;
            comment.PosterName = PosterName;
            comment.MemberShipID = memberID;
            comment.ShowStatus = true;
            comment.Title = "Re:";
            comment.IP = WorkLib.GetItem.IPAddr();
            comment.IPNum = ((long)WorkLib.GetItem.GetIPNum()).ToString();
            comment.CommentContent = CommentContent;
            Models.DataAccess.PageCommentsDAO.SetItem(comment);
            result.Result = HandlerResult.Success;
            result.Message = "回覆成功";
            return Json(result);
        }
        public JsonResult AddGood(int replyType, long CommentID)
        {
            ViewModels.CommentType ctype = (ViewModels.CommentType)replyType;
            GoodHandlerResult result = new GoodHandlerResult();
            long? memberID = (long?)null;
            //應問題單 太報問題單_201804191630_阿聰.docx 第一點要求, 改為不管後台設定, 要點讚一定要是會員.
            //if (ctype == ViewModels.CommentType.MemberOnly)
            //{
            Member curUser = Member.Current;
                if (curUser == null)
                {
                    result.Result = HandlerResult.MustLogin;
                    result.Message = "請先登入會員或會員登入時效已過期!";
                    return Json(result);
                }
                memberID = curUser.ID;
            //}
            if (ctype == ViewModels.CommentType.FB ||
                ctype == ViewModels.CommentType.Closed)
            {
                result.Result = HandlerResult.NotSupport;
                result.Message = "系統不提供按「讚」功能!";
                return Json(result);
            }

            Models.PageCommentLogsModels logModel = Models.DataAccess.PageCommentsDAO.GetGoodLogs(CommentID, ViewModels.CommentLogType.Good, memberID.Value);
            int goodResultCount = 0;
            if (logModel == null)
                goodResultCount = Models.DataAccess.PageCommentsDAO.AddGoodLogs(CommentID, ViewModels.CommentLogType.Good, Request.Browser.Browser, Request.UserAgent, memberID);
            else
                goodResultCount = Models.DataAccess.PageCommentsDAO.DeleteGoodLogs(CommentID, logModel.ID);
            //if (Models.DataAccess.PageCommentsDAO.IsAddGoodLogOverTime(CommentID, ViewModels.CommentLogType.Good, memberID))
            //{
            //    result.Result = HandlerResult.NoRepeat;
            //    result.Message = "您已點過「讚」!";
            //    return Json(result);
            //}


            result.Result = HandlerResult.Success;
            result.ResultCount = goodResultCount;
            return Json(result);
        }
    }
    public class CommentPostHandlerResult
    {
        public HandlerResult Result { get; set; }
        public string Message { get; set; }
    }
    public class GoodHandlerResult
    {
        public HandlerResult Result { get; set; }
        public string Message { get; set; }
        public int ResultCount { get; set; } = 0;
    }
    public enum HandlerResult
    {
        /// <summary>
        /// 重覆點讚
        /// </summary>
        NoRepeat = 3,
        /// <summary>
        /// 登入先
        /// </summary>
        MustLogin = 2,
        /// <summary>
        /// 送出成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 送出失敗
        /// </summary>
        Fail =0,
        /// <summary>
        /// 系統不提供
        /// </summary>
        NotSupport = 9
    }
}