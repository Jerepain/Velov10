using System;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Velov10.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Velov10
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private readonly CoreDispatcher _cd;
        private readonly LocationIcon100M _locationIcon100M;
        private readonly LocationIcon10M _locationIcon10M;
        private readonly Geolocator _geo;

        public MainPage()
        {
            InitializeComponent();
            _locationIcon100M = new LocationIcon100M();
            _locationIcon10M = new LocationIcon10M();
            MyMap.Children.Add(_locationIcon10M);
            MyMap.Children.Add(_locationIcon100M);

            _cd = Window.Current.CoreWindow.Dispatcher;
            MyMap.Center = new Geopoint(new BasicGeoposition
            {
                Latitude = 45.7600,
                Longitude = 4.8400
            });
            // Hub is only supported in Portrait orientation
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            var stations = Helper.LocalisationStations().Result;

                //DoWork.DeserializeStations().ToList();
            foreach (var station in stations)
            {
                var myButton = new Button();

                MapControl.SetLocation(myButton, new Geopoint(new BasicGeoposition
                {
                    Latitude = station.Latitude,
                    Longitude = station.Longitude
                }));

                myButton.Tapped += Helper.DisplayRibbon(station.Number, myButton);
                MyMap.Children.Add(myButton);
            }

            if (_geo == null)
            {
                _geo = new Geolocator();
            }
            if (_geo != null)
            {
                _geo.MovementThreshold = 3.0;
                //     _geo.PositionChanged += new TypedEventHandler<Geolocator, PositionChangedEventArgs>(geo_PositionChanged);
            }
        }

        private async void geo_PositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var pos = e.Position;
                var location = new Geopoint(new BasicGeoposition
                {
                    Latitude = pos.Coordinate.Point.Position.Latitude,
                    Longitude = pos.Coordinate.Point.Position.Longitude
                });
                if (pos.Coordinate.Accuracy <= 10)
                {
                    MapControl.SetLocation(_locationIcon10M, location);
                }
                else if (pos.Coordinate.Accuracy <= 100)
                {
                    MapControl.SetLocation(_locationIcon100M, location);
                }
                MyMap.Center = location;
            });
        }
    }
}