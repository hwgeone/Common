using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HttpHelper
{
    public class RestHelper
    {
         private RestClient _httpClient;
        private string _baseIPAddress;

        /// <param name="ipaddress">请求的基础IP，例如：http://192.168.0.33:8080/ </param>
        public RestHelper(string ipaddress = "")
        {
            _httpClient = new RestClient();
            _httpClient.BaseUrl = new Uri(ipaddress);
            _httpClient.Proxy = null;
        }

        /// <summary>
        /// Get请求数据
        ///   /// <para>最终以url参数的方式提交</para>
        /// <para>yubaolee 2016-3-3 重构与post同样异步调用</para>
        /// </summary>
        /// <param name="parameters">参数字典,可为空</param>
        /// <param name="requestUri">例如/api/Files/UploadFile</param>
        /// <returns></returns>
        public string Get(Dictionary<string, string> parameters, string requestUri)
        {
            if (parameters != null)
            {
              
            }
            else
            {
               
            }

            var request = new RestRequest(requestUri, Method.GET);

            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

            var response = _httpClient.Execute(request);

            return response.Content;
        }

        /// <summary>
        /// Get请求数据
        /// <para>最终以url参数的方式提交</para>
        /// </summary>
        /// <param name="parameters">参数字典</param>
        /// <param name="requestUri">例如/api/Files/UploadFile</param>
        /// <returns>实体对象</returns>
        public T Get<T>(Dictionary<string, string> parameters, string requestUri) where T : class
        {
            string jsonString = Get(parameters, requestUri);
            if (string.IsNullOrEmpty(jsonString))
                return null;

            T obj = JsonUtils.Instance.Deserialize<T>(jsonString);
            return obj;
        }

        public T Get<T>(Dictionary<string, string> parameters, string requestUri, JsonSerializerSettings settings) where T : class
        {
            string jsonString = Get(parameters, requestUri);
            if (string.IsNullOrEmpty(jsonString))
                return null;

            T obj = JsonUtils.Instance.DeserializeBySetting<T>(jsonString, settings);
            return obj;
        }

        /// <summary>
        /// 以json的方式Post数据 返回string类型
        /// <para>最终以json的方式放置在http体中</para>
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="requestUri">例如/api/Files/UploadFile</param>
        /// <returns></returns>
        public string Post(object entity, string requestUri)
        {
            var request = new RestRequest(requestUri, Method.POST);
            RestSharp.Serializers.JsonSerializer serializer = new RestSharp.Serializers.JsonSerializer();
            // Json to post.
            string jsonToSend = serializer.Serialize(entity);

            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            try
            {
                var response = _httpClient.Execute(request);
                return response.Content;
            }
            catch (Exception error)
            {
                throw error;
            }
        }
    }
}
