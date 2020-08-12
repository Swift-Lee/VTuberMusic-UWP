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
        public TimeSpan duration;

        public Player()
        {
            player.TimelineController = playerTimelineController;
            player.AudioCategory = MediaPlayerAudioCategory.Media;
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
            duration = sender.Duration.GetValueOrDefault();
            Log.WriteLine("[Player]载入媒体的时长: " + duration.ToString(), Level.DeBug);
        }

        public void Play()
        {
            var position = playerTimelineController.Position;
            playerTimelineController.Start();
            playerTimelineController.Position = position;
            Log.WriteLine("[Player]开始播放 播放进度: " + position.ToString(), Level.Info);
        }

        public void Pause()
        {
            var position = playerTimelineController.Position;
            playerTimelineController.Pause();
            Log.WriteLine("[Player]暂停播放 播放进度: " + position.ToString(), Level.Info);
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
    }
}
