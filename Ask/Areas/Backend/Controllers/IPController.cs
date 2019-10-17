using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Golbal;
using WorkV3.Common;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;
using Newtonsoft.Json;

namespace WorkV3.Areas.Backend.Controllers
{
    public class IPController : Controller
    {
        public ActionResult List() {

            IEnumerable<IntraIPlimitModel> items = IntraIPlimitDAO.GetItems();
            return View(items);
        }

        #region Edit
        public ActionResult Edit(long? id)
        {
            IntraIPlimitModel item = new IntraIPlimitModel();
            item.IsSystemSet = false;
            if (id != null)
            {
                item = WorkV3.Areas.Backend.Models.DataAccess.IntraIPlimitDAO.GetItem((long)id);
            }
            return View(item);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(IntraIPlimitModel item)
        {
            if (!Utility.CheckIsLogin())
            {
                Response.Redirect(Url.Action("Login", "Home"));
            }
            if (item.ID == 0)
            {
                WorkV3.Areas.Backend.Models.DataAccess.IntraIPlimitDAO.InsertData(item.OpenStatus, item.IP_Begin, item.IP_End, false);
            }
            else
            {
                WorkV3.Areas.Backend.Models.DataAccess.IntraIPlimitDAO.SetItem(item);
            }
            ViewBag.Exit = true;

            return View(item);
        }
        #endregion
        [HttpPost]
        public void IPDel(IEnumerable<long> ids)
        {
            WorkV3.Areas.Backend.Models.DataAccess.IntraIPlimitDAO.Delete(ids);
        }
    }
}