using Microsoft.Band;
using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace UV
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel _vm;

        private const string NO_BANDS_TEXT = "This app requires a Microsoft Band paired to your phone. \nAlso make sure that you have the latest firmware installed on your Band, as provided by the latest Microsoft Health app.";
        private bool bandFound = false;

        public IBandInfo[] PairedBands { get; private set; }
        public IBandClient Band { get; private set; }



        public MainPage()
        {
            _vm = new MainViewModel(Dispatcher);
            DataContext = _vm;

            this.InitializeComponent();
            loadingRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
            loadingRing.IsActive = true;
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        private async System.Threading.Tasks.Task GetBandsAsync()
        {

            // Get the list of Microsoft Bands paired to the phone.
            PairedBands = await BandClientManager.Instance.GetBandsAsync();
            if (PairedBands.Length < 1)
            {
                MessageDialog md = new MessageDialog(NO_BANDS_TEXT, "No Microsoft Band found");
                md.ShowAsync();
                bandFound = false;
            }
            else bandFound = true;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GetUvIndex();
        }

        private async Task GetUvIndex()
        {
            await GetBandsAsync();
            // Connect to Microsoft Band.
            using (Band = await BandClientManager.Instance.ConnectAsync(PairedBands[0]))
            {

                Band.SensorManager.Ultraviolet.ReportingInterval = Band.SensorManager.Ultraviolet.SupportedReportingIntervals.ToList()[1];

                // Subscribe to Accelerometer data.
                Band.SensorManager.Ultraviolet.ReadingChanged += Ultraviolet_ReadingChanged;
                await Band.SensorManager.Ultraviolet.StartReadingsAsync();

                // Receive Accelerometer data for a while.
                await Task.Delay(TimeSpan.FromSeconds(1));
                await Band.SensorManager.Ultraviolet.StopReadingsAsync();

                loadingRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                loadingRing.IsActive = false;

            }
        }

        async void Ultraviolet_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandUltravioletLightReading> e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                switch (e.SensorReading.ExposureLevel)
                {
                    case UltravioletExposureLevel.None:
                        //_vm.UvLevel = "None";
                        //break;
                    case UltravioletExposureLevel.Low:
                        _vm.UvLevel = "Low";
                        _vm.MinIndex = 1;
                        _vm.MaxIndex = 2;
                        break;
                    case UltravioletExposureLevel.Medium:
                        _vm.UvLevel = "Medium";
                        _vm.MinIndex = 3;
                        _vm.MaxIndex = 5;
                        break;
                    case UltravioletExposureLevel.High:
                        _vm.UvLevel = "Medium";
                        _vm.MinIndex = 6;
                        _vm.MaxIndex = 7;
                        break;
                    case UltravioletExposureLevel.VeryHigh:
                        _vm.UvLevel = "VeryHigh";
                        _vm.MinIndex = 8;
                        _vm.MaxIndex = 10;
                        break;
                }
                _vm.CalculateModifiedUvIndex();
                _vm.Description = _vm.GetDescription();
            });
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (StringKeyValue item in e.AddedItems)
            {
                if (!_vm.SelectedLocationType.Contains(item))
                    _vm.SelectedLocationType.Add(item);
            }

            foreach (StringKeyValue item in e.RemovedItems)
            {
                if (_vm.SelectedLocationType.Contains(item))
                    _vm.SelectedLocationType.Remove(item);
            }

            _vm.CalculateModifiedUvIndex();
            _vm.Description = _vm.GetDescription();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(spfText.Text))
                return;
            int val = 1;
            if (Int32.TryParse(spfText.Text, out val))
                _vm.SunscreenValue = val;
            else
                _vm.SunscreenValue = 1;

            _vm.CalculateModifiedUvIndex();
            _vm.Description = _vm.GetDescription();
        }

        private void ListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            _vm.CalculateModifiedUvIndex();
            _vm.Description = _vm.GetDescription();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }
    }
}
