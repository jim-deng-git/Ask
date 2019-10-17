using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class MemberShipSettingDAO
    {
        private static string statementModuleName = "Member";
        public static List<CategoryModels> GetSelectedItems(long MemberShipID, string Type)
        {
            List<CategoryModels> items = new List<CategoryModels>();

            string sql = $"SELECT * FROM Categories WHERE  ID IN (SELECT CategoryID FROM MemberShipSetting WHERE MemberShipID=" + MemberShipID.ToString() + " AND Type='" + Type.Replace("'", "") + "') ORDER BY Sort ";
           
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            foreach (DataRow dr in datas.Rows)
            {
                CategoryModels m = new CategoryModels();
                m.ID = (long)dr["ID"];
                m.Type = dr["Type"].ToString().Trim();
                m.Title = dr["Title"].ToString().Trim();
                m.RemarkText = dr["RemarkText"].ToString().Trim();
                m.ShowStatus = Convert.ToBoolean(dr["ShowStatus"].ToString());
                m.Icon = dr["Icon"].ToString().Trim();
                m.Sort = Convert.ToInt32(dr["Sort"].ToString());
                items.Add(m);
            }

            return items;
        }
        public static bool SetSelectedItems(long MemberShipID, string Type, List<string> CategoryIDList)
        {
            string sel_sql = $"SELECT * FROM MemberShipSetting WHERE  MemberShipID={MemberShipID.ToString()} AND Type='{ Type.Replace(",", "','") }' ";
            
            string addIDList = "", delIDList = "";
            List<string> existList = new List<string>();
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sel_sql);
            if (datas != null && datas.Rows.Count > 0)
            {
                foreach (DataRow row in datas.Rows)
                {
                    if (CategoryIDList.Contains(row["CategoryID"].ToString()))
                    {
                        existList.Add(row["CategoryID"].ToString());
                        continue;
                    }
                    else
                    {
                        delIDList += row["CategoryID"].ToString() + ",";
                    }
                }
            }

            foreach (string categoryID in CategoryIDList)
            {
                if(!existList.Contains(categoryID))
                    addIDList += categoryID + ",";
            }
            addIDList = addIDList.Trim(',');
            delIDList = delIDList.Trim(',');
            string exeCommandStr = "";
            if (!string.IsNullOrEmpty(delIDList))
            {
                exeCommandStr = $" DELETE MemberShipSetting WHERE MemberShipID={MemberShipID.ToString()} AND Type='{ Type.Replace("'", "") }' AND CategoryID IN ({delIDList});";
            }
            if (!string.IsNullOrEmpty(addIDList))
            {
                string[] addIDs = addIDList.Split(',');
                foreach (string id in addIDs)
                {
                    exeCommandStr += $"INSERT INTO MemberShipSetting (MemberShipID, Type, CategoryID, CreateTime, ModifyTime) VALUES ({MemberShipID.ToString()}, '{ Type.Replace("'", "") }', {id}, GETDATE(), GETDATE()); ";
                }
            }
            if (!string.IsNullOrEmpty(exeCommandStr))
            {
                int exeCount = db.ExecuteNonQuery(exeCommandStr);
            }
            return true;
        }
        public static AgreeStatementSetModels GetStatementItems()
        {
            string sel_sql = $"SELECT * FROM AgreeStatementSet WHERE  ModuleName='{ statementModuleName.Replace("'", "")}' ";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                var qResult =  conn.Query<AgreeStatementSetModels>(sel_sql);
                if (qResult != null && qResult.Count() > 0)
                    return qResult.First();
            }
            return null;
        }
        public static bool SetStatementItems(AgreeStatementSetModels item)
        {
            item.ModuleName = statementModuleName;
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("AgreeStatementSet");
            tableObj.GetDataFromObject(item);
            DateTime now = DateTime.Now;
            string sql = $"SELECT * FROM AgreeStatementSet WHERE  ModuleName='{ statementModuleName.Replace("'", "")}' ";
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = now;
                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = now;
                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ModuleName");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");
                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = now;
                tableObj.Update(item.ModuleName);
            }
            return true;
        }
        public static MailTemplateSetModels GetMailTemplateItems(string TemplateName)
        {
            string sel_sql = $"SELECT * FROM MailTemplateSet WHERE  TemplateName='{ TemplateName.Replace("'", "")}' ";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                var qResult = conn.Query<MailTemplateSetModels>(sel_sql);
                if (qResult != null && qResult.Count() > 0)
                {
                    MailTemplateSetModels model = qResult.First();
                    model.AttShowFiles = "";
                    if (!string.IsNullOrEmpty(model.AttFiles))
                    {
                        List<ViewModels.MemberShipSetFile> fileShowList = new List<ViewModels.MemberShipSetFile>();
                        List<WorkV3.Models.ResourceFilesModels> fileList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WorkV3.Models.ResourceFilesModels>>(model.AttFiles);
                        foreach (WorkV3.Models.ResourceFilesModels fileModel in fileList)
                        {
                            ViewModels.MemberShipSetFile showFile = new ViewModels.MemberShipSetFile()
                            {
                                FileInfo = fileModel.FileInfo,
                                FileSize = fileModel.FileSize.HasValue ? fileModel.FileSize.Value : 0,
                                FileSizeDesc = "0",
                                ShowName = fileModel.ShowName
                            };
                            showFile.FileSizeDesc = WorkLib.uFiles.SizeToText((long)showFile.FileSize);
                            fileShowList.Add(showFile);
                        }
                        model.AttShowFiles = Newtonsoft.Json.JsonConvert.SerializeObject(fileShowList);
                    }
                    return model;
                }
                else
                {
                    MailTemplateSetModels item = new MailTemplateSetModels();
                    item.TemplateName = TemplateName;
                    System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
                    xmldoc.Load(HttpContext.Current.Server.MapPath("~/App_Data/mailContent/MailTemplates.xml"));
                    System.Xml.XmlNode templateNode = xmldoc.SelectSingleNode(string.Format("/Mails/Template[@ID='{0}']", TemplateName));

                    item.MailTitle = templateNode.SelectSingleNode("Subject").InnerXml;
                    item.MailContent = templateNode.SelectSingleNode("Content").InnerXml;
                    item.MailFromName = (string)WorkLib.GetItem.appSet("MailSender");
                    item.MailFromAddress = (string)WorkLib.GetItem.appSet("MailFrom");
                    item.AttFiles = "";
                    SQLData.Database db = new SQLData.Database(WebInfo.Conn);
                    SQLData.TableObject tableObj = db.GetTableObject("MailTemplateSet");
                    tableObj.GetDataFromObject(item);
                    if (Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent != null)
                        tableObj["Creator"] = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                    else
                        tableObj["Creator"] = 0;
                    tableObj["CreateTime"] = DateTime.Now;
                    if (Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent != null)
                        tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                    else
                        tableObj["Modifier"] = 0;
                    tableObj["ModifyTime"] = DateTime.Now;
                    tableObj.Insert();
                    return item;
                }
            }
            //return new MailTemplateSetModels();
        }
        public static bool SetMailTemplateItems(MailTemplateSetModels item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            item.AttFiles = item.AttFiles == null ? "" : item.AttFiles;
            SQLData.TableObject tableObj = db.GetTableObject("MailTemplateSet");
            tableObj.GetDataFromObject(item);
            DateTime now = DateTime.Now;
            string sql = $"SELECT * FROM MailTemplateSet WHERE  TemplateName='{ item.TemplateName.Replace("'", "")}' ";
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = now;
                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = now;
                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("TemplateName");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");
                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = now;
                tableObj.Update(item.TemplateName);
            }
            return true;
        }

        public static MemberShipSettingModel GetMemberSetItem(long MemberShipID, string Type)
        {
            string sql = $"select top 1 * from MemberShipSetting where MemberShipID = {MemberShipID} and Type = '{Type.Replace("'", "")}'";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                return conn.Query<MemberShipSettingModel>(sql).SingleOrDefault();
            }
        }
    }
}