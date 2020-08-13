using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VTuberMusic.Modules;
using Windows.ApplicationModel.Core;
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
        }

        private void UpdateInfo(Player sender,object args)
        {
            Invoke(new Action(delegate
            {
                MusicName.Text = sender.SongName;
                Vocal.Text = sender.VocalName;
                BackgroundImage.Source = new BitmapImage(new Uri(sender.SongImage));
            }));
        }

        #region Invoke
        public async void Invoke(Action action, Windows.UI.Core.CoreDispatcherPriority Priority = Windows.UI.Core.CoreDispatcherPriority.Normal)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); });
        }
        #endregion
    }
}
