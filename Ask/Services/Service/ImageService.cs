using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using WorkV3.Services.Interface;

namespace WorkV3.Services.Service
{
    public class ImageService: IImageService
    {
        public void GetImageWidthAndHeight(string source, out int width, out int height)
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
        public void GetLocalImageWidthAndHeight(string path, out int width, out int height)
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
        public void GetWebImageWidthAndHeight(string url, out int width, out int height)
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
    }
}