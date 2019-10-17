using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WorkV3.Areas.Backend.Controllers
{
    public class PublicController : Controller
    {
        /// <summary>
        /// 取縣市資料
        /// </summary>
        /// <returns></returns>
        public JsonResult CityList()
        {
            Models.DataAccess.PublicDAO _dao = new Models.DataAccess.PublicDAO();
            return Json(_dao.GetCityList(), "text/html", JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 取行政區資料
        /// </summary>
        /// <returns></returns>
        public JsonResult GetBoroughList(int ZipCode)
        {
            Models.DataAccess.PublicDAO _dao = new Models.DataAccess.PublicDAO();
            return Json(_dao.GetBoroughList(ZipCode), "text/html", JsonRequestBehavior.DenyGet);
        }

    }
}