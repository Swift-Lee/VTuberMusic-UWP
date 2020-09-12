using LitJson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VTuberMusic.Tools;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace VTuberMusic.Modules
{
    class Lyric
    {
        public bool karaoke { get; set; }
        public bool scrollDisabled { get; set; }
        public bool translated { get; set; }
        public Origin origin { get; set; }
        public Translate translate { get; set; }
        public WordWithTranslate[] parse { get; set; } = new WordWithTranslate[0];

        public class Origin
        {
            public int version { get; set; } = 0;
            public string text { get; set; } = "";
        }

        public class Translate
        {
            public int version { get; set; } = 0;
            public string text { get; set; } = "";
        }

        public class Word
        {
            public string Id { get; set; } = "-1";
            public TimeSpan Time = TimeSpan.Zero;
            public string Text = "";
        }

        public static void test(string content)
        {
            Lyric lyric = JsonMapper.ToObject<Lyric>(content);
            string orgin = lyric.origin.text;
            string translate = lyric.translate.text;
            string[] ContentLines = orgin.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i != ContentLines.Length; i++)
            {
                Regex regex = new Regex(@"\[(.*?)\]([^[\]]*)\s*", RegexOptions.Compiled);
                MatchCollection mc = regex.Matches(ContentLines[i]);
                Log.WriteLine("\r\n[" + mc[0].Groups[1].Value + "]\r\n" + mc[0].Groups[2].Value);
            }
        }

        public static Lyric ParseLyric(string LyricContent)
        {
            try
            {
                return ParseVrc(LyricContent);
            }
            catch (Exception ex)
            {
                Log.WriteLine("[歌词解析器]无法以 Vrc 方式解析 错误原因: " + ex.Message, Level.Error);
            }
            return new Lyric();
        }

        #region 解析 Vrc
        public static Lyric ParseVrc(string Vrc)
        {
            // 将 Json 转换为对象
            Lyric lyric = JsonMapper.ToObject<Lyric>(Vrc);
            // 声明个带翻译的歌词
            List<WordWithTranslate> wordWithTranslates = new List<WordWithTranslate>();
            // 解析 Lrc
            Word[] oringWord = ParseLrc(lyric.origin.text);
            Word[] translateWord = ParseLrc(lyric.translate.text);
            // 向列表添加歌词
            for(int i = 0; i != oringWord.Length; i++)
            {
                wordWithTranslates.Add(new WordWithTranslate {
                    Id = i.ToString(),
                    Time = oringWord[i].Time,
                    Orgin = oringWord[i].Text,
                    brush = new SolidColorBrush(Colors.Black),
                    Translate = translateWord[i].Text }) ;
            }
            lyric.parse = wordWithTranslates.ToArray();
            return lyric;
        }
        #endregion

        #region 解析 Lrc
        public static Word[] ParseLrc(string Lrc)
        {
            string[] ContentLines = Lrc.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            TimeSpan offset = TimeSpan.Zero;
            List<Word> lrcWord = new List<Word>();

            for (int i = 0; i != ContentLines.Length; i++)
            {
                if (ContentLines[i].StartsWith("[offset:"))
                {
                    offset = TimeSpan.FromMilliseconds(int.Parse(SplitInfo(ContentLines[i])));
                }
                else
                {
                    Regex regex = new Regex(@"\[(.*?)\]([^[\]]*)\s*", RegexOptions.Compiled);
                    MatchCollection mc = regex.Matches(ContentLines[i]);
                    TimeSpan time = TimeTransition(mc[0].Groups[1].Value, offset);
                    string word = mc[0].Groups[2].Value;
                    lrcWord.Add(new Word { Id = i.ToString(), Text = word, Time = time });
                }
            }
            return lrcWord.ToArray();
        }
        #endregion

        #region 分割信息时间转换
        private static string SplitInfo(string line)
        {
            return line.Substring(line.IndexOf(":") + 1).TrimEnd(']');
        }

        private static TimeSpan TimeTransition(string time, TimeSpan offset)
        {
            string[] splited = time.Split(":");
            switch (splited.Length)
            {
                // 00:00.00 or 00:00.000
                case 2:
                    string[] seconds = splited[1].Split(".");
                    return TimeSpan.FromMinutes(long.Parse(splited[0])) + 
                        TimeSpan.FromSeconds(long.Parse(seconds[0])) +
                        TimeSpan.FromMilliseconds(long.Parse(seconds[1]));
                // 00.00 毒轴
                case 1:
                    string[] split = splited[0].Split(".");
                    return TimeSpan.FromSeconds(long.Parse(split[0])) +
                        TimeSpan.FromSeconds(float.Parse("0." + split[1]));
                default:
                    return TimeSpan.Zero;
            }
        }
        #endregion
    }

    public class WordWithTranslate
    {
        public string Id { get; set; } = "-1";
        public TimeSpan Time { get; set; } = TimeSpan.Zero;
        public string Orgin { get; set; } = "";
        public Brush brush = new SolidColorBrush(Colors.Black);
        public string Translate { get; set; } = "";
    }
}
