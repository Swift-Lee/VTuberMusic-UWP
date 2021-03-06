﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTuberMusic.Modules;

namespace VTuberMusic.Network.GetTools
{
    class GetModules
    {
        #region 音乐请求数据模型
        public class MusicListGetModule
        {
            public int Total { get; set; }
            public Song[] Data { get; set; }
            public bool Success { get; set; } = false;
            public int ErrorCode { get; set; }
            public object Msg { get; set; }
        }

        public class SingleMusicGetModule
        {
            public Song Data { get; set; }
            public bool Success { get; set; }
            public int ErrorCode { get; set; }
            public string Msg { get; set; }
        }

        public class Vocallist
        {
            public string Id { get; set; }
            public string cn { get; set; }
            public string jp { get; set; }
            public string en { get; set; }
            public string originlang { get; set; }
        }
        #endregion

        #region VTuber 请求数据模型
        public class VocalListGetModule
        {
            public int Total { get; set; }
            public Vocal[] Data { get; set; }
            public bool Success { get; set; }
            public int ErrorCode { get; set; }
            public object Msg { get; set; }
        }

        public class SingleVocalGetModule
        {
            public int Total { get; set; }
            public Vocal Data { get; set; }
            public bool Success { get; set; }
            public int ErrorCode { get; set; }
            public object Msg { get; set; }
        }
        #endregion

        #region 请求 Json 模型
        public class ListPostModule
        {
            public Search search { get; set; } = new Search();
            public int pageIndex { get; set; }
            public int pageRows { get; set; }
            public string sortField { get; set; } = "Id";
            public string sortType { get; set; } = "desc";
        }

        public class Search
        {
            public string condition { get; set; } = "";
            public string keyword { get; set; } = "";
        }

        public class SinglePostModule
        {
            public string id { get; set; }
        }

        public class ArrayPostModule
        {
            public string[] ids { get; set; }
        }

        public class LoginPostModule
        {
            public string password { get; set; }
            public string userName { get; set; }
        }
        #endregion

        #region Banner 请求数据模型
        public class BannerGetModule
        {
            public int Total { get; set; }
            public Banner[] Data { get; set; }
            public bool Success { get; set; }
            public int ErrorCode { get; set; }
            public object Msg { get; set; }
        }
        #endregion

        #region 歌单列表请求数据模型
        public class SongListListGetModule
        {
            public int Total { get; set; }
            public SongListList[] Data { get; set; }
            public bool Success { get; set; }
            public int ErrorCode { get; set; }
            public object Msg { get; set; }
        }

        public class SongListGetModule
        {
            public rdData Data { get; set; }
            public bool Success { get; set; }
            public int ErrorCode { get; set; }
            public object Msg { get; set; }

            public class rdData
            {
                public Song[] Data { get; set; }
            }
        }
        #endregion

        #region 歌曲数据请求模型
        public class LyricGetModules
        {
            public bool karaoke { get; set; }
            public bool scrollDisabled { get; set; }
            public bool translated { get; set; }
            public Origin origin { get; set; }
            public Translate translate { get; set; }

            public class Origin
            {
                public int version { get; set; }
                public string text { get; set; }
            }

            public class Translate
            {
                public int version { get; set; }
                public string text { get; set; }
            }
        }
        #endregion

        #region 用户账户请求数据模型
        public class LoginGetModules
        {
            public string Data { get; set; }
            public bool Success { get; set; }
            public int ErrorCode { get; set; }
            public string Msg { get; set; }
        }

        public class UserGetModules
        {
            public rdData Data { get; set; }
            public bool Success { get; set; }
            public int ErrorCode { get; set; }
            public string Msg { get; set; }

            public class rdData
            {
                public Account UserInfo { get; set; }
                public int Message { get; set; }
            }
        }
        #endregion
    }
}