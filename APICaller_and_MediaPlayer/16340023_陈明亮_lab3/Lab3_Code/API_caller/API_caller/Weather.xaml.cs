using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class Weather : Page
    {
        public Weather()
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

        private string GetWeatherMessage(JsonObject json)
        {
            string result = "";
            try
            {
                JsonObject data = json.GetNamedObject("result");
                JsonObject realtime = data.GetNamedObject("realtime");
                JsonObject wind = realtime.GetNamedObject("wind");
                JsonObject weather = realtime.GetNamedObject("weather");
                JsonObject life = data.GetNamedObject("life");
                JsonArray sport = life.GetNamedObject("info").GetNamedArray("yundong");
                JsonArray cloth = life.GetNamedObject("info").GetNamedArray("chuanyi");
                string cityname = realtime.GetNamedString("city_name");
                string wind_direct = wind.GetNamedString("direct");
                string wind_power = wind.GetNamedString("power");
                string weather_info = weather.GetNamedString("info");
                string weather_temp = weather.GetNamedString("temperature");
                string sport_mess = "";
                string cloth_mess = "";
                for (int i = 0; i < sport.Count; i++)
                {
                    int len = sport[i].ToString().Length;
                    if (len >= 16)
                    {
                        sport_mess += sport[i].ToString().Substring(0, 14) + "\n\t";
                        sport_mess += sport[i].ToString().Substring(15, len-15) + "\n";
                     }
                }
                for(int i = 0; i < cloth.Count; i++)
                {
                    if (cloth[i].ToString().Length >= 16)
                    {
                        cloth_mess += cloth[i].ToString().Substring(0, 14) + "\n\t";
                        cloth_mess += cloth[i].ToString().Substring(15, cloth[i].ToString().Length-15) + "\n";
                    }
                }
                result = "城市名称: " + cityname + "\n\n" + "风向：" + wind_direct + "\n风力：" + wind_power +
                        "\n天气情况：" + weather_info + "\n温度：" + weather_temp + "摄氏度" +
                        "\n运动适宜情况: " + sport_mess + "\n穿衣提示：" + cloth_mess;
            }
            catch (Exception e)
            {
                alert_mess("Please enter correct city name!!");
            }
            return result;
        }

        private async void alert_mess(string mess)
        {
            var dialog = new ContentDialog()
            {
                Content = mess,
                PrimaryButtonText = "OK"
            };
            await dialog.ShowAsync();
        }

        private async void onSearchWeather(object sender, RoutedEventArgs e)
        {
            string search_place = key.Text;
            using (HttpClient client = new HttpClient())
            {
                string url = "http://api.avatardata.cn/Weather/Query?key=1274036ad8074ba584c77a8ed7c02084&cityname=" + search_place;
                string result = await client.GetStringAsync(url);
                Debug.WriteLine(JsonValue.Parse(result));
                JsonObject json = JsonValue.Parse(result).GetObject();
                weather.Text = GetWeatherMessage(json);
            }
        }

        private void Enter_Pressed(object sender, KeyRoutedEventArgs args)
        {
            if (args.Key == Windows.System.VirtualKey.Enter)
            {
                RoutedEventArgs e = new RoutedEventArgs();
                onSearchWeather(sender, e);
            }
        }
    }
}
