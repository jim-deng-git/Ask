using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models;

namespace WorkV3.ViewModels
{
    public class EpaperViewModel
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Img { get; set; }
        public string Publish_Date_Start { get; set; }
        public bool IsCurrent { get; set; }
        public List<WorkV3.Areas.Backend.Models.EpaperAreaModel> BodyAreaModels
        {

            get
            {
                if (string.IsNullOrWhiteSpace(ID))
                    return new List<WorkV3.Areas.Backend.Models.EpaperAreaModel>();

                long numID = 0;
                if (long.TryParse(ID, out numID))
                {
                    var data = WorkV3.Areas.Backend.Models.DataAccess.EpaperDAO.GetEpaperAreas(numID, WorkV3.Common.EpaperAreaType.Content);
                    if(data.Count>0)
                    {
                        foreach (var item in data)
                        {
                            
                            item.Img =  !string.IsNullOrWhiteSpace(item.Img)?  Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(item.Img).Img : "";
                        }
                    }
                    return data;
                }
                else
                    return new List<WorkV3.Areas.Backend.Models.EpaperAreaModel>();
            }
        }
    }
}