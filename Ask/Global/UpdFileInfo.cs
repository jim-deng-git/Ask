using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkLib;
using WorkV3.Models;
using WorkV3.Models.DataAccess;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;

namespace WorkV3.Golbal
{
    public class UpdFileInfo
    {

        private static object lockObj = new object();

        #region BySiteID

        /// <summary>
        /// 取得實體上傳目錄by SiteID
        /// </summary>
        /// <param name="SiteID"></param>
        /// <returns></returns>
        public static string GetUPathBySiteID(long SiteID, string CustomFolder = "")
        {
            string Path = GetItem.UpdPath();
            SitesModels datas = SitesDAO.GetInfo(SiteID);
            if (datas != null)
                Path += "\\" + datas.SN + "\\";

            if (CustomFolder != "")
                Path += CustomFolder + "\\";

            return Path;
        }

        /// <summary>
        /// 取得瀏覽目錄by SiteID
        /// </summary>
        /// <param name="SiteID"></param>
        /// <returns></returns>
        public static string GetVPathBySiteID(long SiteID, string CustomFolder = "") {
            string Path = GetItem.ViewUpdUrl().TrimEnd('/');
            SitesModels datas = SitesDAO.GetInfo(SiteID);
            if (datas != null)
                Path += "/" + datas.SN + "/";
    
            if (CustomFolder != "")
                Path += CustomFolder + "/";

            return Path;
        }

        #endregion

        #region ByMenuID
        /// <summary>
        /// 取得實體上傳目錄by SiteID
        /// </summary>
        /// <param name="MenuID"></param>
        /// <param name="SiteID"></param>
        /// <returns></returns>
        public static string GetUPathByMenuID(long SiteID,long MenuID)
        {
            string Path = GetItem.UpdPath();
            MenusModels datas = MenusDAO.GetInfo(SiteID, MenuID);
            if (datas != null)
                Path += "\\"+ datas.SiteSN +"\\"+ datas.SN + "\\";

            return Path;
        }

        /// <summary>
        /// 取得瀏覽目錄by SiteID
        /// </summary>
        /// <param name="SiteID"></param>
        /// <param name="MenuID"></param>
        /// <returns></returns>
        public static string GetVPathByMenuID(long SiteID, long MenuID)
        {
            string Path = GetItem.ViewUpdUrl().TrimEnd('/');
            MenusModels datas = MenusDAO.GetInfo(SiteID, MenuID);
            if (datas != null)
                Path += "/" + datas.SiteSN + "/" + datas.SN + "/";

            return Path;
        }
        #endregion

        #region 上傳檔案-存檔

        /// <summary>
        /// 自訂目錄存檔
        /// </summary>
        /// <param name="updFiles"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string SaveFiles(HttpPostedFileBase updFiles, string path, string Base64="", string Base64_Resize = "")
        {

            if (updFiles.ContentLength == 0)
                return null;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string oFileName = Path.GetFileName(updFiles.FileName);

            string fileName;
            if (File.Exists(path + "\\" + oFileName))
            {
                fileName = Path.GetFileNameWithoutExtension(path + "\\" + oFileName);
                lock (lockObj)
                {
                    string suffix = Regex.Match(updFiles.FileName, @"\.\w+$").Value;
                    fileName += "_" + GetItem.NewSN() + suffix;
                }
            }
            else
            {
                fileName = oFileName;
            }
            string saveFileFullPath = path + "\\" + fileName;
            if (!string.IsNullOrEmpty(Base64)
               && FileExtensionsType.Base64_Pic.Contains(Path.GetExtension(fileName).Trim('.').ToLower()))
            {
                string baseString64 = Base64.Split(',')[1];
                var bytes = Convert.FromBase64String(baseString64);
                using (var imageFile = new FileStream(saveFileFullPath, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                    imageFile.Dispose();
                }
            }
            else
            {
                //圖片轉向 先註解
                //try
                //{
                //    Image img = Image.FromStream(updFiles.InputStream);

                //    foreach (var prop in img.PropertyItems)
                //    {
                //        if (prop.Id == 0x0112) //value of EXIF
                //        {
                //            int orientationValue = img.GetPropertyItem(prop.Id).Value[0];
                //            RotateFlipType rotateFlipType = GetOrientationToFlipType(orientationValue);
                //            img.RotateFlip(rotateFlipType);
                //            break;
                //        }
                //    }
                //    img.Save(saveFileFullPath);
                //}
                //catch
                //{
                    updFiles.SaveAs(saveFileFullPath);
                //}
            }
            string saveResizeFileFullPath = path + "\\R_" + fileName;
            if (!string.IsNullOrEmpty(Base64_Resize)
               && FileExtensionsType.Base64_Pic.Contains(Path.GetExtension(fileName).Trim('.').ToLower()))
            {
                string baseString64 = Base64_Resize.Split(',')[1];
                var bytes = Convert.FromBase64String(baseString64);
                using (var imageFile = new FileStream(saveResizeFileFullPath, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                    imageFile.Dispose();
                }
            }
            else
            {
                try
                {
                    Image img = Image.FromStream(updFiles.InputStream);

                    foreach (var prop in img.PropertyItems)
                    {
                        if (prop.Id == 0x0112) //value of EXIF
                        {
                            int orientationValue = img.GetPropertyItem(prop.Id).Value[0];
                            RotateFlipType rotateFlipType = GetOrientationToFlipType(orientationValue);
                            img.RotateFlip(rotateFlipType);
                            break;
                        }
                    }
                    img.Save(saveFileFullPath);
                }
                catch
                {
                    updFiles.SaveAs(saveResizeFileFullPath);
                }
            }
            //updFiles.SaveAs(path + "\\" + fileName);
            return fileName;
        }

        /// <summary>
        /// 依網站資料夾存檔
        /// </summary>
        /// <param name="updFiles"></param>
        /// <param name="SiteID"></param>
        /// <returns></returns>
        public static string SaveFilesBySiteID(HttpPostedFileBase updFiles, long SiteID, string CustomFolder = "")
        {
            if (updFiles.ContentLength == 0)
                return null;

            string path = GetUPathBySiteID(SiteID, CustomFolder);
            string fileName = SaveFiles(updFiles, path);
            return fileName;
        }
        /// <summary>
        /// 依網站資料夾存檔
        /// </summary>
        /// <param name="updFiles"></param>
        /// <param name="SiteID"></param>
        /// <returns></returns>
        public static string SaveFilesBySiteID(HttpPostedFileBase updFiles, long SiteID, string CustomFolder = "", string Base64 = "", string Base64_Resize = "")
        {
            if (updFiles.ContentLength == 0)
                return null;

            string path = GetUPathBySiteID(SiteID, CustomFolder);
            string fileName = SaveFiles(updFiles, path, Base64, Base64_Resize);
            return fileName;
        }
        /// <summary>
        /// 依網站選單目錄存檔
        /// </summary>
        /// <param name="updFiles"></param>
        /// <param name="SiteID"></param>
        /// <param name="MenuID"></param>
        /// <returns></returns>
        public static string SaveFilesByMenuID(HttpPostedFileBase updFiles, long SiteID, long MenuID, string Base64 = "", string Base64_Resize = "")
        {

            if (updFiles.ContentLength == 0)
                return null;

            string path = GetUPathByMenuID(SiteID, MenuID);
            string fileName = SaveFiles(updFiles, path, Base64, Base64_Resize);
            return fileName;
        }        
        #endregion

        #region 允許檔案副檔名
        public static class FileExtensionsType
        {
            /// <summary>
            /// 所有網頁合法格式
            /// </summary>
            public static string All = "jpg,png,gif,bmp,xls,xlsx,ppt,pptx,doc,docx,pdf,text/plain,txt,pps,ppsx,zip,rar,7z,mov,flv,mpg,mp4,wmv,avi,mp3";
            /// <summary>
            /// 圖片(可轉換 Base 64 的圖片)
            /// </summary>
            public static string Base64_Pic = "jpg,png,bmp";
            /// <summary>
            /// 圖片
            /// </summary>
            public static string Pic = "jpg,png,gif,bmp";
            /// <summary>
            /// 文件檔
            /// </summary>
            public static string Doc = "xls,xlsx,ppt,pptx,doc,docx,pdf,text/plain,txt,pps,ppsx";
            /// <summary>
            /// 試算表(EXCEL)
            /// </summary>
            public static string Xls = "xls,xlsx";
            /// <summary>
            /// 壓縮檔
            /// </summary>
            public static string ZIP = "zip,rar,7z";
            /// <summary>
            /// PDF
            /// </summary>
            public static string PDF = "pdf";
            /// <summary>
            /// 串流檔MP4
            /// </summary>
            public static string MP4 = "mp4";
            /// <summary>
            /// 影片檔
            /// </summary>
            public static string Video = "mov,flv,mpg,mp4,wmv,avi";
            /// <summary>
            /// 聲音檔
            /// </summary>
            public static string Sound = "mp3";
            /// <summary>
            /// 後台自訂
            /// </summary>
            public static string Custom = "jpg,png,gif,bmp,pdf,doc,docx,xlsx,xls,ppt,pptx,zip,rar";
        }
        #endregion

        #region (Jqurey)fileuploader-files
        //[{"name":"young-woman-1745173_640.jpg","type":"image\/jpeg","size":87399,"file":"uploads\/young-woman-1745173_640.jpg","data":{"url":"http:\/\/localhost\/fileuploader\/examples\/appended-files\/uploads\/young-woman-1745173_640.jpg"}}]
        public class fileuploaderFile
        {
            public string name { get; set; }
            public string type { get; set; }
            public string size { get; set; }
            public string file { get; set; }
            //public string data { get; set; }
            /// <summary>
            ///資料的PK, 用於刪除檔案(by jim 20170707)
            /// </summary>
            public string data { get; set; }

        }
        #endregion

        public static RotateFlipType GetOrientationToFlipType(int orientationValue)
        {
            RotateFlipType rotateFlipType = RotateFlipType.RotateNoneFlipNone;

            switch (orientationValue)
            {
                case 1:
                    rotateFlipType = RotateFlipType.RotateNoneFlipNone;
                    break;
                case 2:
                    rotateFlipType = RotateFlipType.RotateNoneFlipX;
                    break;
                case 3:
                    rotateFlipType = RotateFlipType.Rotate180FlipNone;
                    break;
                case 4:
                    rotateFlipType = RotateFlipType.Rotate180FlipX;
                    break;
                case 5:
                    rotateFlipType = RotateFlipType.Rotate90FlipX;
                    break;
                case 6:
                    rotateFlipType = RotateFlipType.Rotate90FlipNone;
                    break;
                case 7:
                    rotateFlipType = RotateFlipType.Rotate270FlipX;
                    break;
                case 8:
                    rotateFlipType = RotateFlipType.Rotate270FlipNone;
                    break;
                default:
                    rotateFlipType = RotateFlipType.RotateNoneFlipNone;
                    break;
            }

            return rotateFlipType;
        }
    }
}