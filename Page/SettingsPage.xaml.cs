using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VTuberMusic.Tools;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
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
    public sealed partial class SettingsPage
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void HXD_is_me_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Settings.HXD_is_me.HXD));
        }

        private void Log_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Settings.Log));
        }

        private void ScrollViewer_KeyUp(object sender, KeyRoutedEventArgs e)
        {
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
            if (IsCtrlKeyPressed())
            {
                if (IsKeyPressed(VirtualKey.X))
                {
                    if (IsKeyPressed(VirtualKey.P))
                    {
                        Tools.Log.WriteLine("https://s1.ax1x.com/2020/08/15/dk12Js.gif", Level.Info);
                    }
                }
            }
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            Tools.Log.LogText = "";
        }
    }
}
