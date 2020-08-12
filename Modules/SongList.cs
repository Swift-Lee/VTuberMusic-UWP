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
        public string Id { get; set; }
        public string CreateTime { get; set; }
        public object PublishTime { get; set; }
        public string CreatorId { get; set; }
        public string CreatorRealName { get; set; }
        public bool Deleted { get; set; }
        public string Name { get; set; }
        public string CoverImg { get; set; }
        public object introduce { get; set; }

        /// <summary>
        /// 获取 / 搜索歌单列表，当网络错误获取失败时会抛出异常
        /// </summary>
        /// <param name="SearchCondition">搜索依据</param>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="PageIndex">页码</param>
        /// <param name="PageRows">一页输出数量</param>
        /// <returns></returns>
        public static SongListList[] GetSongListList(string SearchCondition, string keyword, int PageIndex, int PageRows, string sortField, string sortType)
        {
            string postJson = JsonMapper.ToJson(new GetModules.ListPostModule { search = new GetModules.Search { condition = SearchCondition, keyword = keyword }, pageIndex = PageIndex, pageRows = PageRows, sortField = sortField, sortType = sortType });
            GetModules.SongListListGetModule jsonData = JsonMapper.ToObject<GetModules.SongListListGetModule>(GetTools.PostApi("/v1/GetAlbumsList", postJson));
            SongListList[] songListList = jsonData.Data;
            return songListList;
        }
    }

    public class SongList
    {
        public static Song[] GetSongListSong(string id)
        {
            string postJson = JsonMapper.ToJson(new GetModules.SinglePostModule { id = id });
            GetModules.SongListGetModule jsonData = JsonMapper.ToObject<GetModules.SongListGetModule>(GetTools.PostApi("/v1/GetAlbumsData", postJson));
            Song[] songs = jsonData.Data.Data;
            for(int i = 0; i != songs.Length; i++)
            {
                string[] assestUri = JointAssetsUrl.GetAssestUri(songs[i].CDN, GetTools.CDNList, songs[i].CoverImg, songs[i].Music, songs[i].Lyric);
                songs[i].assestLink.CoverImg = assestUri[0];
                songs[i].assestLink.Music = assestUri[1];
                songs[i].assestLink.Lyric = assestUri[2];
            }
            return songs;
        }
    }
}
