using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WorkV3.ViewModels.API;
using Jose;
using WorkV3.Models;
using WorkV3.Models.DataAccess;
using System.IO;
using WorkV3.Common;
using System.Text.RegularExpressions;
using WorkV3.Filters;

namespace WorkV3.API
{
    [JwtAuthActionFilter]
    public class MemberBaseController : ApiController
    {
        protected string API_Key = WorkLib.GetItem.appSet("API_Key").ToString();
        protected string API_IV = WorkLib.GetItem.appSet("API_IV").ToString();
        protected string APISecret = WorkLib.GetItem.appSet("APISecret").ToString();
        protected bool API_IsEncrypt = Convert.ToBoolean( WorkLib.GetItem.appSet("API_IsEncrypt").ToString());

        public long GetSiteID(string siteSN)
        {
            string defaultSiteSN = siteSN;
            //string defaultSiteSN = WorkLib.GetItem.appSet("DefaultSiteSN").ToString();
            var sites = WorkV3.Models.DataAccess.SitesDAO.GetDatas();
            long SiteID = 0;
            if (sites != null && sites.Count() > 0)
            {
                var defaultSite = sites.Where(s => s.SN == defaultSiteSN);
                if (defaultSite != null && defaultSite.Count() > 0)
                    SiteID = defaultSite.First().Id;
            }
            return SiteID;
        }

        /// <summary>
        /// 取得段落
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="uploadUrl"></param>
        /// <param name="uploadPath"></param>
        /// <returns></returns>
        public List<ParagraphItem> GetParagraphItem(long Id, string uploadUrl, string uploadPath)
        {
            List<ParagraphItem> paragraphList = new List<ParagraphItem>();

            IEnumerable<ParagraphModels> paragraphs = ParagraphDAO.GetItems(Id);
            foreach (var paragraph in paragraphs)
            {
                string matchType = (paragraph.MatchType ?? string.Empty).ToLower();
                if (matchType == "img")
                {
                    IEnumerable<ResourceImagesModels> images = paragraph.GetImages().Where(m => m.IsShow);
                    if (images == null) continue;

                    if (images.Count() > 1)
                    {
                        List<ParagraphImageList> imgList = new List<ParagraphImageList>();

                        foreach (var img in images)
                        {
                            int width = 0, height = 0;
                            GetImageWidthAndHeight($"{uploadPath}\\{img.Img}", out width, out height);
                            imgList.Add(new ParagraphImageList()
                            {
                                Url = uploadUrl + img.Img,
                                Width = width,
                                Height = height
                            });
                        }

                        paragraphList.Add(new ParagraphItem()
                        {
                            Type = "ImageGroup",
                            ContentList = imgList
                        });
                    }
                    else
                    {
                        foreach (var img in images)
                        {
                            int width = 0, height = 0;
                            GetImageWidthAndHeight($"{uploadPath}\\{img.Img}", out width, out height);
                            ParagraphImage imgItem = new ParagraphImage
                            {
                                Type = "Image",
                                Content = uploadUrl + img.Img,
                                Width = width,
                                Height = height
                            };
                            paragraphList.Add(imgItem);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(paragraph.Title))
                        paragraphList.Add(new ParagraphItem { Type = "Title", Content = paragraph.Title });

                    if (!string.IsNullOrWhiteSpace(paragraph.Contents))
                        paragraphList.Add(new ParagraphItem { Type = "Text", Content = paragraph.Contents });
                }
                else if (matchType == "video")
                {
                    ResourceVideosModels video = paragraph.GetVideo();
                    string shotUrl = string.Empty, videoUrl = string.Empty;

                    if (video.Type == "custom")
                    {
                        videoUrl = uploadUrl + video.Link;
                        if (!string.IsNullOrWhiteSpace(video.Screenshot))
                        {
                            ResourceImagesModels img = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(video.Screenshot);
                            shotUrl = uploadUrl + img.Img;
                        }
                    }
                    else
                    {
                        videoUrl = video.Link;
                        shotUrl = video.Screenshot;
                        if (video.ScreenshotIsCustom && !string.IsNullOrWhiteSpace(shotUrl))
                        {
                            ResourceImagesModels img = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(shotUrl);
                            shotUrl = uploadUrl + img.Img;
                        }
                    }
                    paragraphList.Add(new ParagraphVideo
                    {
                        Type = "Video",
                        Content = videoUrl,
                        VideoType = video.Type,
                        Screenshot = shotUrl
                    });

                    if (!string.IsNullOrWhiteSpace(paragraph.Title))
                        paragraphList.Add(new ParagraphItem { Type = "Title", Content = paragraph.Title });

                    if (!string.IsNullOrWhiteSpace(paragraph.Contents))
                        paragraphList.Add(new ParagraphItem { Type = "Text", Content = paragraph.Contents });
                }
                else if (matchType == "file")
                {
                    if (!string.IsNullOrWhiteSpace(paragraph.Title))
                        paragraphList.Add(new ParagraphItem { Type = "Title", Content = paragraph.Title });

                    if (!string.IsNullOrWhiteSpace(paragraph.Contents))
                        paragraphList.Add(new ParagraphItem { Type = "Text", Content = paragraph.Contents });

                    IEnumerable<ResourceFilesModels> files = paragraph.GetFiles();
                    foreach (var file in files)
                    {
                        paragraphList.Add(new ParagraphFile
                        {
                            Type = "File",
                            Content = uploadUrl + file.FileInfo,
                            Name = (string.IsNullOrWhiteSpace(file.ShowName) ? System.Text.RegularExpressions.Regex.Replace(file.FileInfo, @"\.[^\.]*$", string.Empty) : file.ShowName)
                        });
                    }
                }
                else if (matchType == "link")
                {
                    if (!string.IsNullOrWhiteSpace(paragraph.Title))
                        paragraphList.Add(new ParagraphItem { Type = "Title", Content = paragraph.Title });

                    if (!string.IsNullOrWhiteSpace(paragraph.Contents))
                        paragraphList.Add(new ParagraphItem { Type = "Text", Content = paragraph.Contents });

                    IEnumerable<ResourceLinksModels> links = paragraph.GetLinks();
                    foreach (var link in links)
                    {
                        paragraphList.Add(new ParagraphFile
                        {
                            Type = "Link",
                            Content = link.LinkInfo,
                            Name = link.Descriptions
                        });
                    }
                }
                else if (matchType == "voice")
                {
                    if (!string.IsNullOrWhiteSpace(paragraph.Title))
                        paragraphList.Add(new ParagraphItem { Type = "Title", Content = paragraph.Title });

                    if (!string.IsNullOrWhiteSpace(paragraph.Contents))
                        paragraphList.Add(new ParagraphItem { Type = "Text", Content = paragraph.Contents });

                    IEnumerable<ResourceVoicesModels> voices = paragraph.GetVoices();
                    foreach (var voice in voices)
                    {
                        paragraphList.Add(new ParagraphItem
                        {
                            Type = "Voice",
                            Content = uploadUrl + voice.Path
                        });
                    }
                }
                else if (string.IsNullOrWhiteSpace(matchType))
                {
                    if (!string.IsNullOrWhiteSpace(paragraph.Title))
                        paragraphList.Add(new ParagraphItem { Type = "Title", Content = paragraph.Title });

                    if (!string.IsNullOrWhiteSpace(paragraph.Contents))
                        paragraphList.Add(new ParagraphItem { Type = "Text", Content = paragraph.Contents });
                }
            }

            return paragraphList;
        }

        /// <summary>
        /// 取得圖片長寬
        /// </summary>
        /// <param name="source"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void GetImageWidthAndHeight(string source, out int width, out int height)
        {
            Regex httpRegex = new Regex(@"^(http|https)://.*$");

            if (httpRegex.IsMatch(source))
                GetWebImageWidthAndHeight(source, out width, out height);
            else
                GetLocalImageWidthAndHeight(source, out width, out height);
        }

        /// <summary>
        /// 取得存於本機中指定圖片長寬
        /// </summary>
        /// <param name="path"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void GetLocalImageWidthAndHeight(string path, out int width, out int height)
        {
            int ImgWidth = 0, ImgHeight = 0;

            try
            {
                if (!File.Exists(path))
                    throw new Exception("檔案路徑不存在");

                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    System.Drawing.Image uImage = System.Drawing.Image.FromStream(fs);
                    ImgWidth = uImage.Width;
                    ImgHeight = uImage.Height;
                }
            }
            catch (Exception ex)
            {
                ImgWidth = 0;
                ImgHeight = 0;
            }
            finally
            {
                width = ImgWidth;
                height = ImgHeight;
            }
        }

        /// <summary>
        /// 取得網路圖片長寬
        /// </summary>
        /// <param name="url"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void GetWebImageWidthAndHeight(string url, out int width, out int height)
        {
            int ImgWidth = 0, ImgHeight = 0;

            try
            {
                byte[] imageData = new WebClient().DownloadData(url);
                MemoryStream imgStream = new MemoryStream(imageData);
                System.Drawing.Image img = System.Drawing.Image.FromStream(imgStream);

                ImgWidth = img.Width;
                ImgHeight = img.Height;
            }
            catch (Exception ex)
            {
                ImgWidth = 0;
                ImgHeight = 0;
            }
            finally
            {
                width = ImgWidth;
                height = ImgHeight;
            }
        }

        public static string GetColorCode(string Color)
        {
            string ColorCode = "";

            switch (Color)
            {
                case "red":
                    ColorCode = "#FF1744";
                    break;
                case "orange":
                    ColorCode = "#ef6c00";
                    break;
                case "yellow":
                    ColorCode = "#fdd835";
                    break;
                case "green":
                    ColorCode = "#43A047";
                    break;
                case "light-green":
                    ColorCode = "#8bc34a";
                    break;
                case "blue":
                    ColorCode = "#2196F3";
                    break;
                case "teal":
                    ColorCode = "#009688";
                    break;
                case "deep-purple":
                    ColorCode = "#673ab7";
                    break;
                case "gold":
                    ColorCode = "#ac7224";
                    break;
                case "light-grey":
                    ColorCode = "#bdbdbd";
                    break;
                case "grey":
                    ColorCode = "#616161";
                    break;
                case "black":
                    ColorCode = "#000000";
                    break;
                default:
                    ColorCode = "#000000";
                    break;
            }

            return ColorCode;
        }
    }
}