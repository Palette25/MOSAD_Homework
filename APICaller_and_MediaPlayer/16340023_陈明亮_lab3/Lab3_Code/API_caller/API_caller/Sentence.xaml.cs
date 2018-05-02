using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
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
    public sealed partial class Sentence : Page
    {
        public Sentence()
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

        private string getMottoMessage(XmlDocument xml)
        {
            string result = "";
            XmlNodeList mottos = xml.GetElementsByTagName("MingRenMingYanObj");
            for(int i = 0; i < mottos.Count; i++)
            {
                XmlNode f_name = mottos.Item(i).FirstChild;
                XmlNode f_motto = mottos.Item(i).LastChild;
                result += f_name.InnerText + " :\n\t";
                string mot = f_motto.InnerText;
                if(mot.Length >= 48&&mot.Length<100)
                {
                    result += mot.Substring(0, 48) + "\n\t";
                    result += mot.Substring(49, mot.Length - 49) + "\n\n";
                }else if (mot.Length >= 100)
                {
                    result += mot.Substring(0, 48) + "\n\t";
                    result += mot.Substring(49, 48) + "\n\t";
                    result += mot.Substring(97, mot.Length - 97) + "\n\n";
                }
                else
                {
                    result += mot + "\n\n";
                }
            }
            if (mottos.Count == 0) alert_mess("No search results, please change keys!");
            return result;
        }

        private async void onSearchMotto(object sender, RoutedEventArgs e)
        {
            string search_key = key.Text;
            using (HttpClient client = new HttpClient())
            {
                string url = "http://api.avatardata.cn/MingRenMingYan/LookUp?key=fe0aff16a3484147a11d38a48281e1df&keyword=" + search_key + "&page=1&rows=4&dtype=xml";
                string result = await client.GetStringAsync(url);
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(result);
                motto.Text = getMottoMessage(xml);
            }
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

        private void Enter_Pressed(object sender, KeyRoutedEventArgs args)
        {
            if (args.Key == Windows.System.VirtualKey.Enter)
            {
                RoutedEventArgs e = new RoutedEventArgs();
                onSearchMotto(sender, e);
            }
        }
    }
}
