using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;

namespace VTuberMusic
{
    public class SongObject
    {
        public int ID { get; set; } // 歌曲 Id
        public string Title { get; set; } // 歌曲名称
        public string Vocal { get; set; } // 歌手
        public string ImageUri { get; set; } // 图片地址
        public string SongUri { get; set; } // 歌曲文件地址
        public string LrcUri { get; set; } // 歌词文件地址

        public SongObject()
        {

        }

        public List<SongObject> GetTestObjects()
        {
            var objects = new List<SongObject>
            {
                new SongObject { Title = "勾指起誓", Vocal = "雫るる", ImageUri = "https://s-gz-200-vtbmusic.oss.dogecdn.com/image/d96d70f4e99042ae7c93b31b7345992f" },
                new SongObject { Title = "エヴァーグリーン", Vocal = "绿仙", ImageUri = "https://s-gz-200-vtbmusic.oss.dogecdn.com/image/4a58bbdfb8367660dcb8a0eb9852058e" },
                new SongObject { Title = "Blessing(LOL部)", Vocal = "鹰宫莉音 湊阿库娅 宇森雏子 椎名唯华 德比德比·德比鲁", ImageUri = "https://s-gz-200-vtbmusic.oss.dogecdn.com/image/330b2035fb18dffe713531b09e687fa0" },
                new SongObject { Title = "P.F.M.", Vocal = "绿仙 梦追翔 加贺美隼人", ImageUri = "https://s-gz-200-vtbmusic.oss.dogecdn.com/image/1c6275e5e7bf2297df13d7af0328fa69" },
                new SongObject { Title = "メランコリック", Vocal = "久远蓝", ImageUri = "https://s-gz-200-vtbmusic.oss.dogecdn.com/image/ae3ee46e1aec9b4b65aab4b15c305998" },
                new SongObject { Title = "アヤノの幸福理論", Vocal = "润羽露西娅", ImageUri = "https://s-gz-200-vtbmusic.oss.dogecdn.com/image/3205d36a84c5324bcd92ace0e8309825" }
            };
            return objects;
        }
    }

    public class SongListObject
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Creater { get; set; }
        public string ImageUri { get; set; }

        public List<SongObject> GetSongList()
        {
            SongObject songObject = new SongObject();
            return songObject.GetTestObjects();
        }

        public List<SongListObject> GetTestObject()
        {
            var objects = new List<SongListObject>
            {
                new SongListObject { Title = "星街彗星 3D披露会 Live", Creater = "星街彗星", ImageUri = "https://santiego.gitee.io/vtb-music-source-img/img/c1000.jpg", ID = 1 },
                new SongListObject { Title = "星街彗星 生日前日祭", Creater = "星街彗星", ImageUri = "https://santiego.gitee.io/vtb-music-source-img/img/1021.png", ID = 2 },
                new SongListObject { Title = "物述有栖 4.6 B限歌回", Creater = "物述有栖", ImageUri = "https://santiego.gitee.io/vtb-music-source-img/img/C3200411174000.jpg", ID = 3 },
                new SongListObject { Title = "3.27 B限歌回", Creater = "星街彗星", ImageUri = "https://santiego.gitee.io/vtb-music-source-img/img/1028.jpg", ID = 4 },
                new SongListObject { Title = "药袋卡尔蒂 夜莺的摇篮曲", Creater = "药袋卡尔蒂", ImageUri = "https://santiego.gitee.io/vtb-music-source-img/img/C3200425174700.jpg", ID = 5 },
                new SongListObject { Title = "【B限】gal专场演唱会", Creater = "樱巫女", ImageUri = "https://santiego.gitee.io/vtb-music-source-img/img/C2200506162801.jpg", ID = 6 }
            };
            return objects;
        }
    }

    public class VocalObject
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string jpName { get; set; }
        public string ImageUri { get; set; }

        public List<VocalObject> GetTestObject()
        {
            var objects = new List<VocalObject>
            {
                new VocalObject {ID=1,jpName="星街すいせい", Name="星街彗星", ImageUri="https://santiego.gitee.io/vtb-music-source-img/img/figure/星街彗星.png"},
                new VocalObject {ID=2,jpName="夏色 まつり", Name="夏色祭",ImageUri="https://santiego.gitee.io/vtb-music-source-img/img/figure/夏色祭.png"},
                new VocalObject {ID=3,jpName="天音かなた", Name="天音彼方",ImageUri="https://santiego.gitee.io/vtb-music-source-img/img/figure/天音彼方.png"},
            };
            return objects;
        }
    }
}
