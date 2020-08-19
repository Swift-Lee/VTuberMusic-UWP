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
    class Vocal
    {
        public string Id { get; set; } = "";
        public string CreateTime { get; set; } = "";
        public string CreatorId { get; set; } = "";
        public bool Deleted { get; set; } = false;
        public string OriginalName { get; set; } = "";
        public string ChineseName { get; set; } = "";
        public string JapaneseName { get; set; } = "";
        public string EnglistName { get; set; } = "";
        public string GroupsId { get; set; } = "";
        public string AvatarImg { get; set; } = "";
        public string Bilibili { get; set; } = "";
        public string YouTube { get; set; } = "";
        public string Twitter { get; set; } = "";
        public int? Watch { get; set; } = 0;
        public object introduce { get; set; } = "";

        public static Vocal GetVocalObject(string id)
        {
            string postJson = JsonMapper.ToJson(new GetModules.SinglePostModule { id = id });
            string jsonText = GetTools.PostApi("/v1/GetVtbsData", postJson); // 请求 Api
            GetModules.SingleVocalGetModule jsonData = JsonMapper.ToObject<GetModules.SingleVocalGetModule>(jsonText); // 转 Json 为对象
            if (jsonData.Data != null)
            {
                if (string.IsNullOrEmpty(jsonData.Data.OriginalName))
                {
                    jsonData.Data.OriginalName = "";
                }
                if (string.IsNullOrEmpty(jsonData.Data.JapaneseName))
                {
                    jsonData.Data.JapaneseName = "";
                }
                if (string.IsNullOrEmpty(jsonData.Data.EnglistName))
                {
                    jsonData.Data.EnglistName = "";
                }
                if (string.IsNullOrEmpty(jsonData.Data.ChineseName))
                {
                    jsonData.Data.ChineseName = "";
                }
            }
            else
            {
                return new Vocal();
            }
            return jsonData.Data;
        }

        public static Vocal[] GetVocalList(string SearchCondition, string keyword, int PageIndex, int PageRows, string sortField, string sortType)
        {
            string postJson = JsonMapper.ToJson(new GetModules.ListPostModule { pageIndex = PageIndex, pageRows = PageRows, sortField = sortField, sortType = sortType, search = new GetModules.Search { condition = SearchCondition, keyword = keyword } });
            string jsonText = GetTools.PostApi("/v1/GetVtbsList", postJson); // 请求 Api
            GetModules.VocalListGetModule jsonData = JsonMapper.ToObject<GetModules.VocalListGetModule>(jsonText); // 转 Json 为对象
            if (jsonData.Total != 0)
            {
                for (int i = 0; i != jsonData.Data.Length; i++)
                {
                    if (string.IsNullOrEmpty(jsonData.Data[i].OriginalName))
                    {
                        jsonData.Data[i].OriginalName = "";
                    }
                    if (string.IsNullOrEmpty(jsonData.Data[i].JapaneseName))
                    {
                        jsonData.Data[i].JapaneseName = "";
                    }
                    if (string.IsNullOrEmpty(jsonData.Data[i].EnglistName))
                    {
                        jsonData.Data[i].EnglistName = "";
                    }
                    if (string.IsNullOrEmpty(jsonData.Data[i].ChineseName))
                    {
                        jsonData.Data[i].ChineseName = "";
                    }
                }
            }
            else
            {
                Vocal[] vocal = new Vocal[1];
                vocal[0] = new Vocal();
                return vocal;
            }
            return jsonData.Data;
        }
    }
}
