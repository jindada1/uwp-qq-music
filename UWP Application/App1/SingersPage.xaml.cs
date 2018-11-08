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
    public sealed partial class SingersPage : Page
    {
        List<Singer> singerList = new List<Singer> { };
        int GridItemMinWidth = 160;

        public SingersPage()
        {
            this.InitializeComponent();
            initsingers();
        }

        void initsingers()
        {
            if (App.SingerPageSingers == null)         //first navigated to this page
            {
                App.SingerPageSingers = Creeper.GetSingers();
            }
            singerList = App.SingerPageSingers;
        }

        private void SingersTop15_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var columns = Math.Floor(ActualWidth / GridItemMinWidth);
            ((ItemsWrapGrid)SingersTop15.ItemsPanelRoot).ItemWidth = e.NewSize.Width / columns;
        }

        private void SingersTop15_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = SingersTop15.SelectedIndex;
            MainPage.UpdateStatusBarMessage.ShowStatusMessage(singerList[index].name, 0);
        }
    }
}
