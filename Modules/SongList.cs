  using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTuberMusic.Modules;
using VTuberMusic.Network.GetTools;
using VTuberMusic.Tools;

namespace VTuberMusic.Modules
{
    class SongListList
    {
        public string Id { get; set; } = "";
        public string CreateTime { get; set; } = "";
        public object PublishTime { get; set; } = "";
        public string CreatorId { get; set; } = "";
        public string CreatorRealName { get; set; } = "";
        public bool Deleted { get; set; } = false;
        public string Name { get; set; } = "";
        public string CoverImg { get; set; } = "";
        public string introduce { get; set; } = "";

        /// <summary>
        /// 获取 / 搜索歌单列表
        /// </summary>
        /// <param name="SearchCondition">搜索依据</param>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="PageIndex">页码</param>
        /// <param name="PageRows">一页输出数量</param>
        /// <returns></returns>
        public static SongListList[] GetSongListList(string SearchCondition, string keyword, int PageIndex, int PageRows, string sortField, string sortType)
        {
            string postJson = JsonMapper.ToJson(new GetModules.ListPostModule { search = new GetModules.Search { condition = SearchCondition, keyword = keyword }, pageIndex = PageIndex, pageRows = PageRows, sortField = sortField, sortType = sortType });
            var response = GetTools.PostApi("/v1/GetAlbumsList", postJson);
            if (response.IsSuccessful)
            {
                GetModules.SongListListGetModule jsonData = JsonMapper.ToObject<GetModules.SongListListGetModule>(response.Content);
                if (jsonData.Success)
                {
                    for (int i = 0; i != jsonData.Data.Length; i++)
                    {
                        if (string.IsNullOrEmpty(jsonData.Data[i].introduce))
                        {
                            jsonData.Data[i].introduce = "";
                        }
                    }
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
                throw new Exception("错误代码:" + response.StatusCode.ToString());
            }
        }
    }

    public class SongList
    {
        public static Song[] GetSongListSong(string id)
        {
            string postJson = JsonMapper.ToJson(new GetModules.SinglePostModule { id = id });
            var response = GetTools.PostApi("/v1/GetAlbumsData", postJson);
            if (response.IsSuccessful)
            {
                GetModules.SongListGetModule jsonData = JsonMapper.ToObject<GetModules.SongListGetModule>(response.Content);
                if (jsonData.Success)
                {
                    for (int i = 0; i != jsonData.Data.Data.Length; i++)
                    {
                        string[] assestUri = JointAssetsUrl.GetAssestUri(jsonData.Data.Data[i].CDN, GetTools.CDNList, jsonData.Data.Data[i].CoverImg, jsonData.Data.Data[i].Music, jsonData.Data.Data[i].Lyric);
                        jsonData.Data.Data[i].assestLink = new Song.AssestLink { CoverImg = assestUri[0], Music = assestUri[1], Lyric = assestUri[2] };
                    }
                    Log.WriteLine("获取歌单 " + id + " 成功", Level.Info);
                    return jsonData.Data.Data;
                }
                else
                {
                    Log.WriteLine("请求失败:\r\n" + response.Content, Level.Error);
                    throw new Exception(jsonData.Msg.ToString());
                }
            }
            else
            {
                throw new Exception("错误代码:" + response.StatusCode.ToString());
            }
        }
    }
}
