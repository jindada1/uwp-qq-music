using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace App1
{
    public sealed partial class MagicImage : UserControl
    {
        public MagicImage()
        {
            this.InitializeComponent();
        }


        private void Image_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint ptrPt = e.GetCurrentPoint(Profile);
            AngleRotate.RotationX = (Convert.ToInt32(ptrPt.Position.Y) - 60)/6;
            AngleRotate.RotationY = (60 - Convert.ToInt32(ptrPt.Position.X))/6;
        }

        private void Profile_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            AngleRotate.RotationX = AngleRotate.RotationY = 0;
        }

        public void setinfo(string _singername, string profile_url, string _singerinfomation = "")
        {
            title.Text = _singername;
            ProfileImage.Source = new BitmapImage( new Uri(profile_url, UriKind.Absolute));
            information.Text = _singerinfomation;
        }

        public void setsinger(string _songnumber, string _albumnumber, string _mvnumber)
        {
            SongAlbumMV.Visibility = Visibility.Visible;
            songnumber.Text = _songnumber;
            albumnumber.Text = _albumnumber;
            mvnumber.Text = _mvnumber;
        }

        public void setfooter(string _footinfo)
        {
            foottext.Visibility = Visibility.Visible;
            foottext.Text = _footinfo;
        }
    }
}
