using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;


//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace App1
{
    public sealed partial class InfoCard : UserControl
    {
        public InfoCard()
        {
            this.InitializeComponent();
        }

        public string inputwhensearch;

        public void setAlbumInfo(string albumPic,string albumName,string singername,string publictime,string input)
        {
            Album.Visibility = Visibility.Visible;
            AlbumPicture.Source = new BitmapImage(new Uri(albumPic, UriKind.Absolute));
            AlbumTitle.Text = albumName;
            AlbumSingerName.Text = singername;
            AlbumPublicTime.Text = publictime;
            AlbumInput.Text = input;
            inputwhensearch = input;
        }
        
        public void setSongInfo(string songPic, string songName, string singername, string publictime, string input)
        {
            Song.Visibility = Visibility.Visible;
            SongPicture.Source = new BitmapImage(new Uri(songPic, UriKind.Absolute));
            SongTitle.Text = songName;
            SongSingerName.Text = singername;
            SongPublicTime.Text = publictime;
            SongInput.Text = input;
            inputwhensearch = input;
        }
        
        public void setSingerInfo(string singerPic, string singerName, string singerInfo, string singerSongs, string singerAlbum,string singerMV,string input)
        {
            Singer.Visibility = Visibility.Visible;
            SingerPicture.ProfilePicture = new BitmapImage(new Uri(singerPic, UriKind.Absolute));
            SingerName.Text  = singerName; 
            SingerInfo.Text  = singerInfo; 
            SingerSongs.Text = singerSongs;
            SingerAlbum.Text = singerAlbum;
            SingerMV.Text    = singerMV;
            SingerInput.Text = input;
            inputwhensearch  = input;
        }

        public void setTVInfo(string TVPic, string TVname, string input)
        {
            TV.Visibility = Visibility.Visible;
            TVPicture.Source = new BitmapImage(new Uri(TVPic, UriKind.Absolute));
            TVName.Text = TVname;
            TVInput.Text = input;
            inputwhensearch = input;
        }

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Shadow.Color = Color.FromArgb(255, 0, 0, 0);
        }

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Shadow.Color = Color.FromArgb(255,156, 156, 156);
        }
    }
}
