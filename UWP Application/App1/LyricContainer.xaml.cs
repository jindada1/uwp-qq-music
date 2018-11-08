using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace App1
{
    public sealed partial class LyricContainer : UserControl
    {
        int mylove;
        double SingleItemHeight;
        TextBlock HeadText;
        TextBlock FootText;
        int lyricsIndex = 0;

        List<double> keyframes = new List<double>();


        public LyricContainer()
        {
            this.InitializeComponent();
        }

        public void Settime(double timespan)
        {
            if (keyframes.Count > lyricsIndex)
            {
                while (timespan + 0.1 > keyframes[lyricsIndex])
                {
                    lyricsIndex++;
                }

                while (timespan + 0.1 < keyframes[lyricsIndex] && lyricsIndex > 0)
                {
                    lyricsIndex--;
                }

                ScrollTo(lyricsIndex+1);
            }
        }

        public void GetLyrics(string songmid)
        {
            lyricsIndex = 0;
            mylove = 0;
            if (keyframes.Count > 0)// 切歌了，清空容器
            {
                keyframes.Clear();
                Lyrics.Children.Clear();
                ScrollContainer.ChangeView(1, 0, 1);
            }
            Lyrics.Children.Add(HeadText = new TextBlock());
            HeadText.Height = ScrollContainer.ActualHeight / 2;

            Invoke(() =>
            {
                List<string> Result = Creeper.GetLyrics(songmid);   //Result[0]是原版，Result[1]是翻译版

                var lyrics = Result[0].Split(Environment.NewLine.ToCharArray());

                string Title, Artist, Album, LrcBy, Offset;

                foreach (var line in lyrics)
                {
                    if (line.StartsWith("[ti:"))
                    {
                        Title = line.Substring(line.IndexOf(":") + 1).TrimEnd(']');
                    }
                    else if (line.StartsWith("[ar:"))
                    {
                        Artist = line.Substring(line.IndexOf(":") + 1).TrimEnd(']');
                    }
                    else if (line.StartsWith("[al:"))
                    {
                        Album = line.Substring(line.IndexOf(":") + 1).TrimEnd(']');
                    }
                    else if (line.StartsWith("[by:"))
                    {
                        LrcBy = line.Substring(line.IndexOf(":") + 1).TrimEnd(']');
                    }
                    else if (line.StartsWith("[offset:"))
                    {
                        Offset = line.Substring(line.IndexOf(":") + 1).TrimEnd(']');
                    }
                    else
                    {
                        try
                        {
                            Regex regexword = new Regex(@".*\](.*)");
                            Match mcw = regexword.Match(line);
                            string word = mcw.Groups[1].Value;
                            if (word.Length != 0)
                            {
                                Regex regextime = new Regex(@"\[([0-9.:]*)\]", RegexOptions.Compiled);
                                MatchCollection mct = regextime.Matches(line);
                                foreach (Match item in mct)
                                {                               // day
                                    double time = TimeSpan.Parse("00:" + item.Groups[1].Value).TotalSeconds;   // item.Groups[1].Value 00:40.44
                                    keyframes.Add(time);
                                    // 更新 UI，填入歌词
                                    Lyrics.Children.Add(new TextBlock { Text = word, HorizontalAlignment = HorizontalAlignment.Center });
                                }
                            }
                        }
                        catch { continue; }
                    }
                }
                keyframes.Add(99999.9);
                Lyrics.Children.Add(FootText = new TextBlock());
            });
        }

        private void ScrollTo(int mynewlove)
        {
            ((TextBlock)Lyrics.Children[mylove]).Foreground = new SolidColorBrush(Color.FromArgb(255, 60, 60, 60));
            mylove = mynewlove;
            ScrollContainer.ChangeView(1, SingleItemHeight * mylove, 1);
            ((TextBlock)Lyrics.Children[mylove]).Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        }

        private void ScrollContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Lyrics.Children.Count > 1)
            {
                SingleItemHeight = ((TextBlock)Lyrics.Children[1]).ActualHeight + Lyrics.Spacing;
                HeadText.Height = ScrollContainer.ActualHeight / 2;
                FootText.Height = ScrollContainer.ActualHeight / 2;
            }
        }

        private async void Invoke(Action action, Windows.UI.Core.CoreDispatcherPriority Priority = Windows.UI.Core.CoreDispatcherPriority.Normal)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); });
        }
    }
}
