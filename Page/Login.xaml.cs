using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using VTuberMusic.Modules;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class Login
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public Login()
        {
            this.InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if(LoginUserName.Text != "" && LoginPassword.Password != "")
            {
                string userName = LoginUserName.Text;
                string password = LoginPassword.Password;
                ProgressPanel.Visibility = Visibility.Visible;
                new Thread(a =>
                {
                    try
                    {
                        var result = Modules.Account.GetToken(userName, password);
                        localSettings.Values["Token"] = result;
                        Invoke(new Action(delegate
                        {
                            Frame.Navigate(typeof(Account));
                        }));
                    }
                    catch (Exception ex)
                    {
                        Invoke(new Action(delegate
                        {
                            ProgressPanel.Visibility = Visibility.Collapsed;
                            LoginStatue.Visibility = Visibility.Visible;
                            LoginStatue.Foreground = new SolidColorBrush(Colors.Red);
                            LoginStatue.Text = ex.Message;
                        }));
                    }
                }).Start();
            }
            else
            {
                LoginStatue.Visibility = Visibility.Visible;
                LoginStatue.Foreground = new SolidColorBrush(Colors.Red);
                LoginStatue.Text = "请输入用户名和密码";
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
