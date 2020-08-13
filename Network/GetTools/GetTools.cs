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
using VTuberMusic.Tools;

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
        /// <returns>Post 失败会返回空白信息</returns>
        public static string PostApi(string Uri, string Raw)
        {
            var client = new RestClient(ApiUri);
            var request = new RestRequest(Uri, DataFormat.Json).AddJsonBody(Raw);
            var response = client.Post(request);
            if (response.IsSuccessful)
            {
                return response.Content;
            }
            else
            {
                Log.WriteLine("POST " + ApiUri + Uri + " 发生错误 错误信息: \r\n" + response.ErrorException, Level.Error);
                return response.Content;
            }
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
            JsonData jsonData = JsonMapper.ToObject(PostApi("/v1/GetCDNList", "{\"pageIndex\": 1,\"pageRows\": 999}"));
            string[] CDNList = new string[20];
            for (int i = (int)jsonData["Total"] - 1; i != -1; i--)
            {
                CDNList[int.Parse((string)jsonData["Data"][i]["name"])] = (string)jsonData["Data"][i]["url"];
            }
            Log.WriteLine("[Net]已成功获取 CDN 列表", Level.Info);
            return CDNList;
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
