using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WorkV3.Filters;
using WorkV3.Models;
using WorkV3.Models.DataAccess;
using WorkV3.ViewModels.API;

namespace WorkV3.API
{
    [JwtAuthActionFilter]
    public class ArticlesDetailController : MemberBaseController
    {
        /// <summary>
        /// 最新消息內頁, 需傳入Token
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ApiResult<ArticlesDetailResult> Post([FromBody]ArticlesDetailRequest data)
        {
            ApiResult<ArticlesDetailResult> result = new ApiResult<ArticlesDetailResult>();

            try
            {
                ArticlesDetailResultCode rcode = ArticlesDetailResultCode.Success;
                if (string.IsNullOrEmpty(data.SiteSN))
                {
                    rcode = ArticlesDetailResultCode.SiteNull;
                    result.Success = false;
                    result.Code = (int)rcode;
                    result.Message = rcode.GetMessage();
                    return result;
                }

                long SiteID = GetSiteID(data.SiteSN);
                if(SiteID <= 0)
                {
                    rcode = ArticlesDetailResultCode.SiteNull;
                    result.Success = false;
                    result.Code = (int)rcode;
                    result.Message = rcode.GetMessage();
                    return result;
                }

                long menuId = CardsDAO.GetMenuID(data.CardNo);
                string uploadUrl = Golbal.UpdFileInfo.GetVPathByMenuID(SiteID, menuId);
                string uploadPath = Golbal.UpdFileInfo.GetUPathByMenuID(SiteID, menuId).TrimEnd('\\');

                ArticleModels item = ArticleDAO.GetItemByCard(data.CardNo);

                #region 主影片
                MainVision mainVisionsItem = null;
                if (item.isShowVideo && item.VideoID != null)
                {
                    int width = 0, height = 0;
                    string link = item.VideoID, shotUrl = item.VideoImg;

                    if (item.VideoType == "custom")
                        link = uploadUrl + item.VideoID;

                    string imgSource = "";
                    if (item.VideoImgIsCustom)
                    {
                        ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(item.VideoImg);
                        shotUrl = uploadUrl + imgModel.Img;

                        imgSource = $"{uploadPath}\\{imgModel.Img}";
                    }
                    else
                    {
                        imgSource = item.VideoImg;
                    }

                    GetImageWidthAndHeight(imgSource, out width, out height);

                    mainVisionsItem = new MainVision()
                    {
                        Type = item.VideoType,
                        Link = link,
                        ImgUrl = shotUrl,
                        ImgWidth = width,
                        ImgHeight = height
                    };

                }
                #endregion

                #region Paragraph
                List<ParagraphItem> paragraphList = new List<ParagraphItem>();
                if (item == null)
                {
                    rcode = ArticlesDetailResultCode.ArticlesNull;
                    result.Success = false;
                    result.Code = (int)rcode;
                    result.Message = rcode.GetMessage();
                    return result;
                }
                else
                    paragraphList = GetParagraphItem(item.ID, uploadUrl, uploadPath);
                #endregion

                #region 文章類別
                IEnumerable<ArticleTypesModels> ItemTypes = ArticleDAO.GetItemTypes(item.ID);
                List<ArticleTypes> articleTypesItem = new List<ArticleTypes>();
                foreach (var type in ItemTypes)
                {
                    articleTypesItem.Add(new ArticleTypes()
                    {
                        Name = type.Name,
                        Color = GetColorCode(type.Color)
                    });
                }
                #endregion

                result.Content = new ArticlesDetailResult()
                {
                    Title = item.Title,
                    IssueDate = item.IssueDate?.ToString("yyyy.MM.dd"),
                    ArticleTypes = articleTypesItem,
                    MainVision = mainVisionsItem,
                    ParagraphList = paragraphList
                };

                result.Success = true;
                result.Code = (int)rcode;
                result.Message = rcode.GetMessage();
                return result;
            }
            catch (Exception ex)
            {
                result.Code = (int)ResultCode.Exception;
                result.Success = false;
                result.Message = ex.ToString();
            }

            return result;
        }
    }
}