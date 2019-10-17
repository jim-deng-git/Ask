using System.Web.Mvc;
using WorkV3.Models;

namespace WorkV3.Controllers
{
    public class HtmlController : Controller
    {
        public ActionResult Index(CardsModels model)
        {
            var data = PlainTextDAO.Get(model.No, true);
            if (data == null)
                return null;

            int style = model.StylesID;
            return View("Style" + style, data);
        }
    }
}