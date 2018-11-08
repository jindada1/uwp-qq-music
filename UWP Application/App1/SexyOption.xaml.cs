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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace App1
{
    public sealed partial class SexyOption : UserControl
    {
        public SexyOption(string _name = "placeholder")
        {
            this.InitializeComponent();
            OptionName.Text = _name;
        }

        public void losslove(bool IsLeft)
        {
            OptionName.Foreground = new SolidColorBrush(Color.FromArgb(255, 176, 176, 176));
            if (IsLeft)
            {
                LossLoveAnimLeft.Begin();
            }
            else
            {
                LossLoveAnimRight.Begin();
            }
        }

        public void Gainlove(bool IsLeft)
        {
            UnderStrong.Visibility = Visibility.Visible;
            OptionName.Foreground = new SolidColorBrush(Color.FromArgb(255,0,0,0));
            if (IsLeft)
            {
                GainLoveAnimLeft.Begin();
            }
            else
            {
                GainLoveAnimRight.Begin();
            }
        }

        public void SelectIt()
        {
            OptionName.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            UnderStrong.Visibility = Visibility.Visible;
        }

        private void RelativePanel_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
        }

        private void RelativePanel_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }
    }
}
