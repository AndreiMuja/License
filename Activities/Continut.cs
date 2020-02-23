using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Com.Microsoft.Projectoxford.Vision;
using System.IO;
using Com.Microsoft.Projectoxford.Vision.Contract;
using GoogleGson;
using Newtonsoft.Json;
using PrevenireRiscIT.Model;
using Android.Content;
using System;
using Android.Runtime;
using Android.Provider;
using Android.Database;
using Android.Graphics.Drawables;

namespace PrevenireRiscIT.Activities
{
    [Activity(Label = "Continut")]
    public partial class Continut : Activity
    {
        public VisionServiceRestClient _clientVision = new VisionServiceRestClient("b44f81b5a56c434887b955f00b69a3de");
        public static readonly int _alegere = 1000;
        private Bitmap _convertor;
        private ImageView contentImage;
        public RadioButton adeqRadio;
        public RadioButton inadeqRadio;
        public RadioButton contRadio;
        private Button testerButton;
        private Button leftButton;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Continut);
            Toast.MakeText(this, "Choose a photo from your device by clicking in the center of this activity, then check its content " +
                " with the button below", ToastLength.Long).Show();
            contentImage = FindViewById<ImageView>(Resource.Id.ivMediaContent);
            adeqRadio = FindViewById<RadioButton>(Resource.Id.adecvatRadio);
            inadeqRadio = FindViewById<RadioButton>(Resource.Id.neadecvatRadio);
            contRadio = FindViewById<RadioButton>(Resource.Id.contentRadio);
            testerButton = FindViewById<Button>(Resource.Id.mediaButton);
            leftButton = FindViewById<Button>(Resource.Id.left);

            testerButton.Visibility = ViewStates.Invisible;
            contentImage.Click += _deschideGalerie;
        }

        private void _deschideGalerie(object ob, EventArgs ev)
        {
            Intent _intent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
            StartActivityForResult(_intent, _alegere);
        }

        protected override void OnActivityResult(int _req, [GeneratedEnum] Result _res, Intent _data)
        {
            if ((_req == _alegere) && (_res == Result.Ok) && (_data != null))
            {
                Android.Net.Uri _uri = _data.Data;
                string[] _proiectie = { MediaStore.Images.Media.InterfaceConsts.Data };
                ICursor _c = ContentResolver.Query(_uri, _proiectie, null, null, null);
                _c.MoveToFirst();
                int _index = _c.GetColumnIndex(_proiectie[0]);
                string _cale = _c.GetString(_index);
                _c.Close();
                _convertor = BitmapFactory.DecodeFile(_cale);
                Drawable _draw = new BitmapDrawable(_convertor);
                contentImage.Background = _draw;
            }
            byte[] _date;
            using (var _memo = new MemoryStream())
            {
                _convertor.Compress(Bitmap.CompressFormat.Jpeg, 100, _memo);
                _date = _memo.ToArray();
            }
            Stream _intrare = new MemoryStream(_date);
            testerButton.Visibility = ViewStates.Visible;
            testerButton.Click += delegate
            {
                new AnalizaContinut(this).Execute(_intrare);
            };
        }
    }

    internal partial class AnalizaContinut : AsyncTask<Stream, string, string>
    {
        private Continut _continut;
        private ProgressDialog _dialog = new ProgressDialog(Application.Context);

        public AnalizaContinut(Continut _continut)
        {
            this._continut = _continut;

        }

        protected override string RunInBackground(params Stream[] @params)
        {
            try
            {
                PublishProgress("Checking image...");
                string[] _cont = { "Adult" };
                AnalysisResult _rezultat = _continut._clientVision.AnalyzeImage(@params[0], _cont, null);
                string _sirRezultat = new Gson().ToJson(_rezultat);
                return _sirRezultat;
            }
            catch (Java.Lang.Exception)
            {
                return null;
            }
        }

        protected override void OnPreExecute()
        {
            _dialog.Window.SetType(WindowManagerTypes.SystemAlert);
            _dialog.Show();
        }

        protected override void OnProgressUpdate(params string[] _val)
        {
            _dialog.SetMessage(_val[0]);
        }

        protected override void OnPostExecute(string _rezultat)
        {
            _dialog.Dismiss();
            AdultModel _model = JsonConvert.DeserializeObject<AdultModel>(_rezultat);
            TextView descripTV = _continut.FindViewById<TextView>(Resource.Id.mediaTV);
            System.Text.StringBuilder _creator = new System.Text.StringBuilder();
            _creator.Append("Image type: " + _model.metadata.format + " Width: " + _model.metadata.width + " Height: " + _model.metadata.height);
            if (_model.adult.isAdultContent == true)
                _continut.inadeqRadio.Checked = true;
            else if (_model.adult.isRacyContent == true)
                _continut.contRadio.Checked = true;
            else
                _continut.adeqRadio.Checked = true;
            descripTV.Text = _creator.ToString();
        }
    }
}