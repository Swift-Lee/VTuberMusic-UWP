using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using VTuberMusic.Modules;
using VTuberMusic.Tools;
using System.Threading.Tasks;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace VTuberMusic.Page
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Home
    {
        Song[] songs = Song.GetHotMusic(1,10);
        Banner[] banners = Banner.GetBanners();
        Vocal[] vocals = Vocal.GetVocalList("OriginalName", "", 1, 10, "Watch", "desc");
        SongListList[] songList = SongListList.GetSongListList("Id", "", 1, 10, "CreateTime", "desc");

        public Home()
        {
            this.InitializeComponent();
        }

        private void RefreshContainer_RefreshRequested(RefreshContainer sender, RefreshRequestedEventArgs args)
        {
            songs = Song.GetHotMusic(1, 1);
            banners = Banner.GetBanners();
            vocals = Vocal.GetVocalList("OriginaName", "", 1, 10, "Watch", "desc");
            songList = SongListList.GetSongListList("Id", "", 1, 10, "CreateTime", "desc");
        }

        private void SongGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Log.WriteLine(songs[SongGridView.SelectedIndex].OriginName, Level.DeBug);
            MainPage.player.GetSong(songs[SongGridView.SelectedIndex]);
            MainPage.player.SetPlayerPosition(TimeSpan.FromSeconds(0));
            Frame.Navigate(typeof(Playing));
        }
    }
}
