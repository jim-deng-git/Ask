using System;
using System.Web.Mvc;
using WorkLib;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Models;

namespace WorkV3.Areas.Backend.Controllers
{
    public class HtmlController : Controller
    {
        public ActionResult Edit(long SiteID, long MenuID, long CardNo)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;

            PlainTextModel model = PlainTextDAO.Get(CardNo);
            if (model == null)
            {
                model = new PlainTextModel();
                model.CardNo = CardNo;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(PlainTextModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsValid = false;
                return View(model);
            }

            DateTime now = DateTime.Now;
            if (model.ID == 0)
            {
                model.ID = GetItem.NewSN();
                model.Creator = MemberDAO.SysCurrent.Id;
                model.CreateTime = now;
            }

            model.Modifier = MemberDAO.SysCurrent.Id;
            model.ModifyTime = now;

            int result = PlainTextDAO.Instance.Update(model);
            if (result == 0)
                ViewBag.EditResult = "儲存發生錯誤";
            else
                ViewBag.EditResult = "儲存成功";

            return View(model);
        }
    }
}