using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models;

namespace WorkV3.ViewModels
{
    public class LightBoxViewModel
    {
        public IEnumerable<ResourceImagesModels> Imgs { get; set; }
        public ResourceLightBoxModels lightBox { get; set; }
    }
}