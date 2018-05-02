using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MediaPlayer mplayer = new MediaPlayer();
        private MediaTimelineController control = new MediaTimelineController();
        DispatcherTimer timer = new DispatcherTimer();

        public MainPage()
        {
            this.InitializeComponent();
            
            changeBackImg();
            mplayer.Volume = 0.6;
            sound.Value = mplayer.Volume * 100;
            setPlayerSource();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += slider_change;
        }

        private void changeBackImg()
        {
            ImageBrush img = new ImageBrush();
            img.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/green-blue.jpg", UriKind.Absolute));
            grid.Background = img;
        }

        private void setPlayerSource()
        {
            var source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/test1.mp3", UriKind.Absolute));
            mplayer.Source = source;
            mplayer.CommandManager.IsEnabled = false;
            mplayer.TimelineController = control;
        }

        private void on_play(object sender, RoutedEventArgs e)
        {
            AppBarButton btn = sender as AppBarButton;
            if(btn.Name == "play")
            {
                on_begin(btn);
               
            }else if(btn.Name == "pause")
            {
                on_pause(btn);
            }
        }

        private void on_begin(AppBarButton btn)
        {
            timer.Start();
            //Resume the time line
            if((int)control.Position.TotalSeconds == 0)
            {
                control.Start();
            }
            else control.Resume();
            //Set play button and begin audio
            btn.Name = "pause";
            board.Begin();
            btn.Icon = new SymbolIcon(Symbol.Pause);
            slider.Maximum = mplayer.PlaybackSession.NaturalDuration.TotalSeconds;
            Debug.WriteLine(control.State);
        }

        private void on_pause(AppBarButton btn)
        {
            Debug.WriteLine(control.State);
            //Pause the time line
            control.Pause();

            //Set play button and pause audio
            board.Pause();
            btn.Name = "play";
            btn.Icon = new SymbolIcon(Symbol.Play);
        }

        private void onSlideChanged(object sender, RoutedEventArgs e)
        {
            control.Position = TimeSpan.FromSeconds(slider.Value);
            startTime.Text = control.Position.ToString(@"hh\:mm\:ss");
            endTime.Text = ((TimeSpan)mplayer.PlaybackSession.NaturalDuration - control.Position).ToString(@"hh\:mm\:ss");
        }

        private void slider_change(object sender, object e)
        {
            slider.Value = ((TimeSpan)control.Position).TotalSeconds;
            Debug.WriteLine("slid: " + slider.Value + "max: " + slider.Maximum + "state: " + control.State);
            if (slider.Value == slider.Maximum)
            {
                on_reset();
            }
        }

        private void on_reset()
        {
            timer.Stop();
            slider.Value = 0;
            slider.Maximum = mplayer.PlaybackSession.NaturalDuration.TotalSeconds;
            control.Position = TimeSpan.FromSeconds(0);
            control.Pause();
            board.Stop();
            play.Icon = new SymbolIcon(Symbol.Play);
            play.Name = "play";
        }

        private void on_stop(object sender, RoutedEventArgs e)
        {
            on_reset();
            startTime.Text = "00:00:00";
            endTime.Text = "00:00:00";
        }

        private void on_fullScreen(object sender, RoutedEventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            
            AppBarButton btn = sender as AppBarButton;
            if(btn.Name == "large")
            {
                view.TryEnterFullScreenMode();
                if (videoPlayer.Visibility == Visibility.Visible) {
                    videoPlayer.IsFullWindow = true;
                }
                btn.Name = "small";
                btn.Icon = new SymbolIcon(Symbol.BackToWindow);
            }else
            {
                view.ExitFullScreenMode();
                videoPlayer.IsFullWindow = false;
                btn.Name = "large";
                btn.Icon = new SymbolIcon(Symbol.FullScreen);
            }
        }

        private void on_watchVideo(object sender, RoutedEventArgs e)
        {
            slider.Maximum = mplayer.PlaybackSession.NaturalDuration.TotalSeconds;
            AppBarButton btn = sender as AppBarButton;
            if(btn.Name == "video")
            {
                to_video(btn);
            }else
            {
                to_audio(btn);
            }
            timer.Stop();
            slider.Value = 0;
            control.Position = TimeSpan.FromSeconds(0);
            control.Pause();
            board.Stop();
            play.Icon = new SymbolIcon(Symbol.Play);
            play.Name = "play";
            startTime.Text = "00:00:00";
            endTime.Text = "00:00:00";
        }

        private void to_video(AppBarButton btn)
        {
            videoPlayer.Visibility = Visibility.Visible;
            var source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/test0.mp4", UriKind.Absolute));
            mplayer.Source = source;
            videoPlayer.SetMediaPlayer(mplayer);
            btn.Icon = new SymbolIcon(Symbol.Audio);
            btn.Name = "audio";
        }

        private void to_audio(AppBarButton btn)
        {
            videoPlayer.Visibility = Visibility.Collapsed;
            var source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/test1.mp3", UriKind.Absolute));
            mplayer.Source = source;
            btn.Icon = new SymbolIcon(Symbol.Video);
            btn.Name = "video";
        }

        private async void on_upload(object sender, RoutedEventArgs e)
        {
            on_reset();
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".mp3");
            picker.FileTypeFilter.Add(".wma");
            picker.FileTypeFilter.Add(".wmv");
            picker.FileTypeFilter.Add(".mp4");
            StorageFile result = await picker.PickSingleFileAsync();
            upload_event(result);
            startTime.Text = "00:00:00";
            endTime.Text = "00:00:00";
        }

        private async void upload_event(StorageFile result)
        {
            if (result != null)
            {
                using (IRandomAccessStream stream = await result.OpenAsync(FileAccessMode.Read))
                {
                    mplayer.Source = MediaSource.CreateFromStream(stream, result.FileType);
                    var thumb = await result.GetScaledImageAsThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.MusicView, 700);
                    BitmapDecoder de = await BitmapDecoder.CreateAsync(thumb);
                    WriteableBitmap wbit = new WriteableBitmap((int)de.PixelWidth, (int)de.PixelHeight);
                    wbit.SetSource(thumb);
                    AudioImg.ImageSource = wbit;
                    if (result.FileType == ".mp4")
                    {
                        videoPlayer.SetMediaPlayer(mplayer);
                        videoPlayer.Visibility = Visibility.Visible;
                        video.Icon = new SymbolIcon(Symbol.Audio);
                        video.Name = "audio";
                    }
                    else
                    {
                        videoPlayer.Visibility = Visibility.Collapsed;
                        video.Icon = new SymbolIcon(Symbol.Video);
                        video.Name = "video";
                    }
                }
            }
        }

        private void on_changeVolume(object sender, RoutedEventArgs e)
        {
            if(vpanel.Visibility == Visibility.Collapsed)
            {
                vpanel.Visibility = Visibility.Visible;
            }
            else
            {
                vpanel.Visibility = Visibility.Collapsed;
            }
            
        }

        private void volume_changed(object sender, RoutedEventArgs e)
        {
            mplayer.Volume = (double)sound.Value/100;
        }

        private async void BackImage_Drag(object sender, DragEventArgs e)
        {
            on_reset();
            e.AcceptedOperation = DataPackageOperation.Copy;
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                items = items.OfType<StorageFile>()
                        .Where(s => s.FileType.Equals(".mp3") || s.FileType.Equals(".mp4") || s.FileType.Equals(".wma") || s.FileType.Equals(".wmv")
                        ).ToList() as IReadOnlyList<IStorageItem>;
                foreach(var item in items)
                {
                    StorageFile file = item as StorageFile;
                    upload_event(file);
                    startTime.Text = "00:00:00";
                    endTime.Text = "00:00:00";
                }
            }

        }

        private void BackImage_Drop(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }
    }
}
