using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models;
using WorkV3.Models.DataAccess;

namespace WorkV3.Common
{
    public class Member
    {
        private static readonly string key = "systemUser";
        public long ID { get; private set; }
        public string Account { get; private set; }
        public string Name { get; private set; }
        public MemberType Type { get; private set; }

        private Member() { }

        /// <summary>
        /// 登出
        /// </summary>
        public void Logout() {
            HttpContext.Current.Session.RemoveAll();
        }
                
        public static LoginStatus Login(long siteId, string account, string password, bool IsAddSession=true) {
            MemberShipModels memberItem = null;
            WorkV3.Areas.Backend.Models.MemberShipRegSetModels regSetModel =  WorkV3.Areas.Backend.Models.DataAccess.MemberShipRegSetDAO.GetItem(siteId);
            if (regSetModel != null)
            {
                if(!regSetModel.IsOpenReg)
                    return LoginStatus.用戶已禁用;
                foreach (WorkV3.Areas.Backend.Models.MemberShipLoginType ltype in regSetModel.LoginTypeList)
                {
                    switch (ltype)
                    {
                        case Areas.Backend.Models.MemberShipLoginType.InputAccount:
                            memberItem = MemberShipDAO.GetItem(siteId, account);
                            break;
                        case Areas.Backend.Models.MemberShipLoginType.Email:
                            memberItem = MemberShipDAO.GetItemByEmail(siteId, account);
                            break;
                        case Areas.Backend.Models.MemberShipLoginType.ID:
                            memberItem = MemberShipDAO.GetItemByIDCard(siteId, account);
                            break;
                        case Areas.Backend.Models.MemberShipLoginType.Mobile:
                            memberItem = MemberShipDAO.GetItemByMobile(siteId, account);
                            break;
                    }
                    if (memberItem != null)
                        break;

                }
            }
            if(memberItem == null)
                return LoginStatus.用戶名不存在;
            if (!memberItem.Status)
                return LoginStatus.用戶已禁用;

            if (memberItem.Password != password)
                return LoginStatus.密碼錯誤;

            LoginStatus lStatus = LoginStatus.用戶名不存在;

            if (string.IsNullOrEmpty(memberItem.VerifyTime) & regSetModel.VerifyType == Areas.Backend.Models.MemberShipVerifyType.Email)
                lStatus = LoginStatus.EMail未驗證;
            else if(regSetModel.VerifyType == Areas.Backend.Models.MemberShipVerifyType.Mobile)
            {
                if(!string.IsNullOrEmpty(memberItem.VerifyTime) && memberItem.MobileVerifyCode == "000000")
                {
                    lStatus = LoginStatus.成功;
                }
                else
                {
                    lStatus = LoginStatus.手機未驗證;
                }
            } 
            else
                lStatus = LoginStatus.成功;
            Member member = new Member
            {
                ID = memberItem.ID,
                Account = memberItem.Account,
                Name = memberItem.Name,
                Type = MemberType.Web
            };

            if (IsAddSession)
            {
                HttpContext.Current.Session[key] = member;
            }
            MemberShipDAO.UpdateLoginTime(member.ID);
            return lStatus;
        }
        public static void RefreshMemberSession()
        {
            Member curUser = Member.Current;
            if (curUser == null)
            {
                return;
            }
            MemberShipModels memberItem = Models.DataAccess.MemberShipDAO.GetItem(curUser.ID);
            curUser.ID = memberItem.ID;
            curUser.Account = memberItem.Account;
            curUser.Name = memberItem.Name;
            HttpContext.Current.Session[key] = curUser;
        }

        public static LoginStatus Login(long siteId, string socialID)
        {
            MemberShipModels memberItem = MemberShipDAO.GetItemBySocialID(siteId, socialID);
            if (memberItem == null)
                return LoginStatus.用戶名不存在;

            if (!memberItem.Status)
                return LoginStatus.用戶已禁用;


            Member member = new Member
            {
                ID = memberItem.ID,
                Account = memberItem.Account,
                Name = memberItem.Name,
                Type = memberItem.MemberType
            };

            HttpContext.Current.Session[key] = member;
            MemberShipDAO.UpdateLoginTime(member.ID);
            return LoginStatus.成功;
        }

        public static void ApiSetSession(MemberShipModels model)
        {
            Member member = new Member
            {
                ID = model.ID,
                Account = model.Account,
                Name = model.Name,
                Type = model.MemberType
            };

            HttpContext.Current.Session[key] = member;
        }

        /// <summary>
        /// 獲取當前登入用戶，若未登入，返回 null
        /// </summary>
        public static Member Current {
            get {
                return HttpContext.Current.Session[key] as Member;
            }
        }
        //public static string GenerateRecommandCode()
        //{
        //    int length = 6;
        //    Random rd = new Random(DateTime.Now.Millisecond);
        //    string charList = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz";
        //    string value = string.Empty;
        //    for (int i = 0; i < length; ++i)
        //        value += charList[rd.Next(charList.Length)];
        //    return value;
        //}
    }
}