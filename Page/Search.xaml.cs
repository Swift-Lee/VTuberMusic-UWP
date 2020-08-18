using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VTuberMusic.Modules;
using VTuberMusic.Tools;
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
                case 0:
                    Log.WriteLine("[UI]搜索音乐: " + searchText, Level.Info);
                    Song[] songsArray;
                    songsArray = Song.GetMusicList("OriginName", searchText, 1, 50, "OriginName", "dasc");
                    songs.Clear();
                    if (songsArray[0].Id != "")
                    {
                        for (int i = 0; i != songsArray.Length; i++)
                        {
                            songs.Add(songsArray[i]);
                        }
                    }
                    else
                    {
                        SongFailText.Text = "找不到内容";
                        Log.WriteLine("[UI]搜索音乐: " + searchText + " 失败", Level.Error);
                    }
                    break;
                case 1:
                    Vocal[] vocalsArray;
                    Log.WriteLine("[UI]搜索 VTuber: " + searchText, Level.Info);
                    vocalsArray = Vocal.GetVocalList("OriginalName", searchText, 1, 50, "OriginalName", "desc");
                    vocals.Clear();
                    if (vocalsArray[0].Id != "")
                    {
                        for (int i = 0; i != vocalsArray.Length; i++)
                        {
                            vocals.Add(vocalsArray[i]);
                        }
                    }
                    else
                    {
                        VTuberFailText.Text = "找不到内容";
                        Log.WriteLine("[UI]搜索 VTuber: " + searchText + " 失败", Level.Error);
                    }
                    break;
                case 2:
                    SongListList[] songListArray;
                    Log.WriteLine("[UI]搜索歌单: " + searchText, Level.Info);
                    songListArray = SongListList.GetSongListList("Name", searchText, 1, 50, "Name", "desc");
                    songLists.Clear();
                    if (songListArray[0].Id != "")
                    {
                        for (int i = 0; i != songListArray.Length; i++)
                        {
                            songLists.Add(songListArray[i]);
                        }
                    }
                    else
                    {
                        SongListFailText.Text = "找不到内容";
                        Log.WriteLine("[UI]搜索歌单: " + searchText + " 失败", Level.Error);
                    }
                    break;
            }
        }

        private void SongView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.ItemIndex % 2 == 0)
            {
                args.ItemContainer.Background = new SolidColorBrush(Colors.WhiteSmoke);
            }
            else
            {
                args.ItemContainer.Background = new SolidColorBrush(Colors.White);
            }
        }

        private void VTuberView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.ItemIndex % 2 == 0)
            {
                args.ItemContainer.Background = new SolidColorBrush(Colors.WhiteSmoke);
            }
            else
            {
                args.ItemContainer.Background = new SolidColorBrush(Colors.White);
            }
        }

        private void SongListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.ItemIndex % 2 == 0)
            {
                args.ItemContainer.Background = new SolidColorBrush(Colors.WhiteSmoke);
            }
            else
            {
                args.ItemContainer.Background = new SolidColorBrush(Colors.White);
            }
        }
    }
}