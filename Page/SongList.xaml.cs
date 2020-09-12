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


namespace VTuberMusic.Page
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SongList
    {
        public string songListId;
        SongListList[] songListList = null;
        public ObservableCollection<Song> songs = new ObservableCollection<Song>();

        public SongList()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            songListId = (string)e.Parameter;
            LoadingRing.IsActive = true;
            if (songListId != null)
            {
                Log.WriteLine("[GUI]跳转到歌单: " + songListId, Level.Info);
                new Thread(a =>
                {
                    Song[] getSongs = null;
                    try
                    {
                        songListList = SongListList.GetSongListList("Id", songListId, 1, 1, "Id", "dasc");
                        getSongs = Modules.SongList.GetSongListSong(songListId);
                    }
                    catch (Exception ex)
                    {
                        Invoke(new Action(delegate
                        {
                            Frame.Navigate(typeof(Fail), new Error { ErrorCode = ex.Message, CanBackHome = true, ReTryPage = typeof(VTuber), ReTryArgs = songListId });
                        }));
                    }

                    if (getSongs != null)
                    {
                        Invoke(new Action(delegate
                        {
                            songs.Clear();
                            for (int i = 0; i != getSongs.Length; i++)
                            {
                                songs.Add(getSongs[i]);
                            }
                        }));
                    }

                    if (songListList != null)
                    {
                        Invoke(new Action(delegate
                        {
                            LoadingRing.IsActive = false;
                            SongListName.Text = songListList[0].Name;
                            intro.Text = songListList[0].introduce;
                            SongListCreator.Text = songListList[0].CreatorRealName;
                            SongListCreatorImage.DisplayName = songListList[0].CreatorRealName;
                            BitmapImage bitmapImage = new BitmapImage(new Uri(songListList[0].CoverImg));
                            SongListImage.Source = (bitmapImage);
                        }));
                    }
                }).Start();
            }
            else
            {
                Log.WriteLine("[GUI]跳转到歌单: 本地我喜欢歌单", Level.Info);
            }
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

        private void Share_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested += DataRequested;
            DataTransferManager.ShowShareUI();
        }

        private void DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            args.Request.Data.SetWebLink(new Uri("https://vtbmusic.com/songlist?id=" + songListId));
            args.Request.Data.Properties.Title = "分享歌单";
            args.Request.Data.Properties.Description = songListList[0].Name;
        }

        private void PlayAll_Click(object sender, RoutedEventArgs e)
        {
            if(songs.Count != 0)
            {
                MainPage.player.PlayListClear();
                MainPage.player.PlayListAddSongList(songs.ToArray());
                MainPage.player.PlayIndex(0);
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
