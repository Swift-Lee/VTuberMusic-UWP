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
        /// 获取横幅，当网络错误获取失败时会抛出异常
        /// </summary>
        public static Banner[] GetBanners()
        {
            string postJson = JsonMapper.ToJson(new GetModules.ListPostModule { pageIndex = 1,pageRows= 20, sortField="Id",sortType="desc",search = new GetModules.Search() });
            string jsonText = GetTools.PostApi("/v1/GetDataList", postJson); // 请求 Api
            GetModules.BannerGetModule jsonData = JsonMapper.ToObject<GetModules.BannerGetModule>(jsonText); // 转 Json 为对象
            return jsonData.Data;
        }
    }
}
