using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace App1
{
    public sealed partial class MusicListItem : UserControl
    {
        UISettings UI;
        bool focused = false;
        int myid;

        public MusicListItem()
        {
            this.InitializeComponent();
            UI = new UISettings();
        }

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!focused)
            {
                PlayIcon.Visibility = Visibility.Visible;
                Container.Background = new SolidColorBrush(Color.FromArgb(200, 200, 200, 200));
            }
        }

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!focused)
            {
                PlayIcon.Visibility = Visibility.Collapsed;
                Container.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            }
        }

        public void GetFocus()
        {
            FocusedBar.Background = new SolidColorBrush(UI.GetColorValue(UIColorType.Accent));
            focused = true;
            PlayIcon.Glyph = "\uE769";
            Foreground = new SolidColorBrush(UI.GetColorValue(UIColorType.Accent));
            PlayIcon.Visibility = Visibility.Visible;
        }

        public void LossFocus()
        {
            FocusedBar.Background = null;
            focused = false;
            PlayIcon.Glyph = "\uE768";
            Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            PlayIcon.Visibility = Visibility.Collapsed;
        }

        public void SetInfo(int id,Song song)
        {
            myid = id;
            SongName.Text = song.name;
            SingerName.Text = song.singers;
            TimeSpan.Text = song.interval;
        }

        public void SetInfo(int id)
        {
            myid = id;
        }

        private void FontIcon_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!focused)
            {
                ((FontIcon)sender).Foreground = new SolidColorBrush(UI.GetColorValue(UIColorType.Accent));
            }
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
        }

        private void FontIcon_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!focused)
            {
                App.OnePlayerArea.MusicListSet(myid);
            }
            else
            {
                if (PlayIcon.Glyph == "\uE768")  //not playing
                {
                    PlayIcon.Glyph = "\uE769";
                }
                else
                {
                    PlayIcon.Glyph = "\uE768";
                }
            }
        }

        private void FontIcon_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!focused)
            {
                ((FontIcon)sender).Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            }
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }
    }
}
