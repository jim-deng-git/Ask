using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Common;
using WorkV3.Global;
using WorkV3.Models;
using WorkV3.Models.DataAccess;
using WorkV3.Models.Interface;
using Newtonsoft;
using WorkV3.ViewModels;
using WorkV3.Models.Repository;
using WorkV3.Models.Interface;

namespace WorkV3.Controllers
{
    /// <summary>
    /// 點數 (綁定會員)
    /// </summary>
    [MemberShipAuthorize]
    public class PointController : Controller
    {
        private IPointRepository<float> pointRepository;

        public PointController(IPointRepository<float> inPointRepository)
        {
            pointRepository = inPointRepository;
        }

        public ActionResult GetPoint()
        {
            AjaxResponseWithValueViewModel<float> response = new AjaxResponseWithValueViewModel<float>();

            try
            {
                response.Value = pointRepository.GetPoint(Member.Current.ID);
            }
            catch(Exception ex)
            {
                response.Success = false;
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public float GetRate()
        {
            return 30;
        }
    }
}