using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ExampleApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var api = new Api();

            // feeds found at http://gearside.com/public-json-feeds/
            // http://jsonip.com/
            // http://www.telize.com/geoip?callback=
            // https://data.itpir.wm.edu/deflate/api.php?val=100USD1986USA&json=true
            // http://coinabul.com/api.php
            // http://www.nactem.ac.uk/software/acromine/dictionary.py?sf=BMI
            // https://qrng.anu.edu.au/API/jsonI.php?length=1&type=uint8

            var resp = await api.HttpGet("http://jsonip.com/");
            Debug.WriteLine(resp);

            resp = await api.HttpGet("https://data.itpir.wm.edu/deflate/api.php?val=100USD1986USA&json=true");
            Debug.WriteLine(resp);

            resp = await api.HttpGet("http://coinabul.com/api.php");
            Debug.WriteLine(resp);
        }
    }
}
