using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTuberMusic.Network.GetTools;
using VTuberMusic.Tools;

namespace VTuberMusic.Modules
{
    class Banner
    {
        public string Id { get; set; }
        public string CreateTime { get; set; }
        public string CreatorId { get; set; }
        public bool Deleted { get; set; }
        public string CreatorRealName { get; set; }
        public string OriginName { get; set; }
        public string BannerImg { get; set; }
        public string Url { get; set; }

        /// <summary>
        /// 获取横幅
        /// </summary>
        public static Banner[] GetBanners()
        {
            string postJson = JsonMapper.ToJson(new GetModules.ListPostModule { pageIndex = 1,pageRows= 20, sortField="Id",sortType="desc",search = new GetModules.Search() });
            var response = GetTools.PostApi("/v1/GetDataList", postJson); // 请求 Api
            if (response.IsSuccessful)
            {
                GetModules.BannerGetModule jsonData = JsonMapper.ToObject<GetModules.BannerGetModule>(response.Content); // 转 Json 为对象
                if (jsonData.Success)
                {
                    return jsonData.Data;
                }
                else
                {
                    Log.WriteLine("请求失败:\r\n" + response.Content, Level.Error);
                    throw new Exception(jsonData.Msg.ToString());
                }
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }
    }
}
