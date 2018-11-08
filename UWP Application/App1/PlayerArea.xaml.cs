using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

using Windows.Networking.BackgroundTransfer;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System.Threading;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.AccessCache;
using Windows.Media.Playback;
using Windows.Media.Core;
using Windows.Media;


//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace App1
{
    public sealed partial class PlayerArea : UserControl
    {
        double DefaultHeight;
        string MusicRes = "shit";
        CancellationTokenSource cancellationToken;
        MediaTimelineController _mediaTimelineController;

        bool First = true;
        TimeSpan _duration;
        Song SongOwned;
        bool ISMoving = false;

        public PlayerArea()
        {
            this.InitializeComponent();
            DefaultHeight = PlayArea.Height;
            shrinkplayarea();

            //App.Musicplayer.CommandManager.IsEnabled = false;       //千万要注释掉！！！，不然后台不会播放，调声音也不会有反应
            
            _mediaTimelineController = new MediaTimelineController();
            App.Musicplayer.TimelineController = _mediaTimelineController;
            _mediaTimelineController.PositionChanged += _mediaTimelineController_PositionChanged;

        }

        public void SetInfoByRef(Song song,bool autoplay = false)
        {
            SongOwned = song;
            if (PlayArea.Height < DefaultHeight && First)
            {
                expandplayarea();
                First = false;
            }
            AlbumCoverImage.Source = new BitmapImage(new Uri(song.album_url, UriKind.Absolute));
            BackImage.Source = AlbumCoverImage.Source;
            SongPLayingName.Text = song.name;
            SongPLayingSingerName.Text = song.singers;

            PlayProcess.Maximum = song.timeint;
            PlayProcess.Value = 0;

            _mediaTimelineController.Pause();

            WholeTime.Text = "/" + song.interval;
            NowTime.Text = "0:00";

            PlayAndPause.Icon = new SymbolIcon(Symbol.Play);
            MusicRes = "shit";                                     //symbol
            string Songmid = song.songmid;

            LyricArea.GetLyrics(Songmid);

            var t = new Thread(() => MusicRes = Creeper.GetSongResourseURL(Songmid));
            t.Start();

            // switch song
            if (App.Musicplayer.Source != null)
            {
                App.Musicplayer.Source = null;
                if (App.Musicplayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)   //is playing
                {
                    App.Musicplayer.TimelineControllerPositionOffset = TimeSpan.FromSeconds(0);
                }
                PrepareSourse();
            }
            else if (autoplay)
            {
                PrepareSourse();
            }
        }

        private void Icon_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ((FontIcon)sender).Foreground = new SolidColorBrush(Color.FromArgb(255, 48, 179, 221));
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
        }

        private void Icon_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ((FontIcon)sender).Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }

        private void Shrink_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (PlayArea.Height == DefaultHeight)
            {
                shrinkplayarea();
            }
        }

        private void Expand_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (PlayArea.Height == DefaultHeight)
            {
                ExpandAnimation.Begin();
                BorderExpandAnimation.Begin();
                Expand.Glyph = "\uE70D";
                ToolTipService.SetToolTip(Expand, "缩小");
                Shrink.Visibility = Visibility.Collapsed;
                LyricArea.Visibility = Visibility.Visible;
                BackImage.Visibility = Visibility.Visible;
                Acrylic.Visibility = Visibility.Visible;

                RelativePanel.SetRightOf(SongPLayingName, null);
                RelativePanel.SetAlignLeftWith(SongPLayingSingerName, null);
                RelativePanel.SetBelow(SongPLayingName,AlbumCover);
                RelativePanel.SetAlignHorizontalCenterWith(SongPLayingName, AlbumCover);
                RelativePanel.SetAlignHorizontalCenterWith(SongPLayingSingerName, AlbumCover);
                RelativePanel.SetAlignLeftWith(PlayProcess, AlbumCover);
            }
            else if (PlayArea.Height > DefaultHeight)
            {
                ShrinkBackAnimation.Begin();
                BorderShrinkAnimation.Begin();
                Expand.Glyph = "\uE70E";
                ToolTipService.SetToolTip(Expand, "伸展");
                Shrink.Visibility = Visibility.Visible;
                LyricArea.Visibility = Visibility.Collapsed;
                BackImage.Visibility = Visibility.Collapsed;
                Acrylic.Visibility = Visibility.Collapsed;

                RelativePanel.SetBelow(SongPLayingName, null);
                RelativePanel.SetAlignHorizontalCenterWith(SongPLayingName, null);
                RelativePanel.SetAlignHorizontalCenterWith(SongPLayingSingerName, null);
                RelativePanel.SetBelow(SongPLayingName, null);

                RelativePanel.SetAlignLeftWith(SongPLayingSingerName, SongPLayingName);
                RelativePanel.SetAlignLeftWith(PlayProcess, SongPLayingName);
                RelativePanel.SetRightOf(SongPLayingName, AlbumCover);
            }
            else if (PlayArea.Height < DefaultHeight)
            {
                expandplayarea();
            }
        }
        
        private void expandplayarea()
        {
            ExpandBackAnimation.Begin();
            Shrink.Glyph = "\uE70D";
            ToolTipService.SetToolTip(Shrink, "缩小");
            AlbumCover.Visibility = Visibility.Visible;
            SongPLayingName.Visibility = Visibility.Visible;
            SongPLayingSingerName.Visibility = Visibility.Visible;
            RelativePanel.SetAlignHorizontalCenterWithPanel(CommandButtons, true);
            RelativePanel.SetAlignRightWithPanel(PlayProcess, true);
            RelativePanel.SetAbove(CommandButtons, PlayProcess);
            RelativePanel.SetAlignLeftWith(PlayProcess, SongPLayingSingerName);
            RelativePanel.SetAlignLeftWithPanel(CommandButtons, false);
            RelativePanel.SetAlignBottomWithPanel(CommandButtons, false);
            RelativePanel.SetRightOf(PlayProcess, null);
            RelativePanel.SetLeftOf(PlayProcess, null);
        }

        private void shrinkplayarea()
        {
            ShrinkAnimation.Begin();
            Shrink.Glyph = "\uE738";
            ToolTipService.SetToolTip(Shrink, "陛下，真不能再小了");
            AlbumCover.Visibility = Visibility.Collapsed;
            SongPLayingName.Visibility = Visibility.Collapsed;
            SongPLayingSingerName.Visibility = Visibility.Collapsed;
            RelativePanel.SetAlignHorizontalCenterWithPanel(CommandButtons, false);

            RelativePanel.SetAlignRightWithPanel(PlayProcess, false);
            RelativePanel.SetAbove(CommandButtons, null);
            RelativePanel.SetAlignLeftWith(PlayProcess, null);
            RelativePanel.SetAlignLeftWithPanel(CommandButtons, true);
            RelativePanel.SetAlignBottomWithPanel(CommandButtons, true);
            RelativePanel.SetRightOf(PlayProcess, CommandButtons);
            RelativePanel.SetLeftOf(PlayProcess, Expand);
        }
        
        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (MusicRes != "shit")
            {
                await DownLoadFromAs(MusicRes.Substring(4), SongPLayingName.Text + MusicRes.Substring(0, 4));
            }
        }   // Download

        private async Task DownLoadFromAs(string inputURL, string filename, string albumcover = "")
        {
            StorageFolder MyDownloadFolder;
            // 定位 folder
            try
            {
                MyDownloadFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(App.MyDownLoadFolder);
            }
            catch (Exception)
            {
                MyDownloadFolder = await DownloadsFolder.CreateFolderAsync("Music", CreationCollisionOption.GenerateUniqueName);
                StorageApplicationPermissions.FutureAccessList.AddOrReplace(App.MyDownLoadFolder, MyDownloadFolder);
            }

            // check if exists music file
            try
            {
                StorageFile destinationFile = await MyDownloadFolder.CreateFileAsync(filename, CreationCollisionOption.FailIfExists);

                if (destinationFile != null)
                {
                    if (PlayArea.Height >= DefaultHeight)
                    {
                        TextBlockStatus.Text = "0%";
                        DownloadBar.Value = 0;
                        await DownloadBoard.Fade(value: 1.0f, duration: 500, delay: 0, easingType: EasingType.Default).StartAsync();
                    }

                    Uri source = new Uri(inputURL);

                    BackgroundDownloader downloader = new BackgroundDownloader();
                    DownloadOperation download = downloader.CreateDownload(source, destinationFile);

                    Progress<DownloadOperation> progress = new Progress<DownloadOperation>(x => ProgressChanged(download));
                    cancellationToken = new CancellationTokenSource();

                    await download.StartAsync().AsTask(cancellationToken.Token, progress);

                    if (DownloadBoard.Opacity == 1.0)
                    {
                        await DownloadBoard.Fade(value: 0.0f, duration: 1000, delay: 1000, easingType: EasingType.Default).StartAsync();
                    }
                }
            }
            catch (Exception)
            {
                // music exist                    
                FlyoutBase.ShowAttachedFlyout(DownLoadButton);
            }
        }

        private void ProgressChanged(DownloadOperation downloadOperation)
        {
            int progress = (int)(100 * ((double)downloadOperation.Progress.BytesReceived / (double)downloadOperation.Progress.TotalBytesToReceive));

            switch (downloadOperation.Progress.Status)
            {
                case BackgroundTransferStatus.Running:
                case BackgroundTransferStatus.Completed:
                    {// downloadOperation.Progress.TotalBytesToReceive / 1048576
                        TextBlockStatus.Text = string.Format("{0}%", progress);
                        DownloadBar.Value = progress;
                        break;
                    }
                case BackgroundTransferStatus.PausedByApplication:
                    {
                        TextBlockStatus.Text = "Download paused.";
                        break;
                    }
                case BackgroundTransferStatus.PausedCostedNetwork:
                    {
                        TextBlockStatus.Text = "Download paused because of metered connection.";
                        break;
                    }
                case BackgroundTransferStatus.PausedNoNetwork:
                    {
                        TextBlockStatus.Text = "No network detected. Please check your internet connection.";
                        break;
                    }
                case BackgroundTransferStatus.Error:
                    {
                        TextBlockStatus.Text = "An error occured while downloading.";
                        break;
                    }
                case BackgroundTransferStatus.Canceled:
                    {
                        TextBlockStatus.Text = "Download canceled.";
                        break;
                    }
            }

            if (progress >= 100)
            {
                TextBlockStatus.Text = "下载完成";
                downloadOperation = null;
            }
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (MusicRes != "shit")  //资源有效
            {
                if (App.Musicplayer.Source is null)
                {
                    wantplay(SongOwned);
                }
                else
                {
                    if (App.Musicplayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
                    {
                        _mediaTimelineController.Pause();
                        ((AppBarButton)sender).Icon = new SymbolIcon(Symbol.Play);
                    }
                    else
                    {
                        _mediaTimelineController.Resume();
                        ((AppBarButton)sender).Icon = new SymbolIcon(Symbol.Pause);
                    }
                }
            }
        }

        async void PrepareSourse()
        {
            while (MusicRes == "shit") { }  //not get resourse
            StorageFolder MyDownloadFolder;
            // 定位 folder
            try
            {
                MyDownloadFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(App.MyDownLoadFolder);
            }
            catch (Exception)
            {
                MyDownloadFolder = await DownloadsFolder.CreateFolderAsync("Music", CreationCollisionOption.GenerateUniqueName);
                StorageApplicationPermissions.FutureAccessList.AddOrReplace(App.MyDownLoadFolder, MyDownloadFolder);
            }

            // try to open exist file
            try
            {
                StorageFile music = await MyDownloadFolder.GetFileAsync(SongPLayingName.Text + MusicRes.Substring(0, 4));
                var mediaSource = MediaSource.CreateFromStorageFile(music);
                App.Musicplayer.Source = mediaSource;
                
                mediaSource.OpenOperationCompleted += MediaSource_OpenOperationCompleted;
            }

            // not exist, download it
            catch (Exception)
            {
                StorageFile destinationFile = await MyDownloadFolder.CreateFileAsync(SongPLayingName.Text + MusicRes.Substring(0, 4), CreationCollisionOption.FailIfExists);

                var downloader = new BackgroundDownloader();
                var downloadOperation = downloader.CreateDownload(new Uri(MusicRes.Substring(4)), destinationFile);
                await downloadOperation.StartAsync().AsTask();

                var mediaSource = MediaSource.CreateFromStorageFile(destinationFile);
                App.Musicplayer.Source = mediaSource;

                mediaSource.OpenOperationCompleted += MediaSource_OpenOperationCompleted;
            }
        }

        private async void MediaSource_OpenOperationCompleted(MediaSource sender, MediaSourceOpenOperationCompletedEventArgs args)
        {
            _duration = sender.Duration.GetValueOrDefault();

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                // change ui in page
                _mediaTimelineController.Start();
                PlayAndPause.Icon = new SymbolIcon(Symbol.Pause);
            });
        }

        private async void _mediaTimelineController_PositionChanged(MediaTimelineController sender, object args)
        {
            if (_duration != TimeSpan.Zero && !ISMoving)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    int nowtime = Convert.ToInt32(sender.Position.TotalSeconds);
                    if (nowtime % 60 > 9)
                    {
                        NowTime.Text = (nowtime / 60).ToString() + ":" + (nowtime % 60).ToString();
                    }
                    else
                    {
                        NowTime.Text = (nowtime / 60).ToString() + ":0" + (nowtime % 60).ToString();
                    }
                    PlayProcess.Value = nowtime;
                    LyricArea.Settime(Convert.ToDouble(nowtime));
                    if (nowtime == PlayProcess.Maximum)   //play complete
                    {
                        MusicListSet(Playingid + 1);
                    }
                });
            }
        }

        private void PlayProcess_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            try
            {
                if (Convert.ToInt32(_mediaTimelineController.Position.TotalSeconds) != PlayProcess.Value && !ISMoving)
                {
                    _mediaTimelineController.Position = TimeSpan.FromSeconds(PlayProcess.Value);
                }
            }
            catch (Exception)
            {
                PlayProcess.Value = 0;
            }
        }

        private void PlayProcess_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            ISMoving = true;
        }

        private void PlayProcess_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            try
            {
                _mediaTimelineController.Position = TimeSpan.FromSeconds(PlayProcess.Value);
                ISMoving = false;
            }
            catch (Exception)
            {
                PlayProcess.Value = 0;
            }
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            BlurBrush.Amount = (sender as Slider).Value;
        }


        int Playingid = -1;
        bool UICHANGED = false;

        public void MusicListSet(int id,bool _autoplay = true)
        {
            if (id >= App.MusicList.Count)
            {
                id = 0;
            }
            if (id < 0)
            {
                id = App.MusicList.Count - 1;
            }
            if (!UICHANGED)
            {
                if (Playingid != -1)
                {
                    var lastitem = musiclist.Children[Playingid] as MusicListItem;
                    lastitem.LossFocus();
                }
                var item = musiclist.Children[id] as MusicListItem;
                item.GetFocus();
            }
            Playingid = id;

            SetInfoByRef(App.MusicList[id], _autoplay);
        }

        public void wantplay(Song song)
        {
            // search in list
            for (int i = 0; i < App.MusicList.Count; i++)
            {
                if (App.MusicList[i].songmid == song.songmid)  // already has this song
                {
                    // jump to it
                    MusicListSet(i);
                    return;
                }
            }
            // not find, it's a new song
            // insert song below the song playing
            App.MusicList.Insert(++Playingid, song);
            // and play it
            SetInfoByRef(song,true);
            UICHANGED = true;
        }

        public void updateMusicList(bool playfirst = false)
        {
            if (musiclist.Children.Count > 0)
            {
                musiclist.Children.Clear();
            }
            for (int i = 0; i < App.MusicList.Count; i++)
            {
                var item = new MusicListItem();
                item.SetInfo(i, App.MusicList[i]);
                musiclist.Children.Add(item);
            }
            if (playfirst)
            {
                MusicListSet(0);
            }
        }

        private void ShowMusicListButton_Click(object sender, RoutedEventArgs e)
        {
            if (UICHANGED)
            {
                updateMusicList();
                (musiclist.Children[Playingid] as MusicListItem).GetFocus();
                UICHANGED = false;
            }
        }

        private void LastSongButton_Click(object sender, RoutedEventArgs e)
        {
            if (Playingid > -1)
            {
                MusicListSet(Playingid - 1);
            }
        }

        private void NextSongButton_Click(object sender, RoutedEventArgs e)
        {
            if (Playingid > -1)
            {
                MusicListSet(Playingid + 1);
            }
        }

        public void WantPause(bool Pause)
        {
            if (App.Musicplayer.Source != null)
            {
                if (App.Musicplayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing && Pause)
                {
                    _mediaTimelineController.Pause();
                    PlayAndPause.Icon = new SymbolIcon(Symbol.Play);
                }
                if (App.Musicplayer.PlaybackSession.PlaybackState != MediaPlaybackState.Playing && !Pause)
                {
                    _mediaTimelineController.Resume();
                    PlayAndPause.Icon = new SymbolIcon(Symbol.Pause);
                }
            }
        }
    }
}
