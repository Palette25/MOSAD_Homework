using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Text;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace API_caller
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Frame frame = Window.Current.Content as Frame;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Weather_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(typeof(Weather));
        }

        private void Image_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(typeof(Image));
        }

        private void Translate_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(typeof(Robot));
        }

        private void Sentence_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(typeof(Sentence));
        }
       
    }

    
}
