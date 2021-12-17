using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace location_app
{
    public partial class MainPage : ContentPage
    {
        CancellationTokenSource cts;
        Pin pin;
        bool currentIsClicked = false;
        double longitud= 0;
        double latitud = 0;
        bool pinAdded = false;
        public MainPage()
        {
            InitializeComponent();

            getCurrentLocation();

            if (Device.RuntimePlatform == Device.iOS)
            {
                principalScrollview.InputTransparent = false;
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                principalScrollview.InputTransparent = true;
            }
        }
        async Task getCurrentLocation()
        {
            double latLocation = 0;
            double longLocation = 0;
            GeolocationRequest request;
            Location location = null;

            request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            cts = new CancellationTokenSource();
            location = await Geolocation.GetLocationAsync(request, cts.Token);

            latLocation = location.Latitude;
            longLocation = location.Longitude;

            try
            {
                if (location != null)
                {
                    MapView.MoveToRegion(MapSpan.FromCenterAndRadius(
                        new Position(latLocation,
                            longLocation),
                        Distance.FromMiles(0.3)));

                    pin = new Pin
                    {
                        Label = "Nueva Ubicación",
                        Address = calleEntry.Text + " " + coloniaEntry.Text,
                        Type = PinType.Place,
                        Position = new Position(latLocation, longLocation)
                    };

                    pin.MarkerClicked += (s, args) =>
                    {
                        MapView.Pins.Remove(pin);
                        calleEntry.Text = "";
                        postalEntry.Text = "";
                        coloniaEntry.Text = "";
                        municipioEntry.Text = "";
                        pkEstado.SelectedItem = "";
                        paisEntry.Text = "";
                    };

                    if (currentIsClicked)
                    {
                        var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);

                        var placemark = placemarks?.FirstOrDefault();

                        if (placemark != null)
                        {
                            calleEntry.Text = placemark.Thoroughfare + " " + placemark.FeatureName;
                            postalEntry.Text = placemark.PostalCode;
                            coloniaEntry.Text = placemark.SubLocality;
                            municipioEntry.Text = placemark.Locality;
                            pkEstado.SelectedItem = GetEstado(placemark.AdminArea);
                            paisEntry.Text = placemark.CountryName;
                            principalScrollview.ScrollToAsync(0, 0, true);
                        }
                    }

                    if (longitud != 0 && latitud != 0)
                    {
                        MapView.Pins.Add(pin);
                        pinAdded = true;
                    }

                    longitud = longLocation;
                    latitud = latLocation;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Console.WriteLine(fnsEx);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                Console.WriteLine(fneEx);
            }
            catch (PermissionException pEx)
            {
                Console.WriteLine(pEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private string GetEstado(string estadoLbl)
        {
            string estadoCorr = estadoLbl;
            switch (estadoLbl)
            {
                case "AGS":
                    estadoCorr = "Aguascalientes";
                    break;
                case "BC":
                    estadoCorr = "Baja California";
                    break;
                case "BCS":
                    estadoCorr = "Baja California Sur";
                    break;
                case "CAMP":
                    estadoCorr = "Campeche";
                    break;
                case "CHIH":
                    estadoCorr = "Chihuahua";
                    break;
                case "COAH":
                    estadoCorr = "Coahuila de Zaragoza";
                    break;
                case "COL":
                    estadoCorr = "Colima";
                    break;
                case "CHIS":
                    estadoCorr = "Chiapas";
                    break;
                case "CDMX":
                    estadoCorr = "Ciudad de México";
                    break;
                case "DGO":
                    estadoCorr = "Durango";
                    break;
                case "GRO":
                    estadoCorr = "Guerrero";
                    break;
                case "GTO":
                    estadoCorr = "Guanajuato";
                    break;
                case "HGO":
                    estadoCorr = "Hidalgo";
                    break;
                case "JAL":
                    estadoCorr = "Jalisco";
                    break;
                case "Edomex":
                    estadoCorr = "Estado de México";
                    break;
                case "MICH":
                    estadoCorr = "Michoacán";
                    break;
                case "MOR":
                    estadoCorr = "Morelos";
                    break;
                case "NL":
                    estadoCorr = "Nuevo León";
                    break;
                case "NAY":
                    estadoCorr = "Nayarit";
                    break;
                case "OAX":
                    estadoCorr = "Oaxaca";
                    break;
                case "PUE":
                    estadoCorr = "Puebla";
                    break;
                case "QROO":
                    estadoCorr = "Quintana Roo";
                    break;
                case "QRO":
                    estadoCorr = "Querétaro";
                    break;
                case "SIN":
                    estadoCorr = "Sinaloa";
                    break;
                case "S.L.P.":
                    estadoCorr = "San Luis Potosí";
                    break;
                case "SON":
                    estadoCorr = "Sonora";
                    break;
                case "TAB":
                    estadoCorr = "Tabasco";
                    break;
                case "TLAX":
                    estadoCorr = "Tlaxcala";
                    break;
                case "TAMPS":
                    estadoCorr = "Tamaulipas";
                    break;
                case "VER":
                    estadoCorr = "Veracruz";
                    break;
                case "YUC":
                    estadoCorr = "Yucatán";
                    break;
                case "ZAC":
                    estadoCorr = "Zacatecas";
                    break;
            }
            return estadoCorr;
        }
        async void agregaPin(object sender, MapClickedEventArgs e)
        {
            try
            {
                MapView.Pins.Remove(pin);
                var placemarks = await Geocoding.GetPlacemarksAsync(e.Position.Latitude, e.Position.Longitude);

                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    pin = new Pin
                    {
                        Label = "Nueva Ubicación",
                        Address = calleEntry.Text + " " + coloniaEntry.Text,
                        Type = PinType.Place,
                        Position = new Position(e.Position.Latitude, e.Position.Longitude)
                    };
                    MapView.Pins.Add(pin);
                    pinAdded = true;

                    pin.MarkerClicked += (s, args) =>
                    {
                        MapView.Pins.Remove(pin);
                        calleEntry.Text = "";
                        postalEntry.Text = "";
                        coloniaEntry.Text = "";
                        municipioEntry.Text = "";
                        pkEstado.SelectedItem = "";
                        paisEntry.Text = "";
                    };

                    calleEntry.Text = placemark.Thoroughfare + " " + placemark.FeatureName;
                    postalEntry.Text = placemark.PostalCode;
                    coloniaEntry.Text = placemark.SubLocality;
                    municipioEntry.Text = placemark.Locality;
                    pkEstado.SelectedItem = placemark.AdminArea;
                    string deleg = placemark.SubThoroughfare;
                    paisEntry.Text = placemark.CountryName;

                    longitud = e.Position.Longitude;
                    latitud = e.Position.Latitude;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Console.WriteLine(fnsEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private void goCurrent(object sender, EventArgs e)
        {
            currentIsClicked = true;
            getCurrentLocation();
        }
        async void actualizaMapa(object sender, EventArgs e)
        {
            try
            {
                var address = calleEntry.Text + " " + postalEntry.Text + " " + coloniaEntry.Text + " " + municipioEntry.Text + " " + pkEstado.SelectedItem + " " + paisEntry.Text;
                var locations = await Geocoding.GetLocationsAsync(address);

                var location = locations?.FirstOrDefault();
                if (location != null)
                {
                    MapView.Pins.Remove(pin);
                    MapView.MoveToRegion(MapSpan.FromCenterAndRadius(
                        new Position(
                            Convert.ToDouble(location.Latitude),
                            Convert.ToDouble(location.Longitude)),
                        Distance.FromMiles(0.3)));

                    pin = new Pin
                    {
                        Label = "Nueva Ubicación",
                        Address = calleEntry.Text + " " + coloniaEntry.Text,
                        Type = PinType.Place,
                        Position = new Position(location.Latitude, location.Longitude)
                    };
                    MapView.Pins.Add(pin);
                    pinAdded = true;

                    var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);

                    var placemark = placemarks?.FirstOrDefault();
                    string pais = placemark.CountryName;
                    if (placemark != null)
                    {
                        calleEntry.Text = placemark.Thoroughfare + " " + placemark.FeatureName;
                        postalEntry.Text = placemark.PostalCode;
                        coloniaEntry.Text = placemark.SubLocality;
                        municipioEntry.Text = placemark.Locality;
                        pkEstado.SelectedItem = GetEstado(placemark.AdminArea);
                        paisEntry.Text = placemark.CountryName;
                        principalScrollview.ScrollToAsync(0, 0, true);

                        longitud = location.Longitude;
                        latitud = location.Latitude;
                    }
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Console.WriteLine(fnsEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
