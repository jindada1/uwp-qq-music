using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.System;
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
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            initpage();
        }

        private void initpage()
        {
            autoplay.IsOn = App.AutoPlay;
            autosavesearched.IsOn = App.AutoSaveSearch;
            deletecache.IsOn = App.DeleteCache;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var DownloadFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(App.MyDownLoadFolder);
            await Launcher.LaunchFolderAsync(await StorageFolder.GetFolderFromPathAsync(DownloadFolder.Path));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LetUserPickFolder();
        }

        async void LetUserPickFolder()
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads
            };
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
                App.MyDownLoadFolder = "PickedFolderToken";
            }
            else
            {
                // operation failed
            }
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)    //auto play
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                App.AutoPlay = toggleSwitch.IsOn;
            }
        }

        private void ToggleSwitch_Toggled_1(object sender, RoutedEventArgs e)  //auto save search history
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                App.AutoSaveSearch = toggleSwitch.IsOn;
            }
        }

        private void ToggleSwitch_Toggled_2(object sender, RoutedEventArgs e)  //auto delete cache
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                App.DeleteCache = toggleSwitch.IsOn;
            }
        }
    }
}
