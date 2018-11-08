using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        List<Song> Songlist;
        UISettings UI;
        ScrollViewer scrollViewer;
        double lastposition = 0.0;
        Song selectedsong;
        string HolderSinger = "NullSinger";

        string backgroundimage = "http://111.231.107.125:9000/static/images/Image.jpg";

        public SearchPage()
        {
            this.InitializeComponent();
            Initdata();
            InitUI();
        }

        void InitUI()
        {
            // init background image
            UI = new UISettings();
            try
            {
                BackgroundImage.Source = new BitmapImage(new Uri(backgroundimage, UriKind.Absolute));
            }
            catch (Exception) { }
            // refresh video source
            VideoPlayer.Source = null;
        }

        void Initdata()
        {
            Songlist = App.SearchDataToShow.SearchResult;
            Dictionary<string, string> infoToShow = App.SearchDataToShow.infomation;

            switch (infoToShow["infotype"])
            {
                case "1":    //singer
                    if (!infoToShow.ContainsKey("introduce"))
                    {
                        infoToShow.Add("introduce", Creeper.GetShortIntro(infoToShow["singerMID"]));
                    }
                    HolderSinger = infoToShow["singerName"].ToString();
                    InfoDisplay.setinfo(infoToShow["singerName"],infoToShow["singerPic"], infoToShow["introduce"]);
                    InfoDisplay.setsinger(infoToShow["songNum"], infoToShow["albumNum"], infoToShow["mvNum"]);
                    break;

                case "2":    //album
                    InfoDisplay.setinfo(infoToShow["albumName"], infoToShow["albumPic"],"发行时间：" + infoToShow["publicTime"]);
                    InfoDisplay.setfooter(infoToShow["singerName"]);
                    break;

                case "4":    //tv
                    InfoDisplay.setinfo(infoToShow["singerName"], infoToShow["singerPic"]);
                    break;

                case "8":    //song
                    InfoDisplay.setinfo(infoToShow["title"],infoToShow["pic"], "发行时间：" + infoToShow["publish_date"]);
                    InfoDisplay.setfooter(infoToShow["desc"]);
                    break;

                default:
                    InfoDisplay.Visibility = Visibility.Collapsed;
                    break;
            }
            if (InfoDisplay.Visibility == Visibility.Visible)
            {
                Thread th = new Thread(StoreHistoryData);
                th.Start();
            }
        }

        private void StoreHistoryData()
        {
            string inputkey = App.SearchDataToShow.infomation["inputkey"];
            foreach (var search in App.historysearch)
            {
                if (search.ContainsValue(inputkey))  //exist
                {
                    return;
                }
            }
            App.historysearch.Add(App.SearchDataToShow.infomation);
        }

        private void SongList_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Border border = VisualTreeHelper.GetChild(SongList, 0) as Border;
            scrollViewer = VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
            scrollViewer.ViewChanging += Changing;
        }

        private void Changing(object sender, ScrollViewerViewChangingEventArgs e)
        {
            if (Songlist.Count > 15)     //hide top head area
            {
                if (scrollViewer.VerticalOffset - lastposition > 0) //Down scrolling
                {
                    if (scrollViewer.VerticalOffset - lastposition > 30)
                    {
                        UpBoard.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    if (scrollViewer.VerticalOffset < 10)
                    {
                        UpBoard.Visibility = Visibility.Visible;
                    }
                }
                lastposition = scrollViewer.VerticalOffset;
            }
        }

        private void SongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (App.Musicplayer.Source == null)
            {
                App.OnePlayerArea.SetInfoByRef(SongList.SelectedItem as Song);
            }
        }

        private void SongList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            App.OnePlayerArea.wantplay(SongList.SelectedItem as Song);
        }

        private void FontIcon_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ((FontIcon)sender).Foreground = new SolidColorBrush(UI.GetColorValue(UIColorType.Accent));
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
        }

        private void FontIcon_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            myflyout.Hide();
            App.OnePlayerArea.wantplay(selectedsong);
        }

        private void FontIcon_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ((FontIcon)sender).Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }

        private void SongList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            selectedsong = (e.OriginalSource as FrameworkElement)?.DataContext as Song;
            flyoutText.Text = selectedsong.name;
            flyoutImage.Source = new BitmapImage(new Uri(selectedsong.album_url));
            myflyout.ShowAt(e.OriginalSource as FrameworkElement);
        }

        private void PlayAll_Click(object sender, RoutedEventArgs e)
        {
            App.MusicList = Songlist;
            App.OnePlayerArea.updateMusicList(true);
        }

        private async void PlayVideo_Click(object sender, RoutedEventArgs e)
        {
            VideoArea.Visibility = Visibility.Visible;
            await VideoArea.Fade(value: 1.0f, duration: 500, delay: 0, easingType: EasingType.Default).StartAsync();

            if (VideoPlayer.Source == null)
            {
                // find first song mv
                string url = Creeper.GetFromiTunes(Songlist[0].name);
                if (url != "novideo")
                {
                    VideoPlayer.Source = MediaSource.CreateFromUri(new Uri(url));
                    App.OnePlayerArea.WantPause(true);
                }
                // else search singer
                else if (HolderSinger != "NullSinger")
                {
                    url = Creeper.GetFromiTunes(HolderSinger);
                    if (url != "novideo")
                    {
                        VideoPlayer.Source = MediaSource.CreateFromUri(new Uri(url));
                        App.OnePlayerArea.WantPause(true);
                    }
                }
                // no video
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            VideoArea.Visibility = Visibility.Collapsed;
            VideoArea.Opacity = 0.0;
            VideoPlayer.MediaPlayer.Pause();
            App.OnePlayerArea.WantPause(false);
        }
    }
}
