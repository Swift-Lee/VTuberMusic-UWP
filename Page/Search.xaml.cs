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
        Song[] songs;
        
        public Search()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            searchText = (string)e.Parameter;
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pivot = (Pivot)sender;
            switch (pivot.SelectedIndex) {
                case 0:
                    songs = Song.GetMusicList("OriginName", searchText, 1, 10, "OriginName", "dasc");
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }
        }
    }
}

