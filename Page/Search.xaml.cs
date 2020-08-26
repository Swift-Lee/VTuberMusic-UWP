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
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace VTuberMusic.Page
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Search
    {
        string searchText;
        Pivot pivot;
        ObservableCollection<Song> songs = new ObservableCollection<Song>();
        ObservableCollection<Vocal> vocals = new ObservableCollection<Vocal>();
        ObservableCollection<SongListList> songLists = new ObservableCollection<SongListList>();

        public Search()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            searchText = (string)e.Parameter;
            Title.Text = "搜索 \"" + searchText + "\" 的结果";
            Log.WriteLine("[UI]跳转到搜索", Level.Info);
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pivot = (Pivot)sender;
            switch (pivot.SelectedIndex)
            {
                #region 歌曲搜索
                case 0:
                    Log.WriteLine("[UI]搜索音乐: " + searchText, Level.Info);
                    SongProgressRing.IsActive = true;
                    new Thread(a =>
                    {
                        Song[] songsArray = new Song[0];
                        try
                        {
                            songsArray = Song.GetMusicList("OriginName", searchText, 1, 200, "OriginName", "dasc");
                        }
                        catch (Exception ex)
                        {
                            Invoke(new Action(delegate {
                                SongProgressRing.IsActive = false;
                                Frame.Navigate(typeof(Fail), new Error { ErrorCode = ex.Message, CanBackHome = true, ReTryPage = typeof(Search), ReTryArgs = searchText }); 
                            }));
                        }

                        Invoke(new Action(delegate { SongProgressRing.IsActive = false; }));
                        if (songsArray.Length != 0)
                        {
                            Invoke(new Action(delegate
                            {
                                songs.Clear();
                                for (int i = 0; i != songsArray.Length; i++)
                                {
                                    songs.Add(songsArray[i]);
                                }
                            }));
                        }
                        else
                        {
                            Invoke(new Action(delegate { SongFailText.Text = "找不到内容"; }));
                            Log.WriteLine("[UI]搜索音乐: " + searchText + " 失败", Level.Error);
                        }
                    }).Start();
                    break;
                #endregion
                #region VTuber 搜索
                case 1:
                    Log.WriteLine("[UI]搜索 VTuber: " + searchText, Level.Info);
                    VTuberProgressRing.IsActive = true;
                    new Thread(a =>
                    {
                        Vocal[] vocalsArray = new Vocal[0];
                        try
                        {
                            vocalsArray = Vocal.GetVocalList("OriginalName", searchText, 1, 200, "OriginalName", "desc");
                        }
                        catch (Exception ex)
                        {
                            Invoke(new Action(delegate { 
                                Frame.Navigate(typeof(Fail), new Error { ErrorCode = ex.Message, CanBackHome = true, ReTryPage = typeof(Search), ReTryArgs = searchText });
                                VTuberProgressRing.IsActive = false;
                            }));
                        }

                        Invoke(new Action(delegate { VTuberProgressRing.IsActive = false; }));
                        if(vocalsArray.Length != 0)
                        {
                            Invoke(new Action(delegate
                            {
                                vocals.Clear();
                                for (int i = 0; i != vocalsArray.Length; i++)
                                {
                                    vocals.Add(vocalsArray[i]);
                                }
                            }));
                        }
                        else
                        {
                            Invoke(new Action(delegate { VTuberFailText.Text = "找不到内容"; }));
                            Log.WriteLine("[UI]搜索 VTuber: " + searchText + " 失败", Level.Error);
                        }
                    }).Start();
                    break;
                #endregion
                #region 歌单搜索
                case 2:
                    Log.WriteLine("[UI]搜索歌单: " + searchText, Level.Info);
                    SongListProgressRing.IsActive = true;
                    new Thread(a =>
                    {
                        SongListList[] songListArray = new SongListList[0];
                        try
                        {
                            songListArray = SongListList.GetSongListList("Name", searchText, 1, 50, "Name", "desc");
                        }
                        catch (Exception ex)
                        {
                            Invoke(new Action(delegate { 
                                Frame.Navigate(typeof(Fail), new Error { ErrorCode = ex.Message, CanBackHome = true, ReTryPage = typeof(Search), ReTryArgs = searchText });
                                SongListProgressRing.IsActive = false;
                            }));
                        }

                        Invoke(new Action(delegate { SongListProgressRing.IsActive = false; }));
                        if (songListArray.Length != 0)
                        {
                            Invoke(new Action(delegate
                            {
                                songLists.Clear();
                                for (int i = 0; i != songListArray.Length; i++)
                                {
                                    songLists.Add(songListArray[i]);
                                }
                            }));
                        }
                        else
                        {
                            Invoke(new Action(delegate { SongListFailText.Text = "找不到内容"; }));
                            Log.WriteLine("[UI]搜索歌单: " + searchText + " 失败", Level.Error);
                        }
                    }).Start();
                    break;
                    #endregion
            }
        }

        #region 列表颜色相间
        private void SongView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
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

        private void VTuberView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
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
        #endregion

        #region 监听歌单搜索列表
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickButton = (Button)sender;
            Song clickSong = Song.GetSongObject((string)clickButton.Tag);
            MainPage.player.PlayListAddSong(clickSong);
            MainPage.player.PlayIndex(MainPage.player.PlayList.IndexOf(clickSong));
        }

        private void SongView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (SongView.SelectedIndex != -1)
            {
                MainPage.player.PlayListAddSong(songs[SongView.SelectedIndex]);
                MainPage.player.PlayIndex(MainPage.player.PlayList.IndexOf(songs[SongView.SelectedIndex]));
            }
        }
        #endregion

        #region 列表点击事件
        private void VTuberView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (VTuberView.SelectedIndex != -1)
            {
                Frame.Navigate(typeof(VTuber), vocals[VTuberView.SelectedIndex].Id);
            }
        }

        private void SongListView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (SongListView.SelectedIndex != -1)
            {
                Frame.Navigate(typeof(SongList), songLists[SongListView.SelectedIndex].Id);
            }
        }
        #endregion

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