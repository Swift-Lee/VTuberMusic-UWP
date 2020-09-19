using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
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
using Windows.Media.Playback;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace VTuberMusic
{
    public sealed partial class MainPage
    {
        public static Frame pageFrame;
        public static NavigationView navigationView;
        public static Player player;
        private bool ok = false;
        private Error error = null;
        ObservableCollection<Song> playList = new ObservableCollection<Song>();

        #region Item Tag 属性对应的页面
        public static readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("Home",typeof(Page.Home)),
            ("Subscribe",typeof(Page.Subscribe)),
            ("SongList",typeof(Page.SongList)),
            ("Account",typeof(Page.Account)),
            ("Playing",typeof(Page.Playing)),
            ("History",typeof(Page.History)),
            ("Login",typeof(Page.Login))
        };
        #endregion

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
            //Tools.Version.Build = Tools.Version.GetBuild(File.GetLastWriteTime(GetType().Assembly.Location));
            // 输出 Build 版本号和版权信息
            Log.WriteLine("VTuberMusic-UWP " + Tools.Version.VersionNum + " Build:" + Tools.Version.Build, Level.Info);
            Log.WriteLine("Copyright ©  2020 VTuberMusic", Level.Info);
            // 导航侧边栏扩展到标题栏
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            // 让其他对象控制导航视图
            navigationView = TheNavigationView;
            Log.WriteLine("[UI]准备就绪", Level.Info);
            // 获取 CDN 列表并保存
            try
            {
                GetTools.CDNList = GetTools.GetCDNList();
            }
            catch (Exception ex)
            {
                error = new Error { CanBackHome = false, ReTryArgs = null, ReTryPage = typeof(MainPage), ErrorCode = ex.Message };
            }
            // 初始化播放核心
            player = new Player();
            DispatcherTimer timer = new DispatcherTimer();
            player.PlayerPositionChanged += PlayerPositionChanged;
            player.PlayerStateChanged += PlayerStateChanged;
            player.SongChanged += SongChanged;
            player.PlayListChanged += PlayListUpdate;
            player.BufferingProgressChanged += BufferingProgressChanged;
            ok = true;
            // 更新播放列表标题
            PlayListNum.Text = string.Format(Lang.ReadLangText("PlayList"), "0");
            // 创建播放列表阴影
            // 释放内存
            GC.Collect();
        }

        #region 页面加载完成
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (error != null)
            {
                Frame.Navigate(typeof(Page.Fail), error);
            }
        }
        #endregion

        #region NavigationView 相关
        #region View 加载
        private void TheNavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            // 绑定跳转事件
            PageFrame.Navigated += PageFrame_Navigated; ;
            // 跳转到首页
            NavView_Navigate("Home", new EntranceNavigationTransitionInfo());
            // 添加返回键快捷方式 
            var goBack = new KeyboardAccelerator { Key = Windows.System.VirtualKey.GoBack };
            goBack.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(goBack);
            // Alt 键扩展
            var altLeft = new KeyboardAccelerator
            {
                Key = Windows.System.VirtualKey.Left,
                Modifiers = Windows.System.VirtualKeyModifiers.Menu
            };
            altLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(altLeft);
        }
        #endregion

        #region BackPage
        // 返回按钮
        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            On_BackRequested();
        }

        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            On_BackRequested();
            args.Handled = true;
        }

        private bool On_BackRequested()
        {
            if (!PageFrame.CanGoBack)
                return false;

            // 如果侧边栏被隐藏不返回
            if (TheNavigationView.IsPaneOpen &&
                (TheNavigationView.DisplayMode == NavigationViewDisplayMode.Compact ||
                 TheNavigationView.DisplayMode == NavigationViewDisplayMode.Minimal))
                return false;

            PageFrame.GoBack();
            return true;
        }
        #endregion

        #region Frame 跳转事件
        private void PageFrame_Navigated(object sender, NavigationEventArgs e)
        {
            TheNavigationView.IsBackEnabled = PageFrame.CanGoBack;

            if (PageFrame.SourcePageType == typeof(Page.SettingsPage))
            {
                TheNavigationView.SelectedItem = (NavigationViewItem)TheNavigationView.SettingsItem;
            }
            else if (PageFrame.SourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);
                try
                {
                    TheNavigationView.SelectedItem = TheNavigationView.MenuItems
                    .OfType<NavigationViewItem>()
                    .First(n => n.Tag.Equals(item.Tag));
                }
                catch { }
            }
        }
        #endregion

        #region 跳转页面
        private void NavView_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            // 判断是否是跳转到错误页
            if (navItemTag == "settings")
            {
                _page = typeof(Page.SettingsPage);
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;
            }
            // 防止重复跳转页面
            var preNavPageType = PageFrame.CurrentSourcePageType;

            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                PageFrame.Navigate(_page, null, transitionInfo);
            }
        }
        #endregion

        #region NavigationView Item
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

        #region Footer
        private void AccountItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavView_Navigate(((NavigationViewItem)sender).Tag.ToString(), new EntranceNavigationTransitionInfo());
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
                    Invoke(new Action(delegate {
                        LoadingRing.IsActive = false;
                        PlayIcon.Visibility = Visibility.Visible;
                        PlayIcon.Symbol = Symbol.Pause; 
                    }));
                    break;
                case MediaTimelineControllerState.Paused:
                    Invoke(new Action(delegate {
                        LoadingRing.IsActive = false;
                        PlayIcon.Symbol = Symbol.Play;
                        PlayIcon.Visibility = Visibility.Visible;
                    }));
                    break;
                case MediaTimelineControllerState.Error:
                    Invoke(new Action(delegate {
                        LoadingRing.IsActive = false;
                        PlayIcon.Symbol = Symbol.Clear;
                        PlayIcon.Visibility = Visibility.Visible;
                    }));
                    break;
                case MediaTimelineControllerState.Stalled:
                    Invoke(new Action(delegate {
                        LoadingRing.IsActive = true;
                        PlayIcon.Visibility = Visibility.Collapsed;
                    }));
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
                GC.Collect();
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

        private void BufferingProgressChanged(MediaPlaybackSession sender, object args)
        {
            Invoke(new Action(delegate { 
                LoadingBar.Value = player.BufferingProgress * 100;
            }));
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (player.PlayList.Count != 0)
            {
                player.PlayNext();
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (player.PlayList.Count != 0)
            {
                player.PlayPrev();
            }
        }

        private void PlayListDelSong_Click(object sender, RoutedEventArgs e)
        {
            Button clikButton = (Button)sender;
            player.PlayListDeleteSong(player.PlayListFindSongIndex((string)clikButton.Tag));
        }
    }
}
