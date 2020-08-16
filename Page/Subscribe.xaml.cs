using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VTuberMusic.Modules;
using VTuberMusic.Tools;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class Subscribe
    {
        Vocal[] vocals = Vocal.GetVocalList("", "", 1, 50, "Watch", "desc");

        public Subscribe()
        {
            InitializeComponent();
            // Title.Text = "我的主推 (" + vocals.Length + ")";
            Title.Text = string.Format(Lang.ReadLangText("MySubscrubeTitle"), vocals.Length.ToString());
        }
    }
}
