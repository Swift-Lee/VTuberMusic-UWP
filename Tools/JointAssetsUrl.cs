using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Media.Capture;

namespace VTuberMusic.Tools
{
    class JointAssetsUrl
    {
        public static string[] GetAssestUri(string CDN, string[] CDNList, string CoverImg, string Musics, string Lyric)
        {
            string[] AssestUrl = new string[3];
            // 判断是 1:2:3 还是 1
            if (CDN.IndexOf(":") != -1 == true)
            {
                string[] cdn = Regex.Split(CDN, ":");
                AssestUrl[0] = CDNList[int.Parse(cdn[0])] + CoverImg;
                AssestUrl[1] = CDNList[int.Parse(cdn[1])] + Musics;
                AssestUrl[2] = CDNList[int.Parse(cdn[2])] + Lyric;
            }
            else
            {
                AssestUrl[0] = CDNList[int.Parse(CDN)] + CoverImg;
                AssestUrl[1] = CDNList[int.Parse(CDN)] + Musics;
                AssestUrl[2] = CDNList[int.Parse(CDN)] + Lyric;
            }
            return AssestUrl;
        }
    }
}
