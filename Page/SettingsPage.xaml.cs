using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using VTuberMusic.Tools;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
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
    public sealed partial class SettingsPage
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        Modules.Account account;

        public SettingsPage()
        {
            this.InitializeComponent();
            if (localSettings.Values["Token"] != null)
            {
                new Thread(a =>
                {
                    account = Modules.Account.GetAccountInfo((string)localSettings.Values["Token"]);
                    Invoke(new Action(delegate
                    {
                        UserName.Text = account.RoleName;
                        UserRealName.Text = account.UserName;
                        UserId.Text = "Uid: " + account.Id;
                        LogOut.Visibility = Visibility.Visible;
                        var image = new BitmapImage();
                        image.UriSource = new Uri(account.Avatar);
                        UserAvatar.ProfilePicture = image;
                    }));
                })
                { IsBackground = true }.Start();
            }
            BuildVersion.Text = string.Format(Lang.ReadLangText("BuildVersion"), Tools.Version.VersionNum, Tools.Version.Build);
        }

        private void HXD_is_me_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Settings.HXD_is_me.HXD));
        }

        private void Log_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Settings.Log));
        }

        private static bool IsCtrlKeyPressed()
        {
            var ctrlState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Control);
            return (ctrlState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }

        private static bool IsKeyPressed(VirtualKey key)
        {
            var keyState = CoreWindow.GetForCurrentThread().GetKeyState(key);
            return (keyState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }

        private void ScrollViewer_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (IsCtrlKeyPressed() && IsKeyPressed(VirtualKey.X) && IsKeyPressed(VirtualKey.P))
            {
                Tools.Log.WriteLine("[恭喜!你发现了新大陆]https://s1.ax1x.com/2020/08/15/dk12Js.gif", Level.Info);
            }
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            Tools.Log.LogText = "";
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            UserName.Text = "";
            UserAvatar.ProfilePicture = null;
            UserId.Text = "";
            UserRealName.Text = "";
            localSettings.Values["Token"] = null;
            LogOut.Visibility = Visibility.Collapsed;
            Tools.Log.WriteLine("[账户]已登出账户", Level.Info);
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
