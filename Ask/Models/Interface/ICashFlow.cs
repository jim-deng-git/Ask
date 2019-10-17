using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WorkV3.Models.Interface
{
    public interface ICashFlow<TGetAuthResponse, TGetAuthParam, 
                               TCancelAuthResponse, TCancelAuthParam,
                               TSettleResponse, TSettleParam,
                               TCancelSettleResponse, TCancelSettleParam,
                               TRefundResponse, TRefundParam,
                               TCancelRefundResponse, TCancelRefundParam,
                               TQueryResponse, TQueryParam>
    {
        /// <summary>
        /// 取得授權
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        TGetAuthResponse GetAuth(TGetAuthParam param);

        /// <summary>
        /// 取消授權
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        TCancelAuthResponse CancelAuth(TCancelAuthParam param);

        /// <summary>
        /// 請款交易
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        TSettleResponse Settle(TSettleParam param);

        /// <summary>
        /// 取消請款
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        TCancelSettleResponse CancelSettle(TCancelSettleParam param);

        /// <summary>
        /// 退貨交易
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        TRefundResponse Refund(TRefundParam param);

        /// <summary>
        /// 取消退貨
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        TCancelRefundResponse CancelRefund(TCancelRefundParam param);

        /// <summary>
        /// 查詢交易
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        TQueryResponse Query(TQueryParam param);
    }
}