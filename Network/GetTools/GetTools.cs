using LitJson;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VTuberMusic.Modules;
using VTuberMusic.Tools;
using Windows.UI.Xaml.Documents;

namespace VTuberMusic.Network.GetTools
{
    class GetTools
    {
        public static string ApiUri { get; set; } = "https://api.vtbmusic.com:60006";
        public static string[] CDNList { get; set; }

        /// <summary>
        /// Post Api
        /// </summary>
        /// <param name="Uri">请求 Uri</param>
        /// <param name="Raw">请求内容</param>
        /// <returns>向 Api 发送 Post 请求</returns>
        public static IRestResponse PostApi(string Uri, string Raw)
        {
            var client = new RestClient(ApiUri);
            var request = new RestRequest(Uri, DataFormat.Json).AddJsonBody(Raw);
            return client.Post(request);
        }

        /// <summary>
        /// 获取一言
        /// </summary>
        /// <returns>获取失败会返回 "无法获取一言"</returns>
        public static string GetHitokoto()
        {
            var client = new RestClient("https://v1.hitokoto.cn");
            var request = new RestRequest();
            var response = client.Get(request);
            if (response.IsSuccessful)
            {
                JsonData jsonData = JsonMapper.ToObject(response.Content); // 转 Json 为对象
                try
                {
                    return (string)jsonData["hitokoto"] + "  ——" + (string)jsonData["from"] + " 来自 hitokoto.cn";
                }
                catch
                {
                    return ("无法获取一言");
                }
            }
            else
            {
                Log.WriteLine("GET https://v1.hitokoto.cn 发生错误 错误信息: \r\n" + response.ErrorException, Level.Error);
                return "无法获取一言";
            }
        }

        /// <summary>
        /// 获取 CDN 列表
        /// </summary>
        /// <returns>获取失败会抛出异常</returns>
        public static string[] GetCDNList()
        {
            var response = PostApi("/v1/GetCDNList", "{\"pageIndex\": 1,\"pageRows\": 999}");
            if (response.IsSuccessful)
            {
                JsonData jsonData = JsonMapper.ToObject(response.Content);
                if ((bool)jsonData["Success"])
                {
                    string[] CDNList = new string[20];
                    for (int i = (int)jsonData["Total"] - 1; i != -1; i--)
                    {
                        CDNList[int.Parse((string)jsonData["Data"][i]["name"])] = (string)jsonData["Data"][i]["url"];
                    }
                    Log.WriteLine("[Net]已成功获取 CDN 列表", Level.Info);
                    return CDNList;
                }
                else
                {
                    Log.WriteLine("请求失败:\r\n" + response.Content, Level.Error);
                    throw new Exception((string)jsonData["Msg"]);
                }
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        public static string GetRequest(string Uri)
        {
            var client = new RestClient();
            var request = new RestRequest(Uri);
            var response = client.Get(request);
            
            return response.Content;
        }

        public class PostTemplate
        {
            public class GetDataList
            {
                public Search search { get; set; }
                public long pageIndex { get; set; } = 0;
                public long pageRows { get; set; } = 0;

                public class Search
                {
                    public string condition { get; set; } = "";
                    public string keyword { get; set; } = "";
                }
            }
        }
    }
}
