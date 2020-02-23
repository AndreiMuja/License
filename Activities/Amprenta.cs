using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Security.Keystore;
using Android;
using Java.Security;
using Javax.Crypto;
using Android.Hardware.Fingerprints;
using PrevenireRiscIT.Classes;
using Android.Content;

namespace PrevenireRiscIT.Activities
{
    [Activity(Label = "Amprenta")]
    public partial class Amprenta : Activity
    {
        private ImageView ftipsImage;
        private TextView ftipTV;
        private Button gotoCriptare;
        private Button gotoGPS;
        private Button gotoContinut;
        private KeyStore _creareCheie;
        private Cipher _cifru;
        private static readonly string _cheie = "CheiePtCriptare";
        private static readonly string _algoCheie = KeyProperties.KeyAlgorithmAes;
        private static readonly string _dependenta = KeyProperties.BlockModeCbc;
        private static readonly string _umplere = KeyProperties.EncryptionPaddingPkcs7;
        private static readonly string _transforma = _algoCheie + "/" + _dependenta + "/" + _umplere;
        private static readonly string _permisiune = Manifest.Permission.UseFingerprint;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Amprenta);
            ftipsImage = FindViewById<ImageView>(Resource.Id.ivFingertip);
            ftipTV = FindViewById<TextView>(Resource.Id.tvFingertip);
            gotoCriptare = FindViewById<Button>(Resource.Id.gotoCriptare);
            gotoGPS = FindViewById<Button>(Resource.Id.gotoGPS);
            gotoContinut = FindViewById<Button>(Resource.Id.gotoContinut);
            gotoCriptare.Visibility = ViewStates.Invisible;
            gotoGPS.Visibility = ViewStates.Invisible;
            gotoContinut.Visibility = ViewStates.Invisible;
            ftipsImage.Click += _autentificareAmprenta; 
        }

        private void _autentificareAmprenta(object ob, EventArgs ev) 
        {
            KeyguardManager _manager = (KeyguardManager)GetSystemService(KeyguardService);
            FingerprintManager _amprente = (FingerprintManager)GetSystemService(FingerprintService);
            if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, _permisiune) != (int)Android.Content.PM.Permission.Granted)
                return;
            if (!_amprente.IsHardwareDetected)
                Toast.MakeText(this, "The use of fingertip authentication is disabled", ToastLength.Short).Show();
            else
            {
                if (!_amprente.HasEnrolledFingerprints)
                    Toast.MakeText(this, "No fingertips registered in our app", ToastLength.Short).Show();
                else
                {
                    if (!_manager.IsKeyguardSecure)
                        Toast.MakeText(this, "Please enable lock screen security!", ToastLength.Short).Show();
                    else _algoritmCriptare();
                    if (_genereazaCifru())
                    {
                        FingerprintManager.CryptoObject _criptat = new FingerprintManager.CryptoObject(_cifru);
                        CreatorAmprenta _creator = new CreatorAmprenta(this);
                        _creator._startAutentificare(_amprente, _criptat);
                        gotoCriptare.Visibility = ViewStates.Visible;
                        gotoGPS.Visibility = ViewStates.Visible;
                        gotoContinut.Visibility = ViewStates.Visible;
                        ftipTV.Text = "Now you can use any of the options below";
                        gotoCriptare.Click += _accesCriptarePosibil;
                        gotoGPS.Click += _accesGPSPosibil;
                        gotoContinut.Click += _accesContinutPosibil;
                    }
                }
            }
        } 

        private void _accesCriptarePosibil(object ob, EventArgs ev)
        {
            Intent _mergiLaCriptare = new Intent(this, typeof(Criptare));
            StartActivity(_mergiLaCriptare);
            Finish();
        }

        private void _accesGPSPosibil(object ob, EventArgs ev)
        {
            Intent _mergiLaGPS = new Intent(this, typeof(GPS));
            StartActivity(_mergiLaGPS);
            Finish();
        }

        private void _accesContinutPosibil(object ob, EventArgs ev)
        {
            Intent _mergiLaContinut = new Intent(this, typeof(Continut));
            StartActivity(_mergiLaContinut);
            Finish();
        }

        private void _algoritmCriptare()
        {
            _creareCheie = KeyStore.GetInstance("AndroidKeyStore");
            KeyGenerator _generator = null;
            _generator = KeyGenerator.GetInstance(_algoCheie, "AndroidKeyStore");
            _creareCheie.Load(null);
            _generator.Init(new KeyGenParameterSpec.Builder(_cheie, KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                .SetBlockModes(_dependenta).SetUserAuthenticationRequired(true)
                .SetEncryptionPaddings(_umplere).Build());
            _generator.GenerateKey();

        }

        private bool _genereazaCifru()
        {
            try
            {
                _cifru = Cipher.GetInstance(_transforma);
                _creareCheie.Load(null);
                IKey _ch = _creareCheie.GetKey(_cheie, null);
                _cifru.Init(CipherMode.EncryptMode, _ch);
                return true;
            }
            catch (Exception)
            {
                Toast.MakeText(this, "Can't generate encryption key for you", ToastLength.Short).Show();
                return false;
            }
        }
    }
}