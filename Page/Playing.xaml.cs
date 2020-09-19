using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using VTuberMusic.Modules;
using VTuberMusic.Network.GetTools;
using VTuberMusic.Tools;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Foundation;
using Windows.UI.Xaml.Navigation;

namespace VTuberMusic.Page
{

    public sealed partial class Playing
    {
        private ObservableCollection<WordWithTranslate> lrcs = new ObservableCollection<WordWithTranslate>();
        private Timer lyricTimer = null;
        private int NowLyricWord = -1;
        private bool loadingComplete = false;

        public Playing()
        {
            this.InitializeComponent();
            // 
            // 绑定歌曲更改事件
            MainPage.player.SongChanged += UpdateInfo;
            MainPage.player.PlayerStateChanged += PlayModeChange;
            // 更新歌曲信息
            MusicName.Text = MainPage.player.SongName;
            Vocal.Text = MainPage.player.VocalName;
            BackgroundImage.Source = new BitmapImage(new Uri(MainPage.player.SongImage));
            // 判断是否显示分享评论
            if (MainPage.player.SongId == "")
            {
                Share.Visibility = Visibility.Collapsed;
                Comment.Visibility = Visibility.Collapsed;
            }
            else
            {
                Share.Visibility = Visibility.Visible;
                Comment.Visibility = Visibility.Visible;
                #region 载入歌词
                // 多线程加载歌词
                lyricTimer = new Timer(new TimerCallback(LyricTick), null, Timeout.Infinite, 100);
                new Thread(a =>
                {
                    string jsonText = GetTools.GetRequest(MainPage.player.songObject.assestLink.Lyric);
                    Invoke(new Action(delegate
                    {
                        Lyric getLyric = Lyric.ParseLyric(jsonText);
                        lrcs.Clear();
                        for (int i = 0; i != getLyric.parse.Length; i++)
                        {
                            lrcs.Add(getLyric.parse[i]);
                        }

                        loadingComplete = true;
                    }));

                    lyricTimer.Change(0, 100);
                    GC.Collect();
                }).Start();
                #endregion
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if(lyricTimer != null)
            {
                lyricTimer.Dispose();
            }
            NowLyricWord = -1;
            GC.Collect();
        }

        #region 滚动歌词
        private void PlayModeChange(object sender, object args)
        {
            switch (((MediaTimelineController)sender).State) {
                case MediaTimelineControllerState.Running:
                    lyricTimer = new Timer(new TimerCallback(LyricTick), null, 0, 100);
                    break;
                default:
                    if (lyricTimer != null)
                    {
                        lyricTimer.Dispose();
                    }
                    break;
            }
        }

        private void LyricTick(object state)
        {
            if (!loadingComplete)
            {
                
            }
            int playLyric = 0;
            for (int i = 0; i != lrcs.Count; i++)
            {
                if(i + 1 < lrcs.Count)
                {
                    if (MainPage.player.Position > lrcs[i].Time && MainPage.player.Position < lrcs[i + 1].Time)
                    {
                        playLyric = i;
                        break;
                    }
                }
                else
                {
                    if(i != lrcs.Count)
                    {
                        playLyric = lrcs.Count - 1;
                    }
                }
            }
            if (playLyric != NowLyricWord)
            {
                Invoke(new Action(delegate
                {
                    ToLyric(playLyric);
                }));
            }
        }

        #region 跳转到指定歌词
        private void ToLyric(int Index)
        {
            if (loadingComplete)
            {
                if (NowLyricWord != -1)
                {
                    lrcs[NowLyricWord] = new WordWithTranslate
                    {
                        brush = new SolidColorBrush(Colors.Black),
                        Id = lrcs[NowLyricWord].Id,
                        Orgin = lrcs[NowLyricWord].Orgin,
                        Translate = lrcs[NowLyricWord].Translate,
                        Time = lrcs[NowLyricWord].Time
                    };
                    lrcs[Index] = new WordWithTranslate
                    {
                        brush = new SolidColorBrush(Colors.White),
                        Id = lrcs[Index].Id,
                        Orgin = lrcs[Index].Orgin,
                        Translate = lrcs[Index].Translate,
                        Time = lrcs[Index].Time
                    };
                    NowLyricWord = Index;
                    UpdateLayout();
                    var item = (UIElement)LyricControl.ContainerFromItem(LyricControl.Items[Index]);
                    GeneralTransform generalTransform = LyricScrollViewer.TransformToVisual(item);
                    Point point = generalTransform.TransformPoint(new Point());
                    point.Y = -point.Y;
                    if(LyricScrollViewer != null && ((ContentPresenter)item) != null)
                    {
                        LyricScrollViewer.ChangeView(0, point.Y + LyricScrollViewer.VerticalOffset - (LyricScrollViewer.ActualHeight / 3) - (((ContentPresenter)item).ActualHeight / 2), null);
                    }
                    UpdateLayout();
                }
                else
                {
                    NowLyricWord = Index;
                    lrcs[NowLyricWord] = new WordWithTranslate
                    {
                        brush = new SolidColorBrush(Colors.White),
                        Id = lrcs[NowLyricWord].Id,
                        Orgin = lrcs[NowLyricWord].Orgin,
                        Translate = lrcs[NowLyricWord].Translate,
                        Time = lrcs[NowLyricWord].Time
                    };
                    UpdateLayout();
                    var item = (UIElement)LyricControl.ContainerFromItem(LyricControl.Items[Index]);
                    GeneralTransform generalTransform = LyricScrollViewer.TransformToVisual(item);
                    Point point = generalTransform.TransformPoint(new Point());
                    if(point.Y < 0)
                    {
                        Log.WriteLine("bf:"+point.Y.ToString());
                        point.Y = - point.Y;
                    }

                    if (LyricScrollViewer != null && ((ContentPresenter)item) != null)
                    {
                        LyricScrollViewer.ChangeView(0, point.Y, null);
                    }
                    UpdateLayout();
                }
            }
        }
        #endregion
        #endregion

        #region 更新播放信息
        private void UpdateInfo(Player sender,object args)
        {
            Invoke(new Action(delegate
            {
                MusicName.Text = sender.SongName;
                Vocal.Text = sender.VocalName;
                var image = new BitmapImage();
                image.UriSource = new Uri(sender.SongImage);
                BackgroundImage.Source = image;
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

            // 多线程加载歌词
            #region 加载歌词
            new Thread(a =>
            {
                lyricTimer.Dispose();
                string jsonText = GetTools.GetRequest(MainPage.player.songObject.assestLink.Lyric);
                Invoke(new Action(delegate
                {
                    loadingComplete = false;
                    Lyric getLyric = Lyric.ParseLyric(jsonText);
                    lrcs.Clear();
                    foreach (WordWithTranslate lycTemp in getLyric.parse)
                    {
                        lrcs.Add(lycTemp);
                    }

                    NowLyricWord = -1;
                    loadingComplete = true;
                    lyricTimer = new Timer(new TimerCallback(LyricTick), null, 0, 100);
                }));
            }).Start();
            #endregion
        }
        #endregion

        #region Invoke
        private void Invoke(Action action, Windows.UI.Core.CoreDispatcherPriority Priority = Windows.UI.Core.CoreDispatcherPriority.Normal)
        {
            var task = Task.Run(new Action(async delegate { await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); }); }));
        }
        #endregion

        #region 分享
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
        #endregion
    }
}
