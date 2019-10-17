using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models.DataAccess;

namespace WorkV3.Models
{
    public class ArticleModels
    {
        public long ID { get; set; }
        public long CardNo { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public DateTime? IssueDate { get; set; }
        public string RemarkText { get; set; }
        public bool CustomIcon { get; set; }
        public string Icon { get; set; }
        public string Link { get; set; }
        public bool IsOpenNew { get; set; }
        public string Archive { get; set; }
        public string Summary { get; set; }
        public int ReadMode { get; set; }
        public ArticleSettingModels ReplyCommentSetting { get; set; }
        public long SiteID { get; set; }

        #region 主影片
        public bool isShowVideo { get; set; }
        public string VideoType { get; set; }
        public string VideoID { get; set; }
        public bool VideoImgIsCustom { get; set; }
        public string VideoImg { get; set; }
        #endregion
        public IEnumerable<ArticleTypesModels> GetTypes() {
            return ArticleDAO.GetItemTypes(ID);
        }
        public IEnumerable<ArticleSeriesModels> GetSeries()
        {
            return ArticleDAO.GetItemSeries(ID);
        }
        public IEnumerable<ArticleCategoryModels> GetCategories(string type)
        {
            return ArticleDAO.GetItemCategories(ID, type);
        }
        public IEnumerable<ArticlePosterModels> GetPosters() {
            return ArticleDAO.GetItemPosters(ID);
        }
        
        public string GetFirstImg(ArticleSettingModels setting) {
            if(CustomIcon && !string.IsNullOrWhiteSpace(Icon)) {
                ResourceImagesModels img = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(Icon);
                if (!string.IsNullOrWhiteSpace(img?.Img))
                    return img.Img;
            }

            string paragraphImg = ParagraphDAO.GetFirstImage(ID);
            if (!string.IsNullOrWhiteSpace(paragraphImg))
                return paragraphImg;

            if(!string.IsNullOrWhiteSpace(setting?.DefaultImg)) {
                ResourceImagesModels img = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(setting.DefaultImg);
                if (!string.IsNullOrWhiteSpace(img?.Img))
                    return img.Img;
            }

            return null;
        }

        public string GetFirstImg(ArticleSetModels setting) {
            if (CustomIcon && !string.IsNullOrWhiteSpace(Icon)) {
                ResourceImagesModels img = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(Icon);
                if (!string.IsNullOrWhiteSpace(img?.Img))
                    return img.Img;
            }

            string paragraphImg = ParagraphDAO.GetFirstImage(ID);
            if (!string.IsNullOrWhiteSpace(paragraphImg))
                return paragraphImg;

            if (!string.IsNullOrWhiteSpace(setting?.DefaultImg)) {
                ResourceImagesModels img = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(setting.DefaultImg);
                if (!string.IsNullOrWhiteSpace(img?.Img))
                    return img.Img;
            }

            return null;
        }

        public string GetFirstImg() {
            if (CustomIcon && !string.IsNullOrWhiteSpace(Icon)) {
                ResourceImagesModels img = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(Icon);
                if (!string.IsNullOrWhiteSpace(img?.Img))
                    return img.Img;
            }

            string paragraphImg = ParagraphDAO.GetFirstImage(ID);
            if (!string.IsNullOrWhiteSpace(paragraphImg))
                return paragraphImg;
            
            return null;
        }

        public ResourceVideosModels GetFirstVideo() {
            if(isShowVideo && !string.IsNullOrWhiteSpace(VideoID)) {
                return new ResourceVideosModels { Type = "youtube", Link = VideoID };
            }

            return ParagraphDAO.GetFirstVideo(ID);
        }


    }
}