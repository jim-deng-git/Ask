using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkLib;
using System.Text.RegularExpressions;
using System.IO;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;

namespace WorkV3.Golbal
{
    public class PubFunc
    {
        #region 上傳檔案圖示

        public static string GetFileIcon(string fileName)
        {
            string extName = string.Empty;
            int index = fileName.LastIndexOf('.');
            if (index != -1 && index < fileName.Length - 1)
                extName = fileName.Substring(index + 1);

            string root = HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/') + "/Images/file/";
            switch (extName)
            {
                case "xls":
                case "xlsx":
                    return root + "xls.png";
                case "doc":
                case "docx":
                    return root + "doc.png";
                case "ppt":
                case "pptx":
                    return root + "pps.png";
                case "txt":
                    return root + "txt.png";
                case "htm":
                case "html":
                    return root + "htm.png";
                case "zip":
                    return root + "zip.png";
                case "rar":
                    return root + "rar.png";
                case "png":
                    return root + "png.png";
                case "gif":
                    return root + "gif.png";
                case "mp3":
                    return root + "mp3.png";
                case "flv":
                    return root + "flv.png";
                case "swf":
                    return root + "swf.png";
                case "pdf":
                    return root + "pdf.png";
                case "jpg":
                    return root + "jpg.png";
                case "jpeg":
                    return root + "jpeg.png";
                case "tgz":
                    return root + "tgz.png";
                default:
                    return root + "download.png";
            }
        }
        #endregion

        #region Page
        /// <summary>
        /// 為一個主頁面增加相關的 Page、Zone、Card 信息
        /// </summary>
        /// <param name="siteId">站點 ID</param>
        /// <param name="menuId">選單 ID</param>
        /// <param name="name">頁面名稱</param>
        /// <param name="cardType">Card 类型（對應到 Controller）</param>
        /// <param name="viewAction">操作（對應到 Controller Action）</param>
        /// <param name="appendIdToName">是否將 ID 附加到頁面名稱後</param>
        /// <param name="title">頁面標題</param>
        /// <param name="styleId">指定樣式</param>
        /// <returns>返回 CardID</returns>
        public static long AddPage(long siteId, long menuId, string name, string cardType, string viewAction = null, bool appendIdToName = false, string title = null, int ZoneStyleId = 1, bool isMenuAdd = false, int CardStyleId = 1)
        {
            long CardNO = 0;

            long PageNo = WorkLib.GetItem.NewSN();
            if (isMenuAdd == true)
                PageNo = menuId;

            PagesModels page = new PagesModels
            {
                No = PageNo,
                SiteID = siteId,
                MenuID = menuId,
                SN = name,
                Title = title
            };
            if (appendIdToName)
                page.SN += "_" + page.No;

            SysLog.SaveLog(SysActions.Add, SysMgrNo.Page, title, siteId, menuId, PageNo);
            PagesDAO.SetPageInfo(page);




            #region 增加動態 ZONE、CARD
            switch (cardType)
            {
                //例外的卡
                case "Intro":

                    break;
                default:

                    ZonesModels zone = new ZonesModels
                    {
                        No = WorkLib.GetItem.NewSN(),
                        SiteID = siteId,
                        PageNo = page.No,
                        StyleID = ZoneStyleId,
                        Sort = 10, //需排在固定項後
                        ShowStatus = 1
                    };
                    ZonesDAO.SetZoneInfo(zone);
                    if (string.IsNullOrEmpty(viewAction) && cardType == "Form")
                    {
                        viewAction = "Edit";
                    }
                    CardsModels card = new CardsModels
                    {
                        No = WorkLib.GetItem.NewSN(),
                        ZoneNo = zone.No,
                        CardsType = cardType,
                        StylesID = CardStyleId,
                        ViewAction = viewAction,
                        Status = 1

                    };
                    CardsDAO.SetCardInfo(card);
                    CardNO = card.No;
                    break;
            }
         

            #endregion

            if(cardType == "Reward")
            return CardNO;// wei 20180914 集點捨去固定ZONE內容
            #region 固定ZONE
            //CardsType,Zones.Style,Zones.Sort,Cards.StyleID
            string[,] FixedItem = { { "Header", "1000", "1" , WebInfo.StyleHeader }, { "BreadCrumbs", "1002", "5", "1" }, { "Footer", "1001", "20", WebInfo.StyleFooter } };

            if (FixedItem.Rank > 0)
            {
                for (int i = 0; i < FixedItem.GetLength(0); i++)
                {

                    if (page.SN.ToLower() == "index" && FixedItem[i, 0] == "BreadCrumbs")
                    {

                    }
                    else
                    {
                        ZonesModels NewZone = new ZonesModels
                        {
                            No = WorkLib.GetItem.NewSN(),
                            SiteID = siteId,
                            PageNo = page.No,
                            StyleID = int.Parse(FixedItem[i, 1]),
                            Sort = byte.Parse(FixedItem[i, 2]),
                            ShowStatus = 1
                        };
                        ZonesDAO.SetZoneInfo(NewZone);
                        CardsModels NewCard = new CardsModels
                        {
                            No = WorkLib.GetItem.NewSN(),
                            ZoneNo = NewZone.No,
                            CardsType = FixedItem[i, 0],
                            Status = 1,
                            StylesID = int.Parse(FixedItem[i, 3])

                        };
                        CardsDAO.SetCardInfo(NewCard);

                    }

                }
            }

            #endregion
            
            return CardNO;
        }



        /// <summary>
        /// 為一個主頁面增加相關的 Page、Zone、Card 信息
        /// </summary>
        /// <param name="sourcePage">原始頁面</param>
        /// <param name="targetSiteId">站點 ID</param>
        /// <param name="targetMenuId">選單 ID</param>
        /// <returns>返回 CardID</returns>
        public static long CopyPage(PagesModels sourcePage, long targetSiteId, long targetMenuId)
        {
            long CardNO = 0;

            long PageNo = WorkLib.GetItem.NewSN();
            var sourceMnu = WorkV3.Models.DataAccess.MenusDAO.GetInfo(sourcePage.SiteID, sourcePage.MenuID);
            var targetMnu = WorkV3.Models.DataAccess.MenusDAO.GetInfo(targetSiteId, targetMenuId);
            PagesModels newPage = new PagesModels
            {
                No = PageNo,
                SiteID = targetSiteId,
                MenuID = targetMenuId,
                SN = targetMnu.SN,
                Title = sourcePage.Title + "- (複製)"
            };
            newPage.SN += "_" + newPage.No;
            //WorkLib.WriteLog.Write(true, "newPage.No:" + newPage.No.ToString()+"/"+ newPage.Title);
            SysLog.SaveLog(SysActions.Add, SysMgrNo.Page, newPage.Title, targetSiteId, targetMenuId, PageNo);
            PagesDAO.SetPageInfo(newPage);

            var sourceZones = WorkV3.Models.DataAccess.ZonesDAO.GetPageData(sourcePage.SiteID, sourcePage.No);
            foreach (WorkV3.Models.ZonesModels sourceZone in sourceZones)
            {
                ZonesModels newZone = new ZonesModels
                {
                    No = WorkLib.GetItem.NewSN(),
                    SiteID = targetSiteId,
                    PageNo = newPage.No,
                    StyleID = sourceZone.StyleID,
                    Sort = sourceZone.Sort,
                    ShowStatus = sourceZone.ShowStatus
                };
                ZonesDAO.SetZoneInfo(newZone);

                var sourceCards = WorkV3.Models.DataAccess.CardsDAO.GetZoneData(sourceZone.SiteID, sourceZone.No);
                foreach (WorkV3.Models.CardsModels sourceCard in sourceCards)
                {
                    CardsModels newCard = new CardsModels
                    {
                        No = WorkLib.GetItem.NewSN(),
                        ZoneNo = newZone.No,
                        CardsType = sourceCard.CardsType,
                        ViewAction = sourceCard.ViewAction,
                        Status = sourceCard.Status,
                        StylesID = sourceCard.StylesID

                    };
                    CardsDAO.SetCardInfo(newCard);
                    if (newCard.CardsType == "Article" || newCard.CardsType == "Event")
                        CardNO = newCard.No;
                }
            }
            return CardNO;
        }

        /// <summary>
        /// 移動頁面
        /// </summary>
        /// <param name="sourcePage">原始頁面</param>
        /// <param name="targetSiteId">站點 ID</param>
        /// <param name="targetMenuId">選單 ID</param>
        /// <returns>返回 bool</returns>
        public static bool MovePage(PagesModels sourcePage, long targetSiteId, long targetMenuId)
        {
            WorkV3.Areas.Backend.Models.DataAccess.ZonesDAO.MoveZone(sourcePage.No, targetSiteId);

            var sourceMenu = WorkV3.Models.DataAccess.MenusDAO.GetInfo(sourcePage.SiteID, sourcePage.MenuID);
            long sourceSiteID = sourcePage.SiteID;
            long sourceMenuID = sourcePage.MenuID;

            var targetMenu = WorkV3.Models.DataAccess.MenusDAO.GetInfo(targetSiteId, targetMenuId);
            sourcePage.SiteID = targetSiteId;
            sourcePage.MenuID = targetMenuId;
            sourcePage.SN = targetMenu.SN + "_" + sourcePage.No;

            SysLog.SaveLog(SysActions.Move, SysMgrNo.Page, sourcePage.Title, sourceSiteID, sourceMenuID, sourcePage.No);
            PagesDAO.UpdatePageInfo(sourcePage);
            return true;
        }

        public static void CopyIcon(string sourceIcon, long sourceSiteID, long sourceMenuID, long targetSiteID, long targetMenuID)
        {
            if (string.IsNullOrEmpty(sourceIcon))
            {
                return;
            }
            ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(sourceIcon);
            string sourceFolder = WorkV3.Golbal.UpdFileInfo.GetUPathByMenuID(sourceSiteID, sourceMenuID);
            string targetFolder = WorkV3.Golbal.UpdFileInfo.GetUPathByMenuID(targetSiteID, targetMenuID);

            if (sourceFolder == targetFolder)
                return;
            if (!System.IO.Directory.Exists(targetFolder))
                System.IO.Directory.CreateDirectory(targetFolder);
            if (System.IO.File.Exists(sourceFolder + "\\" + imgModel.Img))
            {
                System.IO.File.Copy(sourceFolder + "\\" + imgModel.Img, targetFolder + "\\" + imgModel.Img, true);
            }
        }
        public static void CopyParagraphPhotos(long sourceNo, long sourceSiteID, long sourceMenuID, long targetSiteID, long targetMenuID)
        {
            string sourceFolder = WorkV3.Golbal.UpdFileInfo.GetUPathByMenuID(sourceSiteID, sourceMenuID);
            string targetFolder = WorkV3.Golbal.UpdFileInfo.GetUPathByMenuID(targetSiteID, targetMenuID);

            if (sourceFolder == targetFolder)
                return;
            if (!System.IO.Directory.Exists(targetFolder))
                System.IO.Directory.CreateDirectory(targetFolder);
            var paragraphs = WorkV3.Models.DataAccess.ParagraphDAO.GetItems(sourceNo);
            if (paragraphs != null && paragraphs.Count() > 0)
            {
                foreach (WorkV3.Models.ParagraphModels paragraph in paragraphs)
                {
                    var reourceImages = paragraph.GetImages();
                    if (reourceImages != null && reourceImages.Count() > 0)
                    {
                        foreach (WorkV3.Models.ResourceImagesModels reourceImage in reourceImages)
                        {
                            if (!string.IsNullOrEmpty(reourceImage.Img))
                            {
                                if (System.IO.File.Exists(sourceFolder + "\\" + reourceImage.Img))
                                {
                                    System.IO.File.Copy(sourceFolder + "\\" + reourceImage.Img, targetFolder + "\\" + reourceImage.Img, true);
                                }
                            }
                        }
                    }
                    var reourceFiles = paragraph.GetFiles();
                    if (reourceFiles != null && reourceFiles.Count() > 0)
                    {
                        foreach (WorkV3.Models.ResourceFilesModels reourceFile in reourceFiles)
                        {
                            if (!string.IsNullOrEmpty(reourceFile.FileInfo))
                            {
                                if (System.IO.File.Exists(sourceFolder + "\\" + reourceFile.FileInfo))
                                {
                                    System.IO.File.Copy(sourceFolder + "\\" + reourceFile.FileInfo, targetFolder + "\\" + reourceFile.FileInfo, true);
                                }
                            }
                        }
                    }
                    var videoFile = paragraph.GetVideo();
                    if (videoFile != null)
                    {
                        if (!string.IsNullOrEmpty(videoFile.Screenshot))
                        {
                            if (System.IO.File.Exists(sourceFolder + "\\" + videoFile.Screenshot))
                            {
                                System.IO.File.Copy(sourceFolder + "\\" + videoFile.Screenshot, targetFolder + "\\" + videoFile.Screenshot, true);
                            }
                        }
                        if (videoFile.Type == "custom")
                        {
                            if (!string.IsNullOrEmpty(videoFile.Link))
                            {
                                if (System.IO.File.Exists(sourceFolder + "\\" + videoFile.Link))
                                {
                                    System.IO.File.Copy(sourceFolder + "\\" + videoFile.Link, targetFolder + "\\" + videoFile.Link, true);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// 取日期是星期幾
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string GetDateOfWeek(DateTime date)
        {
            string[] Day = new string[] { "日", "一", "二", "三", "四", "五", "六" };
            return Day[Convert.ToInt32(date.DayOfWeek.ToString("d"))].ToString();
        }

        /// <summary>
        /// 日期格式化(04.04 (二)<br />16:30)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string DateTimeFmt(DateTime date)
        {
            string str = date.ToString("MM/dd");
            str += " (" + GetDateOfWeek(date) + ") ";
            //str += "<br />";
            str += date.ToString("HH:mm");
            return str;
        }
        /// <summary>
         /// 日期格式化(04.04 (二)<br />16:30)
         /// </summary>
         /// <param name="date"></param>
         /// <returns></returns>
        public static string DateTimeFmt2(DateTime date)
        {
            string str = date.ToString("yyyy/MM/dd");
            str += " (" + GetDateOfWeek(date) + ") ";
            //str += "<br />";
            str += date.ToString("HH:mm");
            return str;
        }

        public static string GetDBValue(object obj)
        {
            if (obj == null) return "";
            return obj.ToString();
        }

        #region 加解密

        private static readonly String strAesKey = "huashan1914huashan1914huashan191";//加密所需32們密匙

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="str">要加密內容</param>
        /// <returns></returns>
        public static string Encrypt_AES(String str)
        {
            Byte[] keyArray = System.Text.UTF8Encoding.UTF8.GetBytes(strAesKey);
            Byte[] toEncryptArray = System.Text.UTF8Encoding.UTF8.GetBytes(str);

            System.Security.Cryptography.RijndaelManaged rDel = new System.Security.Cryptography.RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = System.Security.Cryptography.CipherMode.ECB;
            rDel.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

            System.Security.Cryptography.ICryptoTransform cTransform = rDel.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="str">要解密字符串</param>
        /// <returns>返回解密后字符串</returns>
        public static string Decrypt_AES(String str)
        {
            try
            {
                Byte[] keyArray = System.Text.UTF8Encoding.UTF8.GetBytes(strAesKey);
                Byte[] toEncryptArray = Convert.FromBase64String(str);

                System.Security.Cryptography.RijndaelManaged rDel = new System.Security.Cryptography.RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = System.Security.Cryptography.CipherMode.ECB;
                rDel.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

                System.Security.Cryptography.ICryptoTransform cTransform = rDel.CreateDecryptor();
                Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return System.Text.UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch(Exception ex)
            {
                return "";
            }
        }

        #endregion

        #region 個資處理

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type">1:姓名；2：電話；3：Email</param>
        /// <returns></returns>
        public static string PrivateDateFmt(object obj, string type)
        {
            if (obj == null) return "";
            string result = obj.ToString();
            if (string.IsNullOrWhiteSpace(result)) return "";

            if (type == "1")
            {
                result = result.Substring(0, 1) + "xx";
            }
            else if (type == "2")
            {
                result = result.Substring(0, result.Length / 2) + "xxxx";
            }
            else if (type == "3")
            {
                if (result.IndexOf("@") > 0)
                {
                    result = "xxxxx" + result.Substring(result.IndexOf("@"));
                }
            }
            return result;
        }

        #endregion

        /// <summary>
        /// 獲取客戶端IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            string result = String.Empty;
            result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }
            if (null == result || result == String.Empty)
            {
                return "0.0.0.0";
            }
            return result;
        }
        public static string CreateSitemap(long siteID, string serverRootPath)
        {
            try
            {
                var SiteList = Models.DataAccess.SitesDAO.GetDatas();
                Models.SitesModels SitesInfo = Models.DataAccess.SitesDAO.GetInfo(siteID);
                string applicationPath = "/" + HttpContext.Current.Request.ApplicationPath.Trim('/').Trim('/');
                string siteUrlRoot = GetItem.appSet("WebUrl").ToString() + applicationPath + "/w/" + SitesInfo.SN;
                string xmlName = GetSitemapFileName(siteID);
                System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
                System.Xml.XmlDeclaration xmldec = xmldoc.CreateXmlDeclaration("1.0", "UTF-8", "");
                xmldoc.AppendChild(xmldec);
                System.Xml.XmlElement root = xmldoc.CreateElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
                var menuList = Areas.Backend.Models.DataAccess.StatisticConditionDAO.GetMenuORPages(siteID, null);
                if (menuList != null && menuList.Count() > 0)
                    CreateNode(siteUrlRoot, siteID, xmldoc, ref root, menuList.ToList());
                //List<Models.MenusModels> Menus = Models.DataAccess.MenusDAO.GetFrontMenus(siteID);
                //var MenuU = Menus.Where(dr => dr.AreaID == 1).ToList();
                //CreateMenu(siteUrlRoot, xmldoc, ref root, MenuU, MenuU.Where(m => m.ParentID == 0));
                //var MenuM = Menus.Where(dr => dr.AreaID == 2).ToList();
                //CreateMenu(siteUrlRoot, xmldoc, ref root, MenuM, MenuM.Where(m => m.ParentID == 0));
                xmldoc.AppendChild(root);
                xmldoc.Save(string.Format("{0}\\{1}", serverRootPath, xmlName));
                if (siteID == 1)
                    xmldoc.Save(string.Format("{0}\\sitemap.xml", serverRootPath));
                return xmlName;
            }
            catch(Exception ex)
            {
                return "";
            }
        }
        private static void CreateNode(string siteUrlRoot, long siteID, System.Xml.XmlDocument xmldoc, ref System.Xml.XmlElement parent, List<Areas.Backend.ViewModels.MenuStructure> menuList)
        {
            if (menuList != null && menuList.Count > 0)
            {
                IEnumerable<Areas.Backend.ViewModels.MenuStructure> childList = null;
                foreach (Areas.Backend.ViewModels.MenuStructure node in menuList)
                {
                    string url = "", title="", lastmod = "";
                    if (node.Type == Areas.Backend.ViewModels.StructureType.Menu)
                    {
                        Models.MenusModels item = Models.DataAccess.MenusDAO.GetInfo(siteID, long.Parse(node.ID));
                        if (item == null || string.IsNullOrEmpty(item.Title) || string.IsNullOrEmpty(item.SN))
                            continue;
                        url = string.Format("{0}/{1}", siteUrlRoot, item.SN);
                        title = item.Title;
                        lastmod = item.ModifyTime.HasValue ? item.ModifyTime.Value.ToString("yyyy-MM-dd") : (item.CreateTime.HasValue ? item.CreateTime.Value.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd"));

                         childList = Areas.Backend.Models.DataAccess.StatisticConditionDAO.GetMenuORPages(siteID, long.Parse(node.ID));
                    }
                    else
                    {
                        Areas.Backend.Models.PagesModels item = Areas.Backend.Models.DataAccess.PagesDAO.GetPageInfo(siteID, long.Parse(node.ID));
                        if (item == null || string.IsNullOrEmpty(item.Title) || string.IsNullOrEmpty(item.SN))
                            continue;
                        url = string.Format("{0}/{1}", siteUrlRoot, item.SN);
                        title = item.Title;
                        lastmod = item.ModifyTime.HasValue ? item.ModifyTime.Value.ToString("yyyy-MM-dd") : (item.CreateTime.HasValue ? item.CreateTime.Value.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd"));

                    }
                    if (childList != null && childList.Count() > 0)
                        CreateNode(siteUrlRoot, siteID, xmldoc, ref parent, childList.ToList());
                    System.Xml.XmlElement urlNode = xmldoc.CreateElement("url");
                    System.Xml.XmlElement locNode = xmldoc.CreateElement("loc");
                    System.Xml.XmlElement lastmodNode = xmldoc.CreateElement("lastmod");
                    System.Xml.XmlElement titleNode = xmldoc.CreateElement("title");
                    titleNode.InnerXml = title;
                    lastmodNode.InnerXml = lastmod;
                    locNode.InnerXml = url;
                    urlNode.AppendChild(titleNode);
                    urlNode.AppendChild(locNode);
                    urlNode.AppendChild(lastmodNode);
                    parent.AppendChild(urlNode);
                }
            }
        }
        public static string GetSitemapFileName(long siteID)
        {
            Models.SitesModels SitesInfo = Models.DataAccess.SitesDAO.GetInfo(siteID);
            string xmlName = string.Format("sitemap_{0}.xml", SitesInfo.SN);
            return xmlName;
        }
        public static string GetSitemapUrl(long siteID)
        {
            string siteUrlRoot = WorkLib.uUrl.GetURL().TrimEnd('/') + "/" + GetSitemapFileName(siteID);
            return siteUrlRoot;
        }
        //private static void CreateMenu(string siteUrlRoot, System.Xml.XmlDocument xmldoc, ref System.Xml.XmlElement parent, IEnumerable<Models.MenusModels> allMenus, IEnumerable<Models.MenusModels> currentMenus)
        //{
        //    if (currentMenus?.Count() > 0)
        //    {
        //        foreach (Models.MenusModels item in currentMenus)
        //        {
        //            if (item.ShowStatus!=1)
        //                continue;
        //            if (string.IsNullOrEmpty(item.PageSN))
        //                continue;
        //            string url = item.MenuCode == "SingleLink" ? item.LinkInfo :  string.Format("{0}/{1}", siteUrlRoot, item.PageSN);

        //            System.Xml.XmlElement urlNode = xmldoc.CreateElement("url");
        //            System.Xml.XmlElement locNode = xmldoc.CreateElement( "loc");
        //            System.Xml.XmlElement lastmodNode = xmldoc.CreateElement("lastmod");
        //            System.Xml.XmlElement titleNode = xmldoc.CreateElement( "title");
        //            titleNode.InnerXml =item.Title;
        //            lastmodNode.InnerXml = item.ModifyTime.HasValue?item.ModifyTime.Value.ToString("yyyy-MM-dd"): (item.CreateTime.HasValue?item.CreateTime.Value.ToString("yyyy-MM-dd"):DateTime.Now.ToString("yyyy-MM-dd")) ;
        //            locNode.InnerXml = url;
        //            urlNode.AppendChild(titleNode);
        //            urlNode.AppendChild(locNode);
        //            urlNode.AppendChild(lastmodNode);
        //            parent.AppendChild(urlNode);
        //            IEnumerable<Models.MenusModels> subMenus = allMenus.Where(dr => dr.ParentID == item.Id);

        //            if (subMenus?.Count() > 0)
        //            {
        //                CreateMenu(siteUrlRoot, xmldoc, ref parent, allMenus, subMenus);
        //            }
        //        }
        //    }
        //}
    }

}