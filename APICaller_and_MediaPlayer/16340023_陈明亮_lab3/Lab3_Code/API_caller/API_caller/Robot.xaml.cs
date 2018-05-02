using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace API_caller
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Robot : Page
    {
        public Robot()
        {
            this.InitializeComponent();
            key.KeyUp += new KeyEventHandler(Enter_Pressed);
            changeBackImg();
        }

        private void changeBackImg()
        {
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/green-blue.jpg", UriKind.Absolute));
            grid.Background = imageBrush;
        }

        private async void Robot_Click(object sender, RoutedEventArgs e)
        {
            string ques = key.Text;
            HttpClient client = new HttpClient();
            string url = " http://api.avatardata.cn/Tuling/Ask?key=bf143b368ea0400abc73b80de47d3d21&info=" + ques;
            string result = await client.GetStringAsync(url);
            showAnswer(result);
        }

        private void showAnswer(string temp)
        {
            JsonObject data = JsonValue.Parse(temp).GetObject();
            string answer = data.GetNamedObject("result").GetNamedString("text");
            res.Text += answer + "\n";
        }

        private void Enter_Pressed(object sender, KeyRoutedEventArgs args)
        {
            if(args.Key == Windows.System.VirtualKey.Enter)
            {
                RoutedEventArgs e = new RoutedEventArgs();
                Robot_Click(sender, e);
            }
        }
    }
}
