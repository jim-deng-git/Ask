using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Models;

namespace WorkV3.Controllers
{
    public class ClickController : Controller
    {
        public ActionResult Click(int clickType, string data)
        {
            var model = new object();

            switch (clickType)
            {
                case 1:
                    model = JsonConvert.DeserializeObject<List<ClickImgModel>>(data);
                    return View("Img", model);
                case 2:
                    model = JsonConvert.DeserializeObject<ClickVideoModel>(data);
                    return View("Video", model);
                case 3:
                    model = JsonConvert.DeserializeObject<ClickVoiceModel>(data);
                    return View("Voice", model);
                case 4:
                    model = JsonConvert.DeserializeObject<ClickFileModel>(data);
                    return View("File", model);
                case 5:
                    model = JsonConvert.DeserializeObject<ClickLinkModel>(data);
                    return View("Link", model);
            }

            return null;
        }
    }
}