using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models.DataAccess;

namespace WorkV3.Models
{
    public class ParagraphModels
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public string MatchType { get; set; }        
        public string ImgPos { get; set; }
        public string ImgAlign { get; set; }

        public IEnumerable<ResourceFilesModels> GetFiles() {
            return ResourceFilesDAO.GetItems(ID, "Match");
        }

        public IEnumerable<ResourceImagesModels> GetImages() {
            return ResourceImagesDAO.GetItems(ID, "Match");
        }

        public IEnumerable<ResourceLinksModels> GetLinks() {
            return ResourceLinksDAO.GetItems(ID, "Match");
        }

        public ResourceVideosModels GetVideo() {
            return ResourceVideosDAO.GetItem(ID, "Match");
        }

        public IEnumerable<ResourceVoicesModels> GetVoices() {
            return ResourceVoicesDAO.GetItems(ID, "Match");
        }

        public ResourceLightBoxModels GetLightBox()
        {
            return ResourceLightBoxDAO.GetItem(ID, "LightBox");
        }
    }
}