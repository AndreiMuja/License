using Android.App;
using Android.Widget;
using Android.OS;
using PrevenireRiscIT.Classes;
using Android.Views;
using Newtonsoft.Json;
using System;
using Android.Content;
using PrevenireRiscIT.Fragments;

namespace PrevenireRiscIT.Activities
{
    [Activity(Label = "PrevenireRiscIT", Icon = "@drawable/Lock")]
    public partial class MainActivity : Activity
    {
        private Toolbar menuTool;
        private TextView welcomeTV;
        private TextView selectTV;
        private ImageView profileImage;
        private Button cryptButton;
        private Button contentButton;
        private Button webButton;
        private Button gpsButton;
        private Utilizator _utilizator; 

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            Toast.MakeText(this, "In order to start, click in the center of the screen!", ToastLength.Short).Show();
            menuTool = FindViewById<Toolbar>(Resource.Id.mainToolbar);
            SetActionBar(menuTool);
            welcomeTV = FindViewById<TextView>(Resource.Id.tvProfile);
            selectTV = FindViewById<TextView>(Resource.Id.tvMenu);
            profileImage = FindViewById<ImageView>(Resource.Id.ivMain);
            cryptButton = FindViewById<Button>(Resource.Id.cryptButton);
            contentButton = FindViewById<Button>(Resource.Id.contentButton);
            webButton = FindViewById<Button>(Resource.Id.webButton);
            gpsButton = FindViewById<Button>(Resource.Id.infoButton);

            selectTV.Visibility = ViewStates.Invisible;
            cryptButton.Visibility = ViewStates.Invisible;
            contentButton.Visibility = ViewStates.Invisible;
            webButton.Visibility = ViewStates.Invisible;
            gpsButton.Visibility = ViewStates.Invisible;
            _utilizator = JsonConvert.DeserializeObject<Utilizator>(Intent.GetStringExtra("Username"));
            welcomeTV.Text = "Welcome " + _utilizator.NumeClient;

            profileImage.Click += _startMeniuAplicatie;
            cryptButton.Click += _creareAmprentaUtilizator;
            contentButton.Click += _verificaContinutAdecvat;
            webButton.Click += _acceseazaInternetDinAplicatie;
            gpsButton.Click += _acceseazaGPS;
            menuTool.MenuItemClick += _optiuneAplicatie;
        }

        public override bool OnCreateOptionsMenu(IMenu meniu)
        {
            MenuInflater.Inflate(Resource.Menu.mainMenu, meniu);
            return base.OnCreateOptionsMenu(meniu);
        }

        private void _startMeniuAplicatie(object ob, EventArgs ev)
        {
            selectTV.Visibility = ViewStates.Visible;
            profileImage.Visibility = ViewStates.Visible;
            cryptButton.Visibility = ViewStates.Visible;
            contentButton.Visibility = ViewStates.Visible;
            webButton.Visibility = ViewStates.Visible;
            gpsButton.Visibility = ViewStates.Visible;
            Toast.MakeText(this, "Before going any further, if you're acessing the app for the first time, please click on Info button! Thank you", ToastLength.Short).Show();
        }

        private void _creareAmprentaUtilizator(object ob, EventArgs ev)
        {
            Intent _mergiLaAmprenta = new Intent(this, typeof(Amprenta));
            StartActivity(_mergiLaAmprenta);
            Toast.MakeText(this, "In order to encrypt your data, you must authenticate with you fingerprint",ToastLength.Short).Show();
        }

        private void _verificaContinutAdecvat(object ob, EventArgs ev)
        {
            Intent _mergiLaAmprenta = new Intent(this, typeof(Amprenta));
            StartActivity(_mergiLaAmprenta);
            Toast.MakeText(this, "In order to check your content, you must authenticate with you fingerprint", ToastLength.Short).Show();
        }

        private void _acceseazaInternetDinAplicatie(object ob, EventArgs ev)
        {
            Intent _mergiLaAcces = new Intent(this, typeof(Access));
            StartActivity(_mergiLaAcces);
        }

        private void _acceseazaGPS(object ob, EventArgs ev)
        {
            Intent _mergiLaAmprenta = new Intent(this, typeof(Amprenta));
            StartActivity(_mergiLaAmprenta);
            Toast.MakeText(this, "In order to access your GPS, you must authenticate with you fingerprint", ToastLength.Short).Show();
        }

        private void _optiuneAplicatie(object obiect, Toolbar.MenuItemClickEventArgs ev)
        {
            switch (ev.Item.ItemId)
            {
                case Resource.Id.logout:
                    Intent _delogare = new Intent(this, typeof(Logare));
                    StartActivity(_delogare);
                    NavigateUpTo(_delogare);
                    break;
                case Resource.Id.infoItem:
                    FragmentTransaction _tranz = FragmentManager.BeginTransaction();
                    Info _info = new Info();
                    _info.Show(_tranz, "Information fragment");
                    break;
            }
        }
    }
}