using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTuberMusic.Tools;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml.Controls;

namespace VTuberMusic.Modules
{
    public class Player
    {
        private MediaPlayer player = new MediaPlayer();
        private MediaTimelineController playerTimelineController = new MediaTimelineController();
        public TypedEventHandler<MediaTimelineController, object> PlayerPositionChanged;
        public TypedEventHandler<MediaTimelineController, object> PlayerStateChanged;
        public TypedEventHandler<Player, object> SongChanged;
        public string SongName { get; private set; } = "";
        public string VocalName { get; private set; } = "";
        public string SongImage { get; private set; } = "ms-appx:///Assets/Image/noimage.png";
        public string SongId { get; private set; } = "";
        public TimeSpan Position;
        public TimeSpan Duration;

        public Player()
        {
            player.TimelineController = playerTimelineController;
            player.AudioCategory = MediaPlayerAudioCategory.Media;
            playerTimelineController.PositionChanged += PlayerTimelineController_PositionChanged;
            playerTimelineController.StateChanged += PlayerTimelineController_StateChanged;
            SongChanged += Noting;
            Log.WriteLine("[Player]播放核心加载完成", Level.Info);
        }

        public void SetSource(string Uri)
        {
            var mediaSource = MediaSource.CreateFromUri(new Uri(Uri));
            mediaSource.OpenOperationCompleted += MediaSource_OpenOperationCompleted;
            player.Source = mediaSource;
            Log.WriteLine("[Player]设置播放源为: " + Uri, Level.Info);
        }

        private void MediaSource_OpenOperationCompleted(MediaSource sender, MediaSourceOpenOperationCompletedEventArgs args)
        {
            Duration = sender.Duration.GetValueOrDefault();
            Log.WriteLine("[Player]媒体的时长: " + Duration.ToString(), Level.Info);
            playerTimelineController.Start();
        }

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

        public MediaTimelineControllerState IsPlay()
        {
            return playerTimelineController.State;
        }

        public TimeSpan GetPlayerPosition()
        {
            return playerTimelineController.Position;
        }

        public void SetPlayerPosition(TimeSpan position)
        {
            playerTimelineController.Position = position;
        }

        public void SetVol(double vol)
        {
            player.Volume = vol;
        }

        private void PlayerTimelineController_PositionChanged(MediaTimelineController sender, object args)
        {
            Position = playerTimelineController.Position;
            PlayerPositionChanged(sender, args);
        }

        private void PlayerTimelineController_StateChanged(MediaTimelineController sender, object args)
        {
            PlayerStateChanged(sender, args);
        }

        private void Noting(object o1,object o2)
        {

        }

        public void GetSong(Song song)
        {
            // 设置播放歌曲信息
            SongName = song.OriginName;
            VocalName = song.VocalName;
            SongImage = song.assestLink.CoverImg;
            SongId = song.Id;
            Log.WriteLine("[Player]载入歌曲 Id: " + song.Id, Level.Info);
            
            // 载入媒体
            var mediaSource = MediaSource.CreateFromUri(new Uri(song.assestLink.Music));
            mediaSource.OpenOperationCompleted += MediaSource_OpenOperationCompleted;
            player.Source = mediaSource;
            Log.WriteLine("[Player]设置播放源为: " + song.assestLink.Music, Level.Info);
            SongChanged(this, true);
        }
    }
}
