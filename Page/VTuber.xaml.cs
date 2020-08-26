using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using VTuberMusic.Modules;
using VTuberMusic.Tools;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace VTuberMusic.Page
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class VTuber
    {
        Vocal vocal = null;
        string vocalId;
        ObservableCollection<Song> songs = new ObservableCollection<Song>();

        public VTuber()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            vocalId = (string)e.Parameter;
            LoadingRing.IsActive = true;
            new Thread(a =>
            {
                try
                {
                    vocal = Vocal.GetVocalObject(vocalId);
                }
                catch (Exception ex)
                {
                    Invoke(new Action(delegate
                    {
                        LoadingRing.IsActive = false;
                        Frame.Navigate(typeof(Fail), new Error { ErrorCode = ex.Message, CanBackHome = true, ReTryPage = typeof(VTuber), ReTryArgs = vocalId });
                    }));
                }

                Invoke(new Action(delegate { LoadingRing.IsActive = false; }));
                if (vocal != null)
                {
                    Invoke(new Action(delegate
                    {
                        // 判断 Youtube 推特 Bilibili 是否为空 null
                        if (string.IsNullOrEmpty(vocal.Bilibili) && vocal.Bilibili == "")
                        {
                            BiliBili.Visibility = Visibility.Collapsed;
                        }
                        if (string.IsNullOrEmpty(vocal.YouTube) && vocal.YouTube == "")
                        {
                            Youtube.Visibility = Visibility.Collapsed;
                        }
                        if (string.IsNullOrEmpty(vocal.Twitter) && vocal.Twitter == "")
                        {
                            Twitter.Visibility = Visibility.Collapsed;
                        }
                        // 获取歌曲
                        Song[] songsArray = Song.GetMusicList("VocalId", vocalId, 1, 1000, "CreateTime", "desc");
                        // 输出歌曲
                        for (int i = 0; i != songsArray.Length; i++)
                        {
                            songs.Add(songsArray[i]);
                        }
                        // 显示数据
                        OriginalName.Text = vocal.OriginalName;
                        ChineseName.Text = vocal.ChineseName;
                        VocalGroup.Text = string.Format(Lang.ReadLangText("VTuberGroup"), vocal.GroupsId);
                        // 载入图片
                        BitmapImage vocalImage = new BitmapImage(new Uri(vocal.AvatarImg));
                        BackgroundImage.Source = vocalImage;
                        VocalImage.ProfilePicture = vocalImage;
                    }));
                }
            }).Start();
        }

        private void SongListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.ItemIndex % 2 == 0)
            {
                var brush = new SolidColorBrush((Color)Resources["SystemListLowColor"]);
                args.ItemContainer.Background = brush;
            }
            else
            {
                var brush = new SolidColorBrush((Color)Resources["SystemChromeMediumColor"]);
                args.ItemContainer.Background = brush;
            }
        }

        private void SongListView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (SongListView.SelectedIndex != -1)
            {
                MainPage.player.PlayListClear();
                MainPage.player.PlayListAddSongList(songs.ToArray());
                MainPage.player.PlayIndex(MainPage.player.PlayList.IndexOf(songs[SongListView.SelectedIndex]));
            }
        }

        private void PlayAll_Click(object sender, RoutedEventArgs e)
        {
            if (songs.Count != 0)
            {
                MainPage.player.PlayListClear();
                MainPage.player.PlayListAddSongList(songs.ToArray());
                MainPage.player.PlayIndex(MainPage.player.PlayList.IndexOf(songs[0]));
            }
        }

        private async void BiliBili_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://space.bilibili.com/" + vocal.Bilibili));
        }

        private async void Youtube_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.youtube.com/channel/" + vocal.YouTube));
        }

        private async void Twitter_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://twitter.com/" + vocal.Twitter));
        }

        private void Share_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested += DataRequested;
            DataTransferManager.ShowShareUI();
        }

        private void DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            args.Request.Data.SetWebLink(new Uri("https://vtbmusic.com/vtuber?id=" + vocal.Id));
            args.Request.Data.Properties.Title = "分享 VTuber";
            args.Request.Data.Properties.Description = vocal.OriginalName;
        }

        private void AddToPlayList_Click(object sender, RoutedEventArgs e)
        {
            if (songs.Count != 0)
            {
                Button clickButton = (Button)sender;
                MainPage.player.PlayListAddSong(Song.GetSongObject((string)clickButton.Tag));
            }
        }

        public void Invoke(Action action, Windows.UI.Core.CoreDispatcherPriority Priority = Windows.UI.Core.CoreDispatcherPriority.Normal)
        {
            var reslut = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); });
            while (reslut.Status != AsyncStatus.Completed)
            {
                //
            }
            return;
        }
    }
}
