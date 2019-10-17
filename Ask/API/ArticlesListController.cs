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
    public class ArticlesListController : MemberBaseController
    {
        /// <summary>
        /// 最新消息列表, 需傳入Token
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ApiResultWithPage<List<NewsListResult>> Post([FromBody]ArticlesListRequest data)
        {
            ApiResultWithPage<List<NewsListResult>> result = new ApiResultWithPage<List<NewsListResult>>();
            List<NewsListResult> contentItem = new List<NewsListResult>();

            try
            {
                NewsListResultCode rcode = NewsListResultCode.Success;
                if (string.IsNullOrEmpty(data.SiteSN))
                {
                    rcode = NewsListResultCode.SiteNull;
                    result.Success = false;
                    result.Code = (int)rcode;
                    result.Message = rcode.GetMessage();
                    return result;
                }
                if (string.IsNullOrEmpty(data.SN))
                {
                    rcode = NewsListResultCode.ModuleSNNull;
                    result.Success = false;
                    result.Code = (int)rcode;
                    result.Message = rcode.GetMessage();
                    return result;
                }
                long SiteID = GetSiteID(data.SiteSN);
                if(SiteID <= 0)
                {
                    rcode = NewsListResultCode.SiteNull;
                    result.Success = false;
                    result.Code = (int)rcode;
                    result.Message = rcode.GetMessage();
                    return result;
                }
                var menuItem = MenusDAO.GetInfo(SiteID, data.SN);
                if(menuItem == null || menuItem.Id <= 0)
                {
                    rcode = NewsListResultCode.MenuNull;
                    result.Success = false;
                    result.Code = (int)rcode;
                    result.Message = rcode.GetMessage();
                    return result;
                }

                string uploadUrl = Golbal.UpdFileInfo.GetVPathByMenuID(SiteID, menuItem.Id).TrimEnd('/') + "/";

                var setting = ArticleSettingDAO.GetItem(menuItem.Id);
                int pageIndex = data.Index ?? 1;
                int totalRecord;

                //var types = WorkV3.Areas.Backend.Models.DataAccess.ArticleTypesDAO.GetIssueItems(menuItem.Id);
                //if (types == null || types.Count()<=0)
                //{
                //    result.Success = true;
                //    result.Code = (int)rcode;
                //    result.Message = rcode.GetMessage();
                //    result.Content = contentItem;
                //    return result;
                //}
                string typeCond = "";
                if (data.Types != null && data.Types.Count()> 0)
                { typeCond = string.Join(",", data.Types); }
                List<ArticleModels> items = ArticleDAO.GetItems(setting, "", typeCond, null, pageIndex, out totalRecord);
                Dictionary<long, List<ArticleTypesModels>> ItemTypes = ArticleDAO.GetItemTypes(items.Select(item => item.ID));
                foreach(var item in items)
                {
                    #region 文章類別
                    List<ArticleTypes> articleTypesItem = new List<ArticleTypes>();
                    var HasItemTypes = ItemTypes.ContainsKey(item.ID);
                    if (HasItemTypes)
                    {
                        foreach (var type in ItemTypes[item.ID])
                        {
                            articleTypesItem.Add(new ArticleTypes()
                            {
                                Name = type.Name,
                                Color = GetColorCode(type.Color)
                            });
                        }
                    }
                    #endregion

                    string link = string.Empty;
                    if (item.Type == "link")
                        link = item.Link;
                    else if(item.Type == "archive")
                    {
                        if (!string.IsNullOrWhiteSpace(item.Archive)) {
                            ResourceFilesModels file = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceFilesModels>(item.Archive);
                            link = uploadUrl + file.FileInfo;
                        }
                    }

                    contentItem.Add(new NewsListResult()
                    {
                        ID = item.ID,
                        CardNo = item.CardNo,
                        Type = item.Type,
                        Title = item.Title,
                        IssueDate = item.IssueDate?.ToString("yyyy.MM.dd"),
                        Link = link,
                        ArticleTypes = articleTypesItem
                    });
                }

                result.Success = true;
                result.Code = (int)rcode;
                result.Message = rcode.GetMessage();
                result.Content = contentItem;
                return result;
            }
            catch (Exception ex)
            {
                result.Code = (int)NewsListResultCode.Exception;
                result.Success = false;
                result.Message = ex.ToString();
            }

            return result;
        }
    }
}