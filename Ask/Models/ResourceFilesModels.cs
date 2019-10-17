using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Golbal;
using WorkLib;

namespace WorkV3.Models
{
    public class ResourceFilesModels
    {
        public long Id { get; set; }        
        public string FileType { get; set; }
        public string FileInfo { get; set; }
        public long? FileSize { get; set; }        
        public string Descriptions { get; set; }
        public string Detail { get; set; }        
        public string ShowName { get; set; }       

        #region Get 
        public string GetFileLink()
        {
            string desc = String.IsNullOrEmpty(Descriptions) ? Descriptions : FileInfo;
            return string.Format(
                "<a href=\"{0}\" target=\"_blank\" title=\"{1}\"><img src=\"{2}\" alt=\"\" />{1}</a>",
                GetItem.UpdPath().TrimEnd('/') + "/" + FileInfo,
                desc,
               PubFunc.GetFileIcon(FileInfo));
        }

        public long GetSize(string uPath)
        {
            string filePath = uPath.TrimEnd('\\') + "\\" + FileInfo.Replace("/", "\\");
            long FileSize = uFiles.GetFileSize(filePath);

            return FileSize;
        }

        public string GetSizeDesc()
        {
            if (uCheck.IsNumeric(FileSize) )
            {
                long TmpFileSize = (long)FileSize;
                return uFiles.SizeToText(TmpFileSize);         
            }
            else
            {
                return "";
            }
        }        
        #endregion
    }
}