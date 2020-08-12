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
    class Player
    {
        private MediaPlayer player = new MediaPlayer();
        private MediaTimelineController playerTimelineController = new MediaTimelineController();
        public TypedEventHandler<MediaTimelineController, object> PlayerPositionChanged;
        public TypedEventHandler<MediaTimelineController, object> PlayerStateChanged;
        public TimeSpan Position;
        public TimeSpan Duration;

        public Player()
        {
            player.TimelineController = playerTimelineController;
            player.AudioCategory = MediaPlayerAudioCategory.Media;
            playerTimelineController.PositionChanged += PlayerTimelineController_PositionChanged;
            playerTimelineController.StateChanged += PlayerTimelineController_StateChanged;
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
            Log.WriteLine("[Player]载入媒体的时长: " + Duration.ToString(), Level.Info);
        }

        public void Play()
        {
            playerTimelineController.Resume();
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
    }
}
