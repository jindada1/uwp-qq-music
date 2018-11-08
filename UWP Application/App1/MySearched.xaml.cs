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
    public sealed partial class MySearched : Page
    {
        public MySearched()
        {
            this.InitializeComponent();
            initsearched();
        }

        public void initsearched()
        {
            for (int i = App.historysearch.Count - 1; i >= 0; i--)
            {
                var search = App.historysearch[i];
                InfoCard newcard;
                switch (search["infotype"])
                {
                    case "1":    //singer
                        newcard = new InfoCard();
                        newcard.setSingerInfo(search["singerPic"],search["singerName"], search["introduce"], search["songNum"], search["albumNum"], search["mvNum"], search["inputkey"]);
                        SearchedList.Items.Add(newcard);
                        break;

                    case "2":    //album
                        newcard = new InfoCard();
                        newcard.setAlbumInfo(search["albumPic"], search["albumName"], search["singerName"], search["publicTime"], search["inputkey"]);
                        SearchedList.Items.Add(newcard);
                        break;

                    case "4":    //tv
                        newcard = new InfoCard();
                        newcard.setTVInfo(search["singerPic"], search["singerName"], search["inputkey"]);
                        SearchedList.Items.Add(newcard);
                        break;

                    case "8":    //song
                        newcard = new InfoCard();
                        newcard.setSongInfo(search["pic"], search["title"], search["desc"], search["publish_date"], search["inputkey"]);
                        SearchedList.Items.Add(newcard);
                        break;

                    default:
                        break;
                }
            }
            if (App.historysearch.Count == 0)
            {
                EmptyLog.Visibility = Visibility.Visible;
            }
        }

        private void SearchedList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainPage.UpdateStatusBarMessage.ShowStatusMessage(((InfoCard)SearchedList.Items[SearchedList.SelectedIndex]).inputwhensearch, 0);
        }
    }
}
