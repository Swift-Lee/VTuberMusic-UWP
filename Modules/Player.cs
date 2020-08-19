using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTuberMusic.Tools;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace VTuberMusic.Modules
{
    public class Player
    {
        private MediaPlayer player = new MediaPlayer();
        private MediaTimelineController playerTimelineController = new MediaTimelineController();
        private MediaPlaybackSession mediaPlayBackSession;
        public TypedEventHandler<MediaTimelineController, object> PlayerPositionChanged;
        public TypedEventHandler<MediaTimelineController, object> PlayerStateChanged;
        public TypedEventHandler<Player, object> SongChanged;
        public TypedEventHandler<Player, object> PlayListChanged;
        public TypedEventHandler<MediaPlaybackSession, object> BufferingProgressChanged;
        public string SongName { get; private set; } = "";
        public string VocalName { get; private set; } = "";
        public string SongImage { get; private set; } = "ms-appx:///Assets/Image/noimage.png";
        public string SongId { get; private set; } = "";
        public TimeSpan Position;
        public TimeSpan Duration;
        public ObservableCollection<Song> PlayList = new ObservableCollection<Song>();
        public int PlayListNowPlay = 0;
        public int PlayMode = 0; // 0=列表循环 1=单曲循环 2=随机播放
        public bool LoadComplete { get; private set; } = false;
        private SystemMediaTransportControls systemMediaTransportControls;
        public double BufferingProgress = 0;

        public Player()
        {
            // 绑定事件和控制器
            player.TimelineController = playerTimelineController;
            player.AudioCategory = MediaPlayerAudioCategory.Media;
            mediaPlayBackSession = player.PlaybackSession;
            playerTimelineController.PositionChanged += PlayerTimelineController_PositionChanged;
            playerTimelineController.StateChanged += PlayerTimelineController_StateChanged;
            mediaPlayBackSession.DownloadProgressChanged += MediaPlayBackSession_BufferingProgressChanged;
            // 全局播放控件
            systemMediaTransportControls = player.SystemMediaTransportControls;
            player.CommandManager.IsEnabled = false;
            // 启用相关按钮
            systemMediaTransportControls.IsEnabled = true;
            systemMediaTransportControls.ButtonPressed += MediaTransportButtonClik;
            Log.WriteLine("[Player]播放核心加载完成", Level.Info);
        }

        private void MediaSource_OpenOperationCompleted(MediaSource sender, MediaSourceOpenOperationCompletedEventArgs args)
        {
            Duration = sender.Duration.GetValueOrDefault();
            Log.WriteLine("[Player]载入成功! 音乐时长: " + Duration.ToString(), Level.Info);
            LoadComplete = true;
            playerTimelineController.Start();
        }

        #region 通过 Song Id 载入歌曲
        private void GetSong(Song song)
        {
            // 设置播放歌曲信息
            SongName = song.OriginName;
            VocalName = song.VocalName;
            SongImage = song.assestLink.CoverImg;
            SongId = song.Id;
            // 启用 Button 们
            systemMediaTransportControls.IsPlayEnabled = true;
            systemMediaTransportControls.IsPauseEnabled = true;
            systemMediaTransportControls.IsPreviousEnabled = true;
            systemMediaTransportControls.IsNextEnabled = true;
            systemMediaTransportControls.IsPauseEnabled = true;
            // 获取更新对象
            SystemMediaTransportControlsDisplayUpdater updater = systemMediaTransportControls.DisplayUpdater;
            // 设置媒体信息
            updater.Type = MediaPlaybackType.Music;
            updater.MusicProperties.Artist = song.VocalName;
            updater.MusicProperties.AlbumArtist = song.VocalName;
            updater.MusicProperties.Title = song.OriginName;
            // 设置图片
            updater.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(song.assestLink.CoverImg));
            // 更新
            updater.Update();
            // Log
            Log.WriteLine("[Player]载入歌曲 Id: " + song.Id, Level.Info);

            // 载入媒体
            LoadComplete = false;
            var mediaSource = MediaSource.CreateFromUri(new Uri(song.assestLink.Music));
            mediaSource.OpenOperationCompleted += MediaSource_OpenOperationCompleted;
            player.Source = mediaSource;
            Log.WriteLine("[Player]设置播放源为: " + song.assestLink.Music, Level.Info);
            // 触发缓冲进度修改和歌曲改变事件
            SongChanged(this, null);
            BufferingProgressChanged(mediaPlayBackSession, null);
        }
        #endregion

        #region 播放列表
        #region 添加删除歌曲
        public void PlayListAddSong(Song song)
        {
            if (PlayList.Count == 0)
            {
                PlayList.Add(song);
            }
            else
            {
                if (PlayList.IndexOf(song) == -1)
                {
                    PlayList.Add(song);
                }
            }
            PlayListChanged(this, null);
        }

        public void PlayListAddSongList(Song[] songs)
        {
            for (int i = 0; i != songs.Length; i++)
            {
                if (PlayList.All(p => p.Id != songs[i].Id))
                {
                    PlayList.Add(songs[i]);
                }
            }
            PlayListChanged(this, null);
        }

        public void PlayListDeleteSong(int index)
        {
            if(SongId == PlayList[index].Id)
            {
                PlayNext();
            }
            PlayList.Remove(PlayList[index]);
            PlayListChanged(this, null);
        }

        public void PlayListClear()
        {
            PlayList.Clear();
            SongId = "";
            SongName = "";
            SongImage = "ms-appx:///Assets/Image/noimage.png";
            VocalName = "";
            Pause();
            PlayListChanged(this, null);
        }
        #endregion

        #region 控制播放
        public void PlayNext()
        {
            if (PlayMode == 0)
            {
                if (PlayListNowPlay == PlayList.Count - 1)
                {
                    PlayIndex(0);
                }
                else
                {
                    PlayIndex(PlayListNowPlay + 1);
                }
            }
            else
            {
                // 随机播放没做
            }
        }

        public void PlayPrev()
        {
            if (PlayMode == 0)
            {
                if (PlayListNowPlay == 0)
                {
                    PlayIndex(PlayList.Count - 1);
                }
                else
                {
                    PlayIndex(PlayListNowPlay - 1);
                }
            }
            else
            {
                // 随机播放没做哦哦哦
            }
        }

        public void PlayIndex(int index)
        {
            GetSong(PlayList[index]);
            playerTimelineController.Start();
            playerTimelineController.Resume();
            PlayListNowPlay = index;
        }
        #endregion

        #region 事件

        #endregion
        #endregion

        #region 播放器控制
        #region 播放控制
        public void Play()
        {
            if (playerTimelineController.Position == TimeSpan.Zero)
            {
                playerTimelineController.Start();
            }
            else
            {
                playerTimelineController.Resume();
            }
            Log.WriteLine("[Player]开始播放 播放进度: " + Position.ToString(), Level.Info);
        }

        public void Pause()
        {
            playerTimelineController.Pause();
            Log.WriteLine("[Player]暂停播放 播放进度: " + Position, Level.Info);
        }

        public void SetPlayerPosition(TimeSpan position)
        {
            playerTimelineController.Position = position;
        }

        public void SetVol(double vol)
        {
            player.Volume = vol;
        }

        public MediaTimelineControllerState IsPlay()
        {
            return playerTimelineController.State;
        }

        public TimeSpan GetPlayerPosition()
        {
            return playerTimelineController.Position;
        }
        #endregion

        #region 事件监听
        private void PlayerTimelineController_PositionChanged(MediaTimelineController sender, object args)
        {
            Position = playerTimelineController.Position;
            if (Position > Duration)
            {
                if (LoadComplete)
                {
                    switch (PlayMode)
                    {
                        case 0:
                            PlayNext();
                            break;
                        case 1:
                            playerTimelineController.Position = TimeSpan.Zero;
                            break;
                        case 2:
                            // 随机播放还没做呢
                            break;
                    }
                }
            }
            PlayerPositionChanged(sender, args);
        }

        private void PlayerTimelineController_StateChanged(MediaTimelineController sender, object args)
        {
            // 通知系统我播放状态变了,再执行点其他操作
            switch (sender.State)
            {
                case MediaTimelineControllerState.Running:
                    systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Playing;
                    break;
                case MediaTimelineControllerState.Paused:
                    systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                case MediaTimelineControllerState.Error:
                    systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
                    Log.WriteLine("[Player]播放错误",Level.Error);
                    break;
                case MediaTimelineControllerState.Stalled:
                    systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Changing;
                    Log.WriteLine("[Player]缓冲中... 进度: " + BufferingProgress * 100,Level.Info);
                    break;
            }
            PlayerStateChanged(sender, args);
        }

        #endregion
        #endregion

        #region 系统全局播放控制
        private void MediaTransportButtonClik(SystemMediaTransportControls transportControls, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    Play();
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    Pause();
                    break;
                case SystemMediaTransportControlsButton.Next:
                    PlayNext();
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    PlayPrev();
                    break;
            }
        }
        #endregion

        #region 缓冲进度
        private void MediaPlayBackSession_BufferingProgressChanged(MediaPlaybackSession sender, object args)
        {
            BufferingProgress = sender.DownloadProgress;
            BufferingProgressChanged(sender, args);
        }

        #endregion
    }
}
