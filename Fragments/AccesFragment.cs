using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Webkit;

namespace PrevenireRiscIT.Fragments
{
    public partial class AccesFragment : Fragment
    {
        public ProgressBar progressPage;
        private View viewPage;
        public WebView _internetPage;
        private string _pagina;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Bundle _bundle = Arguments;
            _pagina = _bundle.GetString("url");
        }

        public override View OnCreateView(LayoutInflater _infl, ViewGroup _continut, Bundle _instanta)
        {
            viewPage = _infl.Inflate(Resource.Layout.AccesFragment, _continut, false);
            progressPage = viewPage.FindViewById<ProgressBar>(Resource.Id.progressWebPage);
            _internetPage = (WebView)viewPage.FindViewById<View>(Resource.Id.webViewPage);
            _acceseazaInternet();
            return viewPage;
        }

        private void _acceseazaInternet()
        {
            WebSettings _setari = _internetPage.Settings;
            _setari.JavaScriptEnabled = true;
            _internetPage.ScrollBarStyle = ScrollbarStyles.InsideOverlay;

            _internetPage.SetWebChromeClient(new ClientChrome(this));
            _internetPage.SetWebViewClient(new Client(this));
            _internetPage.LoadUrl(_pagina);
        }
    }

    internal partial class ClientChrome : WebChromeClient
    {
        private AccesFragment _accesFragment;

        public ClientChrome(AccesFragment _accesFragment)
        {
            this._accesFragment = _accesFragment;
        }

        public override void OnProgressChanged(WebView _vedere, int _progres)
        {
            base.OnProgressChanged(_vedere, _progres);
            _accesFragment.progressPage.Progress = _progres;
            if (_progres == 100)
            {
                _accesFragment.progressPage.Visibility = ViewStates.Invisible;
            }
        }
    }

    internal partial class Client : WebViewClient
    {
        private AccesFragment _accesFragment;

        public Client(AccesFragment _accesFragment)
        {
            this._accesFragment = _accesFragment;
        }

        public override bool ShouldOverrideUrlLoading(WebView _vedere, IWebResourceRequest _cerere)
        {
            _vedere.LoadUrl(_cerere.Url.ToString());
            return false;
        }

        public override void OnPageFinished(WebView _vedere, string _cale)
        {
            if (_accesFragment.progressPage != null)
            {
                _accesFragment._internetPage.Settings.SetAppCacheEnabled(false);
                _accesFragment._internetPage.Settings.SetGeolocationEnabled(false);
                WebSettings _setari = _accesFragment._internetPage.Settings;
                _setari.JavaScriptEnabled = false;
                _accesFragment.progressPage.Visibility = ViewStates.Invisible;
            }
        }

        public override void OnReceivedError(WebView _vedere, IWebResourceRequest _cerere, WebResourceError _eroare)
        {
            if (_accesFragment.progressPage != null)
            {
                Toast.MakeText(Application.Context, "Error, refresh by clicking on the button or go to other page", ToastLength.Short).Show();
                _accesFragment.progressPage.Visibility = ViewStates.Invisible;
            }
        }
    }
}