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
using PrevenireRiscIT.Classes;
using Firebase.Xamarin.Database;
using Java.Lang;

namespace PrevenireRiscIT.Activities
{
    [Activity(Label = "Export")]
    public partial class Export :BaseAdapter
    {
        private List<CoordonateGPS> _listaLocatii;
        private TextView latExport;
        private TextView longExport;
        private TextView dateExport;
        private TextView addressExport;
        private LayoutInflater _inflator;
        private Activity _activitate;

        public Export(Activity _activ, List<CoordonateGPS> _ls)
        {
            _activitate = _activ;
            _listaLocatii = _ls;
        }

        public override int Count
        {
            get { return _listaLocatii.Count; }
        }

        public override Java.Lang.Object GetItem(int _pozitie)
        {
            return _pozitie;
        }

        public override long GetItemId(int _pozitie)
        {
            return _pozitie;
        }

        public override View GetView(int _pozitie, View _convert, ViewGroup _parinte)
        {
            _inflator = (LayoutInflater)_activitate.BaseContext.GetSystemService(Context.LayoutInflaterService);
            View _vedere = _inflator.Inflate(Resource.Layout.Export, null);
            latExport = _vedere.FindViewById<TextView>(Resource.Id.exportLatitude);
            longExport = _vedere.FindViewById<TextView>(Resource.Id.exportLongitude);
            dateExport = _vedere.FindViewById<TextView>(Resource.Id.exportDate);
            addressExport = _vedere.FindViewById<TextView>(Resource.Id.exportAddress);

            if (_listaLocatii.Count > 0)
            {
                latExport.Text = _listaLocatii[_pozitie].Latitudine;
                longExport.Text = _listaLocatii[_pozitie].Longitudine;
                dateExport.Text = _listaLocatii[_pozitie].DataCurentaLocatie;
                addressExport.Text = _listaLocatii[_pozitie].AdresaLocatie;
            }
            return _vedere;
        }
    }
}