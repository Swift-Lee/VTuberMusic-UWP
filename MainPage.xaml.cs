using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Media.Imaging;
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
        public static Player player;
        private bool ok = false;
        ObservableCollection<Song> playList = new ObservableCollection<Song>();

        #region Item Tag 属性对应的页面
        public static readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
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
            Tools.Version.Build = Tools.Version.GetBuild(File.GetLastWriteTime(GetType().Assembly.Location));
            // 输出 Build 版本号和版权信息
            Log.WriteLine("VTuberMusic-UWP " + Tools.Version.VersionNum + " Build:" + Tools.Version.Build, Level.Info);
            Log.WriteLine("Copyright ©  2020 VTuberMusic", Level.Info);
            // 导航侧边栏扩展到标题栏
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            // 让其他对象控制页面
            navigationView = TheNavigationView;
            Log.WriteLine("[UI]准备就绪", Level.Info);
            // 获取 CDN 列表并且保存
            GetTools.CDNList = GetTools.GetCDNList();
            // 初始化播放核心
            player = new Player();
            DispatcherTimer timer = new DispatcherTimer();
            player.PlayerPositionChanged += PlayerPositionChanged;
            player.PlayerStateChanged += PlayerStateChanged;
            player.SongChanged += SongChanged;
            player.PlayListChanged += PlayListUpdate;
            ok = true;
            // 跳转到首页
            navigationView.SelectedItem = Home;
            Log.WriteLine("[UI]跳转到首页", Level.Info);
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

        #region 播放控制按钮
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if(player.Duration != TimeSpan.Zero)
            {
                switch (player.IsPlay())
                {
                    case MediaTimelineControllerState.Running:
                        player.Pause();
                        break;
                    case MediaTimelineControllerState.Paused:
                        player.Play();
                        break;
                }
            }
        }
        #endregion

        #region 播放列表更新
        private void PlayListUpdate(object obj, object args)
        {
            PlayListNum.Text = string.Format(Lang.ReadLangText("PlayList"), player.PlayList.Count.ToString());
            playList.Clear();
            for (int i = 0; i != player.PlayList.Count; i++)
            {
                playList.Add(player.PlayList[i]);
            }
        }
        #endregion

        #region 播放进度条和音量被更改
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
        #endregion

        #region 播放进度更新事件
        private void PlayerPositionChanged(MediaTimelineController sender, object args)
        {
            Invoke(new Action(delegate { 
                PlayerTimeLine.Maximum = player.Duration.TotalSeconds;
                PlayerTimeLine.Value = (player.GetPlayerPosition()).TotalSeconds;
                PlayerTime.Text = player.GetPlayerPosition().ToString(@"mm\:ss");
                PlayerTotalTime.Text = player.Duration.ToString(@"mm\:ss");
            }));
        }
        #endregion

        #region 播放状态更新事件
        private void PlayerStateChanged(MediaTimelineController sender, object e)
        {
            switch (sender.State)
            {
                case MediaTimelineControllerState.Running:
                    Invoke(new Action(delegate { PlayIcon.Symbol = Symbol.Pause; }));
                    break;
                case MediaTimelineControllerState.Paused:
                    Invoke(new Action(delegate { PlayIcon.Symbol = Symbol.Play; }));
                    break;
                case MediaTimelineControllerState.Error:
                    Invoke(new Action(delegate { PlayIcon.Symbol = Symbol.Clear; }));
                    break;
                default:
                    Invoke(new Action(delegate { PlayIcon.Symbol = Symbol.Play; }));
                    break;
            }
        }
        #endregion

        #region 播放歌曲属性更新
        private void SongChanged(Player sender,object args)
        {
            Invoke(new Action(delegate
            {
                BitmapImage SongImageBitmap = new BitmapImage(new Uri(sender.SongImage));
                SongName.Text = sender.SongName;
                VocalName.Text = sender.VocalName;
                SongImage.Source = SongImageBitmap;
                BackgroudImage.Source = SongImageBitmap;
                
            }));
        }
        #endregion

        #region Invoke
        public async void Invoke(Action action, Windows.UI.Core.CoreDispatcherPriority Priority = Windows.UI.Core.CoreDispatcherPriority.Normal)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); });
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

        private void Search_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            TheNavigationView.SelectedItem = null;
            PageFrame.Navigate(typeof(Page.Search),sender.Text);
        }

        private void SongList_Click(object sender, RoutedEventArgs e)
        {
            if (PlayListWindow.Visibility == Visibility.Visible)
            {
                PlayListWindow.Visibility = Visibility.Collapsed;
            }
            else
            {
                PlayListWindow.Visibility = Visibility.Visible;
            }
        }

        private void ClearPlayList_Click(object sender, RoutedEventArgs e)
        {
            player.PlayListClear();
        }

        private void PlayListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
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

        private void PlayMode_Click(object sender, RoutedEventArgs e)
        {
            switch (player.PlayMode)
            {
                case 0:
                    player.PlayMode = 1;
                    PlayModeIcon.Symbol = Symbol.RepeatOne;
                    break;
                case 1:
                    player.PlayMode = 2;
                    PlayModeIcon.Symbol = Symbol.Shuffle;
                    break;
                case 2:
                    player.PlayMode = 0;
                    PlayModeIcon.Symbol = Symbol.RepeatAll;
                    break;
            }
        }
    }
}
