using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using VTuberMusic.Modules;
using VTuberMusic.Network.GetTools;
using VTuberMusic.Tools;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace VTuberMusic
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    /// 

    public sealed partial class MainPage
    {
        public static Frame pageFrame;
        public static NavigationView navigationView;
        private static Player player;
        public static DispatcherTimer timer = new DispatcherTimer();
        private bool ok = false;

        #region Item Tag 属性对应的页面
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("Home",typeof(Page.Home)),
            ("Subscribe",typeof(Page.Subscribe)),
            ("SongList",typeof(Page.SongList)),
            ("Account",typeof(Page.Account)),
            ("Playing",typeof(Page.Playing)),
        };
        #endregion

        public MainPage()
        {
            InitializeComponent();
            // 输出 Build 版本号和版权信息
            Log.WriteLine("VTuberMusic-UWP Alpha v1.0 Build:" + Tools.Version.GetBuild(File.GetLastWriteTime(GetType().Assembly.Location)), Level.Info);
            Log.WriteLine("Copyright ©  2020 VTuberMusic", Level.Info);
            Log.WriteLine(GetTools.GetHitokoto(), Level.Info);
            // 导航侧边栏扩展到标题栏
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            GetTools.CDNList = GetTools.GetCDNList();
            player = new Player();
            DispatcherTimer timer = new DispatcherTimer();
            player.SetSource("https://santiego.gitee.io/vtb-music-source-song/song/1017.mp3");
            ok = true;

            navigationView = TheNavigationView;
            navigationView.SelectedItem = Home;
            pageFrame = PageFrame;
        }

        #region NavigationView 相关
        #region 监听 NavigationView
        private void TheNavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                NavView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            if (navItemTag == "settings")
            {
                _page = typeof(Page.SettingsPage);
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;
            }
            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = PageFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                PageFrame.Navigate(_page, null, transitionInfo);
            }
        }

        private void TheNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected == true)
            {
                NavView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.SelectedItemContainer != null)
            {
                var navItemTag = args.SelectedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }
        #endregion

        #region Footer 按钮点击事件
        private void AccountNavigationViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // TheNavigationView.SelectedItem = AccountNavigationViewItem;
            PageFrame.Navigate(typeof(Page.Account));
        }
        #endregion
        #endregion

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (player.IsPlay() == MediaTimelineControllerState.Running)
            {
                timer.Stop();
                player.Pause();
                PlayIcon.Symbol = Symbol.Play;
            }
            else if (player.IsPlay() == MediaTimelineControllerState.Paused)
            {
                timer.Interval = TimeSpan.FromSeconds(0.1);
                timer.Tick += timer_Tick;
                timer.Start();
                player.Play();
                PlayIcon.Symbol = Symbol.Pause;
            }
        }


        private void PlayerTimeLine_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            player.SetPlayerPosition(TimeSpan.FromSeconds(PlayerTimeLine.Value));
        }

        private void Vol_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (ok)
            {
                player.SetVol(Vol.Value / 100);
            }
        }

        #region 播放进度更新计时器事件
        void timer_Tick(object sender, object e)
        {
            PlayerTimeLine.Maximum = player.duration.TotalSeconds;
            PlayerTimeLine.Value = (player.GetPlayerPosition()).TotalSeconds;
            PlayerTime.Text = player.GetPlayerPosition().ToString(@"mm\:ss");
            PlayerTotalTime.Text = player.duration.ToString(@"mm\:ss");
            if (PlayerTimeLine.Value == PlayerTimeLine.Maximum)
            {
                player.SetPlayerPosition(TimeSpan.FromSeconds(0));
                player.Pause();
                PlayIcon.Symbol = Symbol.Play;
            }
        }
        #endregion

        #region 正在播放 Grid 的点击和鼠标在上效果 (问就 Grid 没有 Template)
        private void PlayingGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TheNavigationView.SelectedItem = Playing;
            PlayingGrid.Background = new SolidColorBrush(Color.FromArgb(40, 0, 0, 0));
        }

        private void PlayingGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            PlayingGrid.Background = new SolidColorBrush(Color.FromArgb(20, 0, 0, 0));
        }

        private void PlayingGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            PlayingGrid.Background = null;
        }

        private void PlayingGrid_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            PlayingGrid.Background = null;
        }
        #endregion
    }
}
