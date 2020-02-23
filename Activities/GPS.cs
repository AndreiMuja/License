using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using Android.Locations;
using Android.Gms.Maps.Model;
using Android.Icu.Text;
using Java.Util;
using Firebase.Xamarin.Database;
using PrevenireRiscIT.Classes;
using System.Threading.Tasks;


namespace PrevenireRiscIT.Activities
{
    [Activity(Label = "GPS")]
    public partial class GPS : Activity,IOnMapReadyCallback,ILocationListener
    {
        private Toolbar gpsTool;
        private EditText latEdit;
        private EditText longEdit;
        private EditText dataGPS;
        private TextView tvAddress;
        private ProgressBar gpsProgress;
        private GoogleMap _harta;
        private LocationManager _locatie;
        private Location _curent;
        private LatLng _coordonate;
        private SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy HH:mm:ss");
        private Date d = new Date();
        private ListView _listaExportata;
        private List<CoordonateGPS> _listaAdrese = new List<CoordonateGPS>();
        private Export _export;
        private CoordonateGPS _coordGPS;
        private string _sursaLocatie;
        private const string _linkBDFirebase = "https://prevenireriscit.firebaseio.com/";

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;

            Toast.MakeText(this, "Use this activity to determine your last location and print the coordinates in the textfields " +
                " along with the possibility to save and export them from cloud in a listview and make a quick phone call in case of " +
                " emergency!",ToastLength.Long).Show();
            SetContentView(Resource.Layout.GPS);
            gpsTool = FindViewById<Toolbar>(Resource.Id.gpsToolbar);
            SetActionBar(gpsTool);
            latEdit = FindViewById<EditText>(Resource.Id.seeLatitude);
            longEdit = FindViewById<EditText>(Resource.Id.seeLongitude);
            dataGPS = FindViewById<EditText>(Resource.Id.dateGPS);
            tvAddress = FindViewById<TextView>(Resource.Id.tvAddress);
            gpsProgress = FindViewById<ProgressBar>(Resource.Id.gpsProgressBar);
            _listaExportata = FindViewById<ListView>(Resource.Id.cardExport);

            _listaExportata.ItemClick += (s, e) =>
            {
                CoordonateGPS _coo = _listaAdrese[e.Position];
                _coordGPS = _coo;
                latEdit.Text = _coo.Latitudine;
                longEdit.Text = _coo.Longitudine;
                dataGPS.Text = _coo.DataCurentaLocatie;
                tvAddress.Text = _coo.AdresaLocatie;
            };
            gpsTool.MenuItemClick += _optiuniGPS;
            MapFragment _mapa = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.gpsFragment);
            _mapa.GetMapAsync(this);
            _locatie = (LocationManager)GetSystemService(LocationService);
            Criteria _criteriu = new Criteria { Accuracy = Accuracy.Fine };
            _sursaLocatie = _locatie.GetBestProvider(_criteriu, true);

            await _creareBazaDateFirebase();
        }

        public void OnMapReady(GoogleMap hartaGoogle)
        {
            _harta = hartaGoogle;
            _harta.MapType = GoogleMap.MapTypeSatellite;
            _harta.UiSettings.ZoomControlsEnabled = true;
            _harta.UiSettings.CompassEnabled = true;
            _harta.MyLocationEnabled = true;
            _curent = _locatie.GetLastKnownLocation(_sursaLocatie);
            double _latitudine = _curent.Latitude;
            double _longitudine = _curent.Longitude;
            _coordonate = new LatLng(_latitudine, _longitudine);
            _harta.AddMarker(new MarkerOptions().SetPosition(new LatLng(_latitudine, _longitudine))
                .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.Pointer)));
            _harta.MarkerClick += _pozitioneazaCamera;
        }

        private void _pozitioneazaCamera(object ob, GoogleMap.MarkerClickEventArgs ev)
        {
            ev.Handled = true;
            _harta.MoveCamera(CameraUpdateFactory.NewLatLng(_coordonate));
            _harta.AnimateCamera(CameraUpdateFactory.ZoomTo(17));
            Geocoder _geolocatie = new Geocoder(this);
            IList<Address> _listaAdrese = _geolocatie.GetFromLocation(_curent.Latitude, _curent.Longitude, 4);
            Address _adresa = _listaAdrese.FirstOrDefault();
            latEdit.Text = _adresa.Latitude.ToString();
            longEdit.Text = _adresa.Longitude.ToString();
            StringBuilder _stradaTaraCodPostal = new StringBuilder();
            for (int i = 0; i < _adresa.MaxAddressLineIndex; i++)
                _stradaTaraCodPostal.AppendLine(_adresa.GetAddressLine(i));
            tvAddress.Text = _stradaTaraCodPostal.ToString();
            dataGPS.Text = sdf.Format(d);
        }

        protected override void OnResume()
        {
            base.OnResume();
            _locatie.RequestLocationUpdates(_sursaLocatie, 500, 10, this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            _locatie.RemoveUpdates(this);
        }

        public void OnLocationChanged(Location locatia)
        {
        }

        public void OnProviderDisabled(string provider)
        {
            Toast.MakeText(this, "GPS Disabled!", ToastLength.Short).Show();
        }

        public void OnProviderEnabled(string provider)
        {
            Toast.MakeText(this, "GPS Enabled!", ToastLength.Short).Show();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
        }

        public override bool OnCreateOptionsMenu(IMenu meniu)
        {
            MenuInflater.Inflate(Resource.Menu.gpsMenu, meniu);
            return base.OnCreateOptionsMenu(meniu);
        }

        private async Task _creareBazaDateFirebase()
        {
            _listaExportata.Visibility = ViewStates.Invisible;
            var _fireBD = new FirebaseClient(_linkBDFirebase);
            var _valoriCurente = await _fireBD.Child("Coordonate curente GPS").OnceAsync<CoordonateGPS>();
            _listaAdrese.Clear();
            _export = null;
            foreach (var _valCurenta in _valoriCurente)
            {
                CoordonateGPS _gps = new CoordonateGPS()
                {
                    IDCoordonate = _valCurenta.Key,
                    Latitudine = _valCurenta.Object.Latitudine,
                    Longitudine = _valCurenta.Object.Longitudine,
                    DataCurentaLocatie = _valCurenta.Object.DataCurentaLocatie,
                    AdresaLocatie = _valCurenta.Object.AdresaLocatie
                };
                _listaAdrese.Add(_gps);
            }
            _export = new Export(this, _listaAdrese);
            _export.NotifyDataSetChanged();
            _listaExportata.Adapter = _export;
            _listaExportata.Visibility = ViewStates.Visible;
        }

        private void _optiuniGPS(object ob, Toolbar.MenuItemClickEventArgs ev)
        {
            switch (ev.Item.ItemId)
            {
                case Resource.Id.gpsSave:
                    _salveazaInFirebase();
                    break;
                case Resource.Id.contactList:
                    Intent _contacte = new Intent(Intent.ActionPick, Android.Provider.ContactsContract.Contacts.ContentUri);
                    _contacte.SetType(Android.Provider.ContactsContract.CommonDataKinds.Phone.ContentType);
                    StartActivityForResult(_contacte, 101);
                    break;
            }
        }

        protected override void OnActivityResult(int _req, [GeneratedEnum] Result _rezultat, Intent _data)
        {
            if(_req == 101 && _rezultat == Result.Ok)
            {
                if (_data == null || _data.Data == null)
                    return;
                var _listaContacte = new Xamarin.Contacts.AddressBook(this)
                {
                    PreferContactAggregation = false
                };
                var _contact = _listaContacte.Load(_data.Data.LastPathSegment);
                var _tel = (from _apel in _contact.Phones
                            where _apel.Type == Xamarin.Contacts.PhoneType.Mobile
                            select _apel.Number).FirstOrDefault();
                if (string.IsNullOrEmpty(_tel))
                {
                    Toast.MakeText(this, "No phone number in this contact, select another one!", ToastLength.Short).Show();
                    return;
                }
                else
                {
                    Intent _suna = new Intent(Intent.ActionCall);
                    _suna.SetData(Android.Net.Uri.Parse("tel:" + _tel));
                    StartActivity(_suna);
                }
            }
        }

        private async void _salveazaInFirebase()
        {
            var _gps = new CoordonateGPS()
            {
                IDCoordonate = string.Empty,
                Latitudine = latEdit.Text,
                Longitudine = longEdit.Text,
                DataCurentaLocatie = dataGPS.Text,
                AdresaLocatie = tvAddress.Text
            };
            if (_gps.Latitudine != string.Empty && _gps.Longitudine != string.Empty && _gps.AdresaLocatie != string.Empty)
            {
                gpsProgress.Visibility = ViewStates.Visible;
                var _firebase = new FirebaseClient(_linkBDFirebase);
                var _topo = await _firebase.Child("Coordonate curente GPS").PostAsync(_gps);
                gpsProgress.Visibility = ViewStates.Gone;
            }
            else
            {
                Toast.MakeText(this, "Current location not set in fields or inaccessible", ToastLength.Long).Show();
            }
        }
    }
}