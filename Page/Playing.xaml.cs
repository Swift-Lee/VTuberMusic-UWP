using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VTuberMusic.Modules;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class Playing
    {

        public Playing()
        {
            this.InitializeComponent();
            MainPage.player.SongChanged += UpdateInfo;
            MusicName.Text = MainPage.player.SongName;
            Vocal.Text = MainPage.player.VocalName;
            BackgroundImage.Source = new BitmapImage(new Uri(MainPage.player.SongImage));
            if (MainPage.player.SongId == "")
            {
                Share.Visibility = Visibility.Collapsed;
                Comment.Visibility = Visibility.Collapsed;
            }
            else
            {
                Share.Visibility = Visibility.Visible;
                Comment.Visibility = Visibility.Visible;
            }
        }

        private void UpdateInfo(Player sender,object args)
        {
            Invoke(new Action(delegate
            {
                MusicName.Text = sender.SongName;
                Vocal.Text = sender.VocalName;
                BackgroundImage.Source = new BitmapImage(new Uri(sender.SongImage));
                if (MainPage.player.SongId == "")
                {
                    Share.Visibility = Visibility.Collapsed;
                    Comment.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Share.Visibility = Visibility.Visible;
                    Comment.Visibility = Visibility.Visible;
                }
            }));
        }

        #region Invoke
        public async void Invoke(Action action, Windows.UI.Core.CoreDispatcherPriority Priority = Windows.UI.Core.CoreDispatcherPriority.Normal)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); });
        }
        #endregion

        private void Share_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested += DataRequested;
            DataTransferManager.ShowShareUI();
        }

        private void DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            args.Request.Data.SetWebLink(new Uri("https://vtbmusic.com/song?id=" + MainPage.player.SongId));
            args.Request.Data.Properties.Title = "分享歌曲";
            args.Request.Data.Properties.Description = MainPage.player.SongName + " - " + MainPage.player.VocalName;
        }
    }
}
