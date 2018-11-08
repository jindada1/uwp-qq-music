using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace App1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        NavigationViewItem selecteditem;
        bool GetPermission = false;

        public MainPage()
        {
            this.InitializeComponent();
            UpdateStatusBarMessage.OnNewStatusMessage += UpdateStatusBarMessage_OnNewStatusMessage;
            loadconfigdata();
            App.OnePlayerArea = playarea;
            Window.Current.SetTitleBar(RealTitleBar);

            // get jiading weather
            Invoke(() =>
            {
                try
                {
                    WeatherText.Text = Creeper.GetWeather();
                }
                catch (Exception) { }
                try
                {
                    GetPermission = Creeper.Get("http://111.231.107.125:9000/permission") == "ok";
                }
                catch (Exception) { }
            });
        }

        public async void Invoke(Action action, Windows.UI.Core.CoreDispatcherPriority Priority = Windows.UI.Core.CoreDispatcherPriority.Normal)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); });
        }

        void UpdateStatusBarMessage_OnNewStatusMessage(string strMessage, int msgtype)
        {
            if (msgtype == 0)   //click cover
            {
                StartSearch(strMessage);
            }
            if (msgtype == 1)
            {
                //
            }
        }

        /// <summary>
        /// init historic search from file
        /// </summary>

        async void loadconfigdata()
        {
            await DataHandler.LoadFromJSONAsync();
        }

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            string searchkey = SearchBox.Text;
            if (!string.IsNullOrWhiteSpace(searchkey))
            {
                StartSearch(searchkey);
            }
        }

        private async void StartSearch(string searchkey)
        {
             pr_ProgressRing1.IsActive = true;

             App.SearchDataToShow = Creeper.GetSearchResult(searchkey);
             await Task.Delay(1);

             NavigationViewItem newitem = new NavigationViewItem();
             LeftPole.MenuItems.Add(newitem);
             newitem.Tag = "App1.SearchPage";
             LeftPole.SelectedItem = newitem;

             LeftPole.MenuItems.Remove(newitem);
             pr_ProgressRing1.IsActive = false;
        }

        private async void LeftPole_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (GetPermission)
            {
                if (args.IsSettingsSelected)
                {
                    // not implemented
                    ContentFrame.Navigate(typeof(SettingsPage));
                }
                else
                {
                    selecteditem = (NavigationViewItem)args.SelectedItem;
                    string typeName = selecteditem.Tag.ToString();

                    ContentFrame.Navigate(Type.GetType(typeName));
                }
            }
            else
            {
                NoPermission.Visibility = Visibility.Visible;
                await NoPermission.Fade(value: 1.0f, duration: 500, delay: 0, easingType: EasingType.Default).StartAsync();
            }
        }

        public delegate void AddStatusMessageDelegate(string strMessage, int msgtype);

        public static class UpdateStatusBarMessage
        {

            public static Page mainwin;

            public static event AddStatusMessageDelegate OnNewStatusMessage;

            public static void ShowStatusMessage(string strMessage, int msgtype)
            {
                ThreadSafeStatusMessage(strMessage, msgtype);
            }

            private static void ThreadSafeStatusMessage(string strMessage, int msgtype)
            {
                OnNewStatusMessage(strMessage, msgtype);
            }
        }

        private void LeftPole_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
            }
        }
    }
}
