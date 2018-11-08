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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HomePage : Page
    {
        SexyOption selectedoption;
        List<string> options = new List<string> {"热门歌手","巅峰榜·内地","Billboard","iTunes" };


        public HomePage()
        {
            this.InitializeComponent();
            InitOptions();
        }

        void InitOptions()
        {
            int i = -1;

            foreach (var optionname in options)
            {
                SexyOption option = new SexyOption(optionname)
                {
                    Tag = i.ToString()
                };
                OptionsAreas.Children.Add(option);
                option.PointerPressed += SexyOption_PointerPressed;
                i++;
            }

            selectedoption = (SexyOption)OptionsAreas.Children.First();
            selectedoption.SelectIt();
            SubFrame.Navigate(typeof(SingersPage));
        }

        private void SexyOption_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            SexyOption newselect = (SexyOption)sender;

            if (newselect.Tag != selectedoption.Tag)
            {
                if (Convert.ToInt32(newselect.Tag) > Convert.ToInt32(selectedoption.Tag)) //select right
                {
                    newselect.Gainlove(true);
                    selectedoption.losslove(false);
                }
                else
                {
                    newselect.Gainlove(false);
                    selectedoption.losslove(true);
                }
                selectedoption = newselect;

                if (selectedoption.Tag.ToString() == "-1")
                {
                    SubFrame.Navigate(typeof(SingersPage), null, new SuppressNavigationTransitionInfo());
                }
                else
                {
                    SubFrame.Navigate(typeof(SongsPage), selectedoption.Tag.ToString(), new SuppressNavigationTransitionInfo());
                }
            }
        }
    }
}
