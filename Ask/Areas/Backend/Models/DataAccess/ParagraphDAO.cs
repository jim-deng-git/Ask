using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Areas.Backend.Models;
using WorkLib;
using Dapper;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class ParagraphDAO
    {
        public static ParagraphModels GetItem(long id) {
            return CommonDA.GetItem<ParagraphModels>("Paragraph", id);            
        }

        public static void SetItem(ParagraphModels item) {
            item.Title = item.Title ?? string.Empty;
            item.MatchType = item.MatchType ?? string.Empty;
            item.Contents = item.Contents ?? string.Empty;
            
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("Paragraph");
            tableObj.GetDataFromObject(item);
            
            string sql = "Select 1 From Paragraph Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew) {
                tableObj.Insert();
            } else {
                tableObj.Remove("ID");
                tableObj.Remove("SourceNo");
                
                tableObj.Update(item.ID);
            }
        }

        public static int Delete(long id) {
            long[] ids = { id };
            return Delete(ids);
        }

        public static int Delete(IEnumerable<long> ids) {
            if (ids?.Count() == 0)
                return 0;

            string sql =
                "DELETE ResourceFiles WHERE SourceNo IN ({0})\n" +
                "DELETE ResourceImages WHERE SourceNo IN ({0})\n" +
                "DELETE ResourceLinks WHERE SourceNo IN ({0})\n" +
                "DELETE ResourceVideos WHERE SourceNo IN ({0})\n" +
                "DELETE ResourceVoices WHERE SourceNo IN ({0})\n" +
                "DELETE Paragraph WHERE ID IN ({0})\n";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }
        }

        public static int DeleteBySourceNo(IEnumerable<long> sourceNos) {
            if (sourceNos?.Count() == 0)
                return 0;

            string sql =
                "DELETE ResourceFiles WHERE SourceNo IN (SELECT ID FROM Paragraph WHERE SourceNo IN ({0}))\n" +
                "DELETE ResourceImages WHERE SourceNo IN (SELECT ID FROM Paragraph WHERE SourceNo IN({0}))\n" +
                "DELETE ResourceLinks WHERE SourceNo IN (SELECT ID FROM Paragraph WHERE SourceNo IN({0}))\n" +
                "DELETE ResourceVideos WHERE SourceNo IN (SELECT ID FROM Paragraph WHERE SourceNo IN({0}))\n" +
                "DELETE ResourceVoices WHERE SourceNo IN (SELECT ID FROM Paragraph WHERE SourceNo IN({0}))\n" +
                "DELETE Paragraph WHERE SourceNo IN ({0})\n";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.Execute(string.Format(sql, string.Join(", ", sourceNos)));
            }
        }

        public static void CopyAllInSourceNo(long source, long target, long? targetSiteId = null) {
            string siteId = targetSiteId == null ? "SiteID" : targetSiteId.ToString();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "SELECT ID FROM Paragraph WHERE SourceNo = " + source;
                IEnumerable<long> paragraphIds = conn.Query<long>(sql);
                foreach(long paragraphId in paragraphIds) {
                    long newParagraphId = WorkLib.GetItem.NewSN();
                    sql = "INSERT Paragraph (ID, SourceNo, Title, Contents, MatchType, ImgPos, ImgAlign, Sort) " +
                        $"SELECT { newParagraphId }, { target }, Title, Contents, MatchType, ImgPos, ImgAlign, Sort FROM Paragraph Where ID = { paragraphId }";
                    conn.Execute(sql);

                    sql = "Select ID FROM ResourceFiles Where SourceNo = " + paragraphId;
                    IEnumerable<long> resourceIds = conn.Query<long>(sql);
                    foreach(long resourceId in resourceIds) {
                        sql = "INSERT ResourceFiles(ID, SiteID, SourceNo, SourceType, Ver, AreaID, FileType, FileInfo, FileSize, FileMimeType, Descriptions, Detail, ClickType, Creator, CreateTime, Modifier, ModifyTime, Sort, ShowName, Code) " +
                            $"SELECT { WorkLib.GetItem.NewSN() }, { siteId }, { newParagraphId }, SourceType, Ver, AreaID, FileType, FileInfo, FileSize, FileMimeType, Descriptions, Detail, ClickType, Creator, CreateTime, Modifier, ModifyTime, Sort, ShowName, Code " +
                            $"FROM ResourceFiles WHERE ID = { resourceId }";
                        conn.Execute(sql);
                    }

                    sql = "SELECT ID FROM ResourceImages WHERE SourceNo = " + paragraphId;
                    resourceIds = conn.Query<long>(sql);
                    foreach(long resourceId in resourceIds) {
                        sql = "INSERT ResourceImages(ID, SiteID, SourceNo, SourceType, Ver, AreaID, Code, Img, SizeType, IsOriginalSize, MultiImgStyle, Spec, SpecDetail, Link, IsOpenNew, IsShow, Sort, Creator, CreateTime, Modifier, ModifyTime) " +
                            $"SELECT { WorkLib.GetItem.NewSN() }, { siteId }, { newParagraphId }, SourceType, Ver, AreaID , Code, Img, SizeType, IsOriginalSize, MultiImgStyle, Spec, SpecDetail, Link, IsOpenNew, IsShow, Sort, Creator, CreateTime, Modifier, ModifyTime " +
                            $"FROM ResourceImages WHERE ID = { resourceId }";
                        conn.Execute(sql);
                    }

                    sql = "SELECT ID FROM ResourceLinks WHERE SourceNo = " + paragraphId;
                    resourceIds = conn.Query<long>(sql);
                    foreach (long resourceId in resourceIds) {
                        sql = "INSERT ResourceLinks(ID, SiteID, SourceNo, SourceType, Ver, AreaID, LinkType, LinkInfo, Descriptions, Detail, ClickType, Creator, CreateTime, Modifier, ModifyTime, IsOpenNew, Sort, Code) " +
                            $"SELECT { WorkLib.GetItem.NewSN() }, { siteId }, { newParagraphId }, SourceType, Ver, AreaID, LinkType, LinkInfo, Descriptions, Detail, ClickType, Creator, CreateTime, Modifier, ModifyTime, IsOpenNew, Sort, Code " +
                            $"FROM ResourceLinks WHERE ID = { resourceId }";
                        conn.Execute(sql);
                    }

                    sql = "SELECT ID FROM ResourceVideos WHERE SourceNo = " + paragraphId;
                    resourceIds = conn.Query<long>(sql);
                    foreach (long resourceId in resourceIds) {
                        sql = "INSERT ResourceVideos(ID, SiteID, SourceNo, SourceType, Ver, AreaID, Code, Type, Link, SizeType, PlayMode, IsAuto, IsRepeat, ShowSpec, Spec, ShowDuration, Duration, ShowViews, Screenshot, Creator, CreateTime, Modifier, ModifyTime, ScreenshotIsCustom) " +
                            $"SELECT { WorkLib.GetItem.NewSN() }, { siteId }, { newParagraphId }, SourceType, Ver, AreaID, Code, Type, Link, SizeType, PlayMode, IsAuto, IsRepeat, ShowSpec, Spec, ShowDuration, Duration, ShowViews, Screenshot , Creator, CreateTime, Modifier, ModifyTime, ScreenshotIsCustom " +
                            $"FROM ResourceVideos WHERE ID = { resourceId }";
                        conn.Execute(sql);
                    }

                    sql = "SELECT ID FROM ResourceVoices WHERE SourceNo = " + paragraphId;
                    resourceIds = conn.Query<long>(sql);
                    foreach (long resourceId in resourceIds) {
                        sql = "INSERT ResourceVoices(ID, SiteID, SourceNo, SourceType, Ver, AreaID, Code, Path, ShowName, MimeType, Sort, Creator, CreateTime, Modifier, ModifyTime) " +
                            $"SELECT { WorkLib.GetItem.NewSN() }, { siteId }, { newParagraphId }, SourceType, Ver, AreaID, Code, Path, ShowName, MimeType, Sort, Creator, CreateTime, Modifier, ModifyTime " +
                            $"FROM ResourceVoices WHERE ID = { resourceId }";
                        conn.Execute(sql);
                    }
                }
            }
        }

        public static IEnumerable<ParagraphModels> GetItems(long sourceNo) {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Select * From Paragraph Where SourceNo = {0} Order By Sort";
                
                return conn.Query<ParagraphModels>(string.Format(sql, sourceNo));
            }
        }
        public static string GetMatchTypeStr(string itemName)
        {
            string result = string.Empty;
            switch (itemName)
            {
                case "img":
                    result = "圖片";
                    break;
                case "video":
                    result = "影片";
                    break;
                case "file":
                    result = "檔案";
                    break;
                case "voice":
                    result = "聲音";
                    break;
                case "lightbox":
                    result = "燈箱";
                    break;
                case "link":
                    result = "連結";
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}