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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace VTuberMusic.Page
{
    public sealed partial class Account
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        Modules.Account account;

        public Account()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (localSettings.Values["Token"] != null)
            {
                new Thread(a =>
                {
                    try
                    {
                        account = Modules.Account.GetAccountInfo((string)localSettings.Values["Token"]);
                        Invoke(new Action(delegate
                        {
                            // 显示信息
                            UserName.Text = account.RoleName;
                            // 判断性别
                            switch (account.Sex)
                            {
                                case 0:
                                    SexText.Text = "可能是女孩子";
                                    SexIcon.Foreground = new SolidColorBrush(Colors.Pink);
                                    SexIcon.Glyph = "\ue63a";
                                    break;
                                case 1:
                                    SexText.Text = "可能是男孩子";
                                    SexIcon.Foreground = new SolidColorBrush(Colors.DeepSkyBlue);
                                    SexIcon.Glyph = "\ue639";
                                    break;
                                default:
                                    SexText.Text = "可能男女都不是";
                                    break;
                            }
                            // 加载图片
                            var image = new BitmapImage();
                            image.UriSource = new Uri(account.Avatar);
                            PersonPicture.ProfilePicture = image;
                        }));
                    }
                    catch
                    {
                        Invoke(new Action(delegate
                        {
                            Frame.Navigate(typeof(Login));
                        }));
                    }
                })
                { IsBackground = true }.Start();
            }
            else
            {
                Frame.Navigate(typeof(Login));
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
