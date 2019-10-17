using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using WorkV3.Models.Interface;

namespace WorkV3.Models.Repository
{
    public class PointRepository : IPointRepository<float>
    {
        private IMemberShipRepository<MemberShipModels> membershipRepository;

        public PointRepository(IMemberShipRepository<MemberShipModels> inMembershipRepository)
        {
            membershipRepository = inMembershipRepository;
        }

        /// <summary>
        /// 取得指定會員點數
        /// </summary>
        /// <param name="membershipId"></param>
        /// <returns></returns>
        public float GetPoint(long membershipId)
        {
            MemberShipModels member = membershipRepository.GetItem(membershipId);
            return member.Point;
        }

        /// <summary>
        /// 轉移點數
        /// </summary>
        /// <param name="membershipIdFrom"></param>
        /// <param name="membershipIdTo"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool TransferPoint(long membershipIdFrom, long membershipIdTo, float point)
        {
            try
            {
                MemberShipModels memberFrom = membershipRepository.GetItem(membershipIdFrom);
                MemberShipModels memberTo = membershipRepository.GetItem(membershipIdTo);

                // 因未規劃手續費，故先忽略
                if (memberFrom.Point < point)
                    throw new Exception("點數不足");

                memberFrom.Point -= point;
                memberTo.Point += point;

                // 這邊應該需要做 transaction ，之後再研究好了
                membershipRepository.UpdateItem(memberFrom, new string[] { "Point" });
                membershipRepository.UpdateItem(memberTo, new string[] { "Point" });
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 增加點數
        /// </summary>
        /// <param name="membershipId"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool AddPoint(long membershipId, float point)
        {
            try
            {
                MemberShipModels member = membershipRepository.GetItem(membershipId);
                member.Point += point;
                membershipRepository.UpdateItem(member, new string[] { "Point" });
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 確認點數是否足夠
        /// </summary>
        /// <param name="membershipId"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsPointEnough(long membershipId, float point)
        {
            try
            {
                MemberShipModels member = membershipRepository.GetItem(membershipId);
                return member.Point >= point;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 減少點數
        /// </summary>
        /// <param name="membershipId"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool SubtractPoint(long membershipId, float point)
        {
            try
            {
                MemberShipModels member = membershipRepository.GetItem(membershipId);

                if (member.Point < point)
                    throw new Exception("點數不足");

                member.Point -= point;
                membershipRepository.UpdateItem(member, new string[] { "Point" });
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}