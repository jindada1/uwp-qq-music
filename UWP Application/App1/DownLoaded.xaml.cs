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
using Windows.UI.Xaml.Media.Imaging;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DownLoaded : Page
    {
        int GridItemMinWidth = 180;
        public DownLoaded()
        {
            this.InitializeComponent();
            GetLocalFiles();
        }

        private void GridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var columns = Math.Floor(ActualWidth / GridItemMinWidth);
            ((ItemsWrapGrid)LocalFilesContainer.ItemsPanelRoot).ItemWidth = e.NewSize.Width / columns;
        }

        private async void GetLocalFiles()
        {
            try                 // access download folder
            { 
                var DownloadFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(App.MyDownLoadFolder);

                IReadOnlyList<StorageFile> files = await DownloadFolder.GetFilesAsync();

                DisplayFiles(ref files);
            }
            catch (Exception)   // ask user whether to relocate download folder
            {
                EmptyMessage.Visibility = Visibility.Visible;
            }
        }

        async void LetUserPickFolder()
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop
            };
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                EmptyMessage.Visibility = Visibility.Collapsed;

                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);

                IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();

                App.MyDownLoadFolder = "PickedFolderToken";

                DisplayFiles(ref files);
            }
            else
            {
                // operation failed
            }
        }

        private void DisplayFiles(ref IReadOnlyList<StorageFile> files)
        {
            foreach (var song in files)
            {
                StackPanel MusicFile = new StackPanel
                {
                    Width = 180,
                    Spacing = 10
                };

                Image image = new Image
                {
                    Width = 80
                };
                if (song.FileType.Equals(".m4a"))
                {
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/m4a-file-format-symbol.png"));
                }
                else
                {
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/mp3-file-format-variant.png"));
                }
                TextBlock filetext = new TextBlock
                {
                    Text = song.Name,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                MusicFile.Children.Add(image);
                MusicFile.Children.Add(filetext);
                LocalFilesContainer.Items.Add(MusicFile);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LetUserPickFolder();
        }

        private async void Image_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var DownloadFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(App.MyDownLoadFolder);
            await Launcher.LaunchFolderAsync(await StorageFolder.GetFolderFromPathAsync(DownloadFolder.Path));
        }

        private void Image_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Shadow.Opacity = 1.0;
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
        }

        private void Image_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Shadow.Opacity = 0.7;
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }
    }
}
