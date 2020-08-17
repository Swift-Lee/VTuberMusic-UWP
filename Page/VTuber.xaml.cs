using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VTuberMusic.Modules;
using VTuberMusic.Tools;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace VTuberMusic.Page
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class VTuber
    {
        string vocalId;
        ObservableCollection<Song> songs = new ObservableCollection<Song>();

        public VTuber()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            vocalId = (string)e.Parameter;
            Vocal vocal = Vocal.GetVocalObject(vocalId);
            BitmapImage vocalImage = new BitmapImage(new Uri(vocal.AvatarImg));
            BackgroundImage.Source = vocalImage;
            VocalImage.ProfilePicture = vocalImage;
            OriginalName.Text = vocal.OriginalName;
            ChineseName.Text = vocal.ChineseName;
            VocalGroup.Text = string.Format(Lang.ReadLangText("VTuberGroup"), vocal.GroupsId);
            Song[] songsArray = Song.GetMusicList("VocalId", vocalId, 1, 100, "CreateTime", "desc");
            for (int i = 0; i != songsArray.Length; i++)
            {
                songs.Add(songsArray[i]);
            }
            // VocalLang.Text = string.Format(Lang.ReadLangText("VTuberLang"));
        }

        private void SongListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
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
    }
}
