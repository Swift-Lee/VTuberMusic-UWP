using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTuberMusic.Network.GetTools;
using LitJson;
using System.Diagnostics;
using VTuberMusic.Tools;

namespace VTuberMusic.Modules
{
    public class Song
    {
        public string Id { get; set; }
        public string CreateTime { get; set; }
        public string PublishTime { get; set; }
        public string CreatorId { get; set; }
        public string CreatorRealName { get; set; }
        public bool Deleted { get; set; }
        public string OriginName { get; set; }
        public string VocalId { get; set; }
        public string VocalName { get; set; }
        public string CoverImg { get; set; }
        public string Musics { get; set; }
        public string Music { get; set; }
        public string Lyric { get; set; }
        public string CDN { get; set; }
        public string Source { get; set; }
        public string BiliBili { get; set; }
        public string YouTube { get; set; }
        public string Twitter { get; set; }
        public int? Likes { get; set; }
        public int? Length { get; set; }
        public string Label { get; set; }
        public Vocallist[] VocalList { get; set; }
        public AssestLink assestLink { get; set; }

        public class Vocallist
        {
            public string Id { get; set; }
            public string cn { get; set; }
            public string jp { get; set; }
            public string en { get; set; }
            public string originlang { get; set; }
        }

        public class AssestLink
        {
            public string CoverImg { get; set; }
            public string Music { get; set; }
            public string Lyric { get; set; }
        }

        /// <summary>
        /// 获取歌曲对象
        /// </summary>
        /// <param name="SongId">歌曲 Id</param>
        public static Song GetSongObject(string SongId)
        {
            string postJson = JsonMapper.ToJson(new GetModules.SinglePostModule { id = SongId });
            string jsonText = GetTools.PostApi("/v1/GetMusicData",postJson); // 请求 Api
            GetModules.SingleMusicGetModule jsonData = JsonMapper.ToObject<GetModules.SingleMusicGetModule>(jsonText); // 转 Json 为对象
            Song song = jsonData.Data;
            string[] assestUri = JointAssetsUrl.GetAssestUri(song.CDN, GetTools.CDNList, song.CoverImg, song.Musics, song.Lyric);
            song.assestLink = new AssestLink { CoverImg = assestUri[0], Music = assestUri[1], Lyric = assestUri[2] };
            return jsonData.Data;
        }

        /// <summary>
        /// 获取热门歌曲
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageRows"></param>
        /// <returns></returns>
        public static Song[] GetHotMusic(int PageIndex,int PageRows)
        {
            string postJson = JsonMapper.ToJson(new GetModules.ListPostModule { pageIndex = PageIndex, pageRows = PageRows});
            GetModules.MusicListGetModule jsonData = JsonMapper.ToObject<GetModules.MusicListGetModule>(GetTools.PostApi("/v1/GetHotMusicList",postJson));
            Song[] songs = jsonData.Data;
            for(int i=0; i != songs.Length; i++)
            {
                string[] assestUri = JointAssetsUrl.GetAssestUri(songs[i].CDN, GetTools.CDNList, songs[i].CoverImg, songs[i].Music, songs[i].Lyric);
                songs[i].assestLink = new AssestLink { CoverImg = assestUri[0], Music = assestUri[1], Lyric = assestUri[2] };
            }
            return songs;
        }

        /// <summary>
        /// 获取 / 搜索歌曲，当网络错误获取失败时会抛出异常
        /// </summary>
        /// <param name="SearchCondition">搜索依据</param>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="PageIndex">页码</param>
        /// <param name="PageRows">一页输出数量</param>
        /// <returns></returns>
        public static Song[] GetMusicList(string SearchCondition, string keyword, int PageIndex, int PageRows, string sortField, string sortType)
        {
            string postJson = JsonMapper.ToJson(new GetModules.ListPostModule { search = new GetModules.Search { condition = SearchCondition, keyword = keyword }, pageIndex = PageIndex, pageRows = PageRows, sortField = sortField, sortType = sortType });
            GetModules.MusicListGetModule jsonData = JsonMapper.ToObject<GetModules.MusicListGetModule>(GetTools.PostApi("/v1/GetMusicList", postJson));
            Song[] songs = jsonData.Data;
            for (int i = 0; i != songs.Length; i++)
            {
                string[] assestUri = JointAssetsUrl.GetAssestUri(songs[i].CDN, GetTools.CDNList, songs[i].CoverImg, songs[i].Music, songs[i].Lyric);
                songs[i].assestLink = new AssestLink { CoverImg = assestUri[0], Music = assestUri[1], Lyric = assestUri[2] };
            }
            return songs;
        }
    }
}
