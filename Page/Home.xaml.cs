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
using Windows.Media;
using System.Collections.ObjectModel;
using System.Threading;
using Windows.ApplicationModel.Core;
using System.ServiceModel.Channels;
using Windows.UI.Notifications;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace VTuberMusic.Page
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Home
    {
        ObservableCollection<Song> songs = new ObservableCollection<Song>();
        ObservableCollection<Banner> banners = new ObservableCollection<Banner>();
        ObservableCollection<Vocal> vocals = new ObservableCollection<Vocal>();
        ObservableCollection<SongListList> songLists = new ObservableCollection<SongListList>();

        public Home()
        {
            InitializeComponent();
            new Thread(a =>
            {
                int i = 0;
                Song[] getSongs = new Song[0];
                try
                {
                    getSongs = Song.GetHotMusic(1, 20);
                }
                catch (Exception ex)
                {
                    var reslut = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Frame.Navigate(typeof(Fail), new Error { ErrorCode = ex.Message, ReTryPage = typeof(Home) });
                    });
                    while (true)
                    {
                        if (reslut.Status == AsyncStatus.Completed)
                        {
                            return;
                        }
                    }
                }

                while (i != getSongs.Length - 1)
                {
                    Invoke(new Action(delegate { songs.Add(getSongs[i]); }));
                    i++;
                }
            })
            { IsBackground = false }.Start();

            new Thread(a =>
            {
                int i = 0;
                Banner[] getBanners = new Banner[0];
                try
                {
                    getBanners = Banner.GetBanners();
                }
                catch (Exception ex)
                {
                    var reslut = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Frame.Navigate(typeof(Fail), new Error { ErrorCode = ex.Message, ReTryPage = typeof(Home) });
                    });
                    while (true)
                    {
                        if (reslut.Status == AsyncStatus.Completed)
                        {
                            return;
                        }
                    }
                }

                while (i != getBanners.Length - 1)
                {
                    Invoke(new Action(delegate { banners.Add(getBanners[i]); }));
                    i++;
                }
            })
            { IsBackground = false }.Start();

            new Thread(a =>
            {
                int i = 0;
                Vocal[] getVocals = new Vocal[0];
                try
                {
                    getVocals = Vocal.GetVocalList("Id", "", 1, 20, "Watch", "desc");
                }
                catch (Exception ex)
                {
                    var reslut = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Frame.Navigate(typeof(Fail), new Error { ErrorCode = ex.Message, ReTryPage = typeof(Home) });
                    });
                    while (true)
                    {
                        if (reslut.Status == AsyncStatus.Completed)
                        {
                            return;
                        }
                    }
                }

                while (i != getVocals.Length - 1)
                {
                    Invoke(new Action(delegate { vocals.Add(getVocals[i]); }));
                    i++;
                }
            })
            { IsBackground = false }.Start();

            new Thread(a =>
            {
                int i = 0;
                SongListList[] getSongLists = new SongListList[0];
                try
                {
                    getSongLists = SongListList.GetSongListList("Id", "", 1, 20, "CreateTime", "desc");
                }
                catch (Exception ex)
                {
                    var reslut = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Frame.Navigate(typeof(Fail), new Error { ErrorCode = ex.Message, ReTryPage = typeof(Home) });
                    });
                    while (true)
                    {
                        if (reslut.Status == AsyncStatus.Completed)
                        {
                            return;
                        }
                    }
                }

                while (i != getSongLists.Length - 1)
                {
                    Invoke(new Action(delegate { songLists.Add(getSongLists[i]); }));
                    i++;
                }
            })
            { IsBackground = false }.Start();
        }

        private void RefreshContainer_RefreshRequested(RefreshContainer sender, RefreshRequestedEventArgs args)
        {

        }

        private void SongGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainPage.player.PlayListAddSong(songs[SongGridView.SelectedIndex]);
            MainPage.player.PlayIndex(MainPage.player.PlayList.IndexOf(songs[SongGridView.SelectedIndex]));
        }

        private void SongListGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Frame.Navigate(typeof(SongList), songLists[SongListGridView.SelectedIndex].Id);
        }

        private void SongGridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (SongGridView.SelectedIndex != -1)
            {
                if (MainPage.player.SongId == songs[SongGridView.SelectedIndex].Id)
                {
                    if (MainPage.player.IsPlay() == MediaTimelineControllerState.Running)
                    {
                        MainPage.player.Pause();
                    }
                    else
                    {
                        MainPage.player.Play();
                    }
                }
            }
        }

        private void VocalGridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (VocalGridView.SelectedIndex != -1)
            {
                Frame.Navigate(typeof(Page.VTuber), vocals[VocalGridView.SelectedIndex].Id);
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


