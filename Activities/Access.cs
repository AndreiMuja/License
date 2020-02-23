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
using Android.Webkit;
using PrevenireRiscIT.Fragments;


namespace PrevenireRiscIT.Activities
{
    [Activity(Label = "Access")]
    public partial class Access : Activity, View.IOnClickListener
    {
        private ImageView googleImage;
        private ImageView facebookImage;
        private Button googleButton;
        private Button facebookButton;
        private RadioButton grantRadio;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Access);
            googleImage = FindViewById<ImageView>(Resource.Id.ivGoogle);
            facebookImage = FindViewById<ImageView>(Resource.Id.ivFacebook);
            googleButton = FindViewById<Button>(Resource.Id.btnGoogle);
            facebookButton = FindViewById<Button>(Resource.Id.btnFacebook);
            grantRadio = FindViewById<RadioButton>(Resource.Id.rbAccess);
            Toast.MakeText(this, "By security means, the browsers will not keep any history or cookies. To enable that, click to grant access!", ToastLength.Long).Show();
            googleButton.SetOnClickListener(this);
            facebookButton.SetOnClickListener(this);
            
        }

        public void OnClick(View _acces)
        {
            switch (_acces.Id)
            {
                case Resource.Id.btnGoogle:
                    if (grantRadio.Checked == true)
                    {
                        _incarcaPaginaWeb("http://www.google.com");
                        Toast.MakeText(this, "You are now under potential cross-site scripting, cookies or geolocation detection threat!",ToastLength.Long).Show();
                    }
                    else
                    {
                        _incarcaPaginaWeb("http://www.google.com");
                        CookieManager.Instance.RemoveAllCookie();
                    }
                    break;
                case Resource.Id.btnFacebook:
                    if (grantRadio.Checked == true)
                    {
                        _incarcaPaginaWeb("http://facebook.com");
                        Toast.MakeText(this, "You are now under potential cross-site scripting, cookies or geolocation detection threat!", ToastLength.Long).Show();
                    }
                    else
                    {
                        _incarcaPaginaWeb("http://facebook.com");
                        CookieManager.Instance.RemoveAllCookie();
                    }
                    break;
            }
        }

        private void _incarcaPaginaWeb(string _pagina)
        {
            AccesFragment _accesDiferit = new AccesFragment();
            Bundle _bundle = new Bundle();
            _bundle.PutString("url", _pagina);
            _accesDiferit.Arguments = _bundle;

            FragmentManager _fragment = FragmentManager;
            FragmentTransaction _tranzitie = _fragment.BeginTransaction();
            _tranzitie.Add(Resource.Id.fragmentAccess, _accesDiferit);
            _tranzitie.Commit();
        }
    }
}