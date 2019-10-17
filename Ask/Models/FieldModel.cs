using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    /// <summary>
    /// 欄位 資料模型
    /// </summary>
    [Table(Name = "Field")]
    public class FieldModel {
        /// <summary>
        /// 欄位識別碼
        /// </summary>
        [PKey]
        public long ID { get; set; }

        /// <summary>
        /// 父層識別碼
        /// </summary>
        public long ParentID { get; set; }

        /// <summary>
        /// 序號
        /// </summary>
        public int SN { get; set; }
                
        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 是否可刪除
        /// </summary>
        public bool IsRemove { get; set; } = true;

        /// <summary>
        /// 欄位預設類型 1.Email 2.電話 3.姓名 4.性別
        /// </summary>
        public int? DefaultType { get; set; }

        /// <summary>
        /// 欄位類型識別碼
        /// </summary>
        public string TypeID { get; set; }

        /// <summary>
        /// 欄位名稱
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 是否欄位說明
        /// </summary>
        public bool IsDescription { get; set; }

        /// <summary>
        /// 欄位說明（分隔線樣式）
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 提示文字（分隔線線上文字）
        /// </summary>
        public string Hint { get; set; }

        /// <summary>
        /// 欄位寬度 1.25% 2.50% 3.100%
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 欄位高度 (多行填寫框)
        /// </summary>
        public int? High { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool Requied { get; set; }

        /// <summary>
        /// 是否檢查格式
        /// </summary>
        public bool Fomat { get; set; }

        /// <summary>
        /// 檢查格式類型識別碼
        /// </summary>
        public int? FomatType { get; set; }

        /// <summary>
        /// 是否檢查字數
        /// </summary>
        public bool WordLimit { get; set; }

        /// <summary>
        /// 檢查字數下限
        /// </summary>
        public int? WordMin { get; set; }

        /// <summary>
        /// 檢查字數上限
        /// </summary>
        public int? WordMax { get; set; }

        /// <summary>
        /// 選項排列 0:垂直排列 1:水平排列 
        /// </summary>
        public bool OptionArray { get; set; } = true;

        /// <summary>
        /// 選項
        /// </summary>
        public string Options { get; set; }

        /// <summary>
        /// 是否有其他選項
        /// </summary>
        public bool OtherOprion { get; set; }

        /// <summary>
        /// 圖片
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 連結
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// 是否另開
        /// </summary>
        public bool IsOpenNew { get; set; }

        /// <summary>
        /// 檢查檔案大小
        /// </summary>
        public bool FileSize { get; set; }

        /// <summary>
        /// 檢查檔案大小類型識別碼
        /// </summary>
        public int? FileSizeType { get; set; }

        /// <summary>
        /// 檔案數量
        /// </summary>
        public bool FileNumber { get; set; }

        /// <summary>
        /// 檔案數量下限
        /// </summary>
        public int? FileNumberMin { get; set; }

        /// <summary>
        /// 檔案數量上限
        /// </summary>
        public int? FileNumberMax { get; set; }

        /// <summary>
        /// 提供範本
        /// </summary>
        public bool HasTemplate { get; set; }

        /// <summary>
        /// 檔案上傳範本
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// 檢查地址可視範圍
        /// </summary>
        public bool Range { get; set; }

        /// <summary>
        /// 可視範圍限制 1:國家 2.省/州 3.縣/市
        /// </summary>
        public int? RangeLevel { get; set; }

        /// <summary>
        /// 可視範圍限制
        /// </summary>
        public int? RangeLimit { get; set; }

        /// <summary>
        /// 是否在列表中顯示該欄位值
        /// </summary>
        public bool ShowInList { get; set; }

        // 該欄位是否用於報到識別
        public bool IsRegisterField { get; set; }

        /// <summary>
        /// 重復限制 0:不可重複 1:可重複數次 2:可任意報名
        /// </summary>
        public byte? RepeatLimit { get; set; }

        /// <summary>
        /// 當 RepeatLimit 為可重複數次時，該欄位指定可重複的次數
        /// </summary>
        public int? RepeatCount { get; set; }

        /// <summary>
        /// 搭配類型 (image、video)
        /// </summary>
        public string Match { get; set; }

        /// <summary>
        /// 影片類型(youtube、vimeo)
        /// </summary>
        public string VideoType { get; set; }

        /// <summary>
        /// 自行上傳影片
        /// </summary>
        public string Video { get; set; }

        /// <summary>
        /// 影片截圖
        /// </summary>
        public string VideoPhoto { get; set; }

        /// <summary>
        /// 影片自行上傳圖片
        /// </summary>
        public string VideoCustomPhoto { get; set; }

        /// <summary>
        /// 是否自行上傳圖片
        /// </summary>
        public bool IsVideoPhotoCustom { get; set; }

        /// <summary>
        /// 影片ID
        /// </summary>
        public string VideoID { get; set; }

        public FieldValue Value { get; set; }
        
        public string GetDesignUploadUrl() {
            FormModel form = FormDAO.GetItem(ParentID);
            return Golbal.UpdFileInfo.GetVPathBySiteID((long)form.SiteID, "FormDesign").TrimEnd('/');
        }

        public int[] GetRangeHierarchy() {
            if (!Range || RangeLimit == null)
                return null;
            return DataAccess.WorldRegionDAO.GetRegionHierarchy((int)RangeLimit);
        }
    }

    public class FieldDesignItem {
        public string ID { get; set; }
        public bool Enable { get; set; }
    }

    public class FieldAddress {
        public int[] Regions { get; set; }
        public string Address { get; set; }

        public string GetAddress() {
            return FieldDAO.GetAddressFieldValue(this);
        }
    }
}