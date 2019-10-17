using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using WorkV3.Models.Interface;
using WorkV3.Models.Repository;
using WorkV3.ViewModels.TSCashFlow;
using Newtonsoft.Json;
using System.Xml;
using System.Security.Cryptography;
using WorkV3.Common;

namespace WorkV3.Models
{
    /// <summary>
    /// 金流物件原型
    /// </summary>
    public abstract class ApiSender
    {
        private ApiProtocol _apiRootProtocol = ApiProtocol.https;
        private string _apiRoot = "";
        private string _apiName = "";
        private ApiSendDataType _sendType;
        private Dictionary<string, string> _headerValue;
        private string _token;

        public ApiSender(string apiRoot = "", ApiProtocol protocol = ApiProtocol.https, ApiSendDataType sendType = ApiSendDataType.Json)
        {
            _apiRoot = apiRoot;
            _apiRootProtocol = protocol;
            _sendType = sendType;
            _headerValue = new Dictionary<string, string>();
        }

        public void SetApiRoot(string apiRoot)
        {
            _apiRoot = apiRoot;
        }

        public void SetApiRootProtocol(ApiProtocol protocol)
        {
            _apiRootProtocol = protocol;
        }

        public void SetApiName(string apiName)
        {
            _apiName = apiName;
        }

        public void SetApiSendContentType(ApiSendDataType sendType)
        {
            _sendType = sendType;
        }

        public void SetHeaderValue(Dictionary<string, string> param)
        {
            _headerValue = param;
        }

        public void AddHeaderValue(string name, string value)
        {
            _headerValue.Add(name, value);
        }

        public void SetToken(string token)
        {
            _token = token;
        }

        /// <summary>
        /// 呼叫 API
        /// </summary>
        /// <typeparam name="TResponse">API 回傳格式</typeparam>
        /// <typeparam name="TParam">送出參數格式</typeparam>
        /// <param name="apiName">API名稱</param>
        /// <param name="param">送出參數</param>
        /// <returns></returns>
        protected TResponse CallApi<TResponse, TParam>(string apiName, TParam param) where TResponse : new()
        {
            SetApiName(apiName);
            return GetResponse<TResponse, TParam>(param);
        }

        /// <summary>
        /// 將 param 以 json 方式送出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public HttpWebResponse SendRequest<T>(T param)
        {
            string postData = "";
            bool isTypeJson = _sendType == ApiSendDataType.Json;
            
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore; // class 裡值為 null 的話就不轉
            postData = JsonConvert.SerializeObject(param, settings);

            XmlDocument doc = new XmlDocument();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{_apiRootProtocol}://{_apiRoot}/{_apiName}");
            request.Method = "POST";
            request.ContentType = isTypeJson? "application/json": "text/xml";

            if (!isTypeJson)
            {
                doc = JsonConvert.DeserializeXmlNode(postData);
                postData = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>{doc.DocumentElement.OuterXml}";
            }

            if(_headerValue.Count > 0)
            {
                foreach(KeyValuePair<string, string> item in _headerValue)
                {
                    request.Headers[item.Key] = item.Value;
                }
            }

            // 動能驗證方式，因為有點難抽出來寫，所以先寫在這裡
            request.Headers["Signature"] = $"{_token}{postData}".Md5("").ToUpper();


            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(postData);
                streamWriter.Flush();
                streamWriter.Close();
            }

            return (HttpWebResponse)request.GetResponse();
        }

        public TResponse GetResponse<TResponse, TParam>(TParam param) where TResponse: new()
        {
            TResponse response = new TResponse();

            try
            {
                using (HttpWebResponse apiResponse = SendRequest<TParam>(param))
                {
                    using (var reader = new StreamReader(apiResponse.GetResponseStream()))
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var objText = reader.ReadToEnd();
                        response = (TResponse)js.Deserialize(objText, typeof(TResponse));
                    }
                }
            }
            catch (WebException ex)
            {
                string error = ex.Message;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

            return response;
        }
    }

    public enum ApiProtocol
    {
        http = 1,
        https
    }

    public enum ApiSendDataType
    {
        Json = 1,
        Xml
    }
}