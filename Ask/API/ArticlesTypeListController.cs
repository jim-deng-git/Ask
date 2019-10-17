using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Filters;
using WorkV3.Models;
using WorkV3.Models.Repository;
using WorkV3.ViewModels.API;

namespace WorkV3.API
{
    [JwtAuthActionFilter]
    public class ArticlesTypeListController : MemberBaseController
    {
        /// <summary>
        /// 文章模組類別,需傳入token
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ApiResult<List<ArticlesTypeListResult>> Post([FromBody]ArticleModuleRequest data)
        {
            ApiResult<List<ArticlesTypeListResult>> result = new ApiResult<List<ArticlesTypeListResult>>();
            
            try
            {
                ResultCode rcode = ResultCode.Success;
                bool isSuccess = true;

                if (string.IsNullOrEmpty(data.SiteSN))
                {
                    rcode = ResultCode.SiteNull;
                    isSuccess = false;
                    goto Result;
                }
                if (string.IsNullOrEmpty(data.SN))
                {
                    rcode = ResultCode.ModuleSNNull;
                    isSuccess = false;
                    goto Result;
                }
                long SiteID = GetSiteID(data.SiteSN);
                if (SiteID <= 0)
                {
                    rcode = ResultCode.SiteNull;
                    isSuccess = false;
                    goto Result;
                }

                var menuItem = Models.DataAccess.MenusDAO.GetInfo(SiteID, data.SN);
                if (menuItem == null || menuItem.Id <= 0)
                {
                    rcode = ResultCode.MenuNull;
                    result.Success = false;
                    result.Code = (int)rcode;
                    result.Message = rcode.GetMessage();
                    return result;
                }

                var types = ArticleTypesDAO.GetIssueItems(menuItem.Id);

                result.Content = new List<ArticlesTypeListResult>();
                foreach (var item in types)
                {
                    result.Content.Add(new ArticlesTypeListResult()
                    {
                        ID = item.ID,
                        Name = item.Name
                    });
                }

                Result:
                result.Code = (int)rcode;
                result.Success = isSuccess;
                result.Message = rcode.GetMessage();
            }
            catch(Exception ex)
            {
                result.Code = (int)ResultCode.Exception;
                result.Success = false;
                result.Message = ex.ToString();
            }
            
            return result;
        }
    }
}
