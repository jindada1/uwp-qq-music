using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SongsPage : Page
    {
        int GridItemMinWidth = 360;
        List<int> topids = new List<int> { 5, 108, 123 };
        List<string> types = new List<string> { "top", "global", "global" };
        int songlistindex;
        List<Song> HotSongs = new List<Song> { };

        public SongsPage()
        {
            this.InitializeComponent();
        }

        public async void Invoke(Action action, Windows.UI.Core.CoreDispatcherPriority Priority = Windows.UI.Core.CoreDispatcherPriority.Normal)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); });
        }

        private void InitHotSongs(string id)
        {
            int index = Convert.ToInt32(id);
            songlistindex = index;

            Invoke(() =>
            {
                if (App.SongPageHotSongLists[index] == null)
                {
                    //update UI code
                    App.SongPageHotSongLists[index] = Creeper.GetTopSongs(types[index], topids[index]);
                    HotSongsArea.ItemsSource = App.SongPageHotSongLists[index];
                }
                else
                {
                    HotSongsArea.ItemsSource = App.SongPageHotSongLists[index];
                }
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string && !string.IsNullOrWhiteSpace((string)e.Parameter))
            {
                InitHotSongs(e.Parameter.ToString());
            }
            base.OnNavigatedTo(e);
        }

        private void HotSongsArea_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

        }

        private void HotSongsArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.OnePlayerArea.wantplay(App.SongPageHotSongLists[songlistindex][HotSongsArea.SelectedIndex]);
        }

        private void HotSongsArea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var columns = Math.Floor(ActualWidth / GridItemMinWidth);
            ((ItemsWrapGrid)HotSongsArea.ItemsPanelRoot).ItemWidth = e.NewSize.Width / columns;
        }

        private void Shadow_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            //
        }

        private void Shadow_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            //
        }
    }
}
