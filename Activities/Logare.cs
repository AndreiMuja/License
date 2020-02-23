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
using SQLite;
using PrevenireRiscIT.Classes;
using System.Threading;
using Android.Graphics;
using Newtonsoft.Json;
using SQLite.Net.Cipher.Security;
using SQLite.Net.Cipher.Interfaces;

namespace PrevenireRiscIT.Activities
{
    [Activity(Label = "Logare",MainLauncher =true)]
    public partial class Logare : Activity
    {
        private ImageView logImage;
        private EditText userLogin;
        private EditText emailLogin;
        private EditText passLogin;
        private Button logButton;
        private TextView firstAccess;
        private TextView forgotTV;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Logare);
            logImage = FindViewById<ImageView>(Resource.Id.loginImage);
            userLogin = FindViewById<EditText>(Resource.Id.username);
            emailLogin = FindViewById<EditText>(Resource.Id.emailClient);
            passLogin = FindViewById<EditText>(Resource.Id.parola);
            logButton = FindViewById<Button>(Resource.Id.Login);
            firstAccess = FindViewById<TextView>(Resource.Id.tvSignUp);
            forgotTV = FindViewById<TextView>(Resource.Id.tvForgot);

            firstAccess.Click += _inscriereAplicatie;
            logButton.Click += _verificareCont;
            forgotTV.Click += _schimbareParola;
        }

        private void _inscriereAplicatie(object ob, EventArgs ev)
        {
            Intent _laInreg = new Intent(this, typeof(Inregistrare));
            StartActivity(_laInreg);
            Finish();
        }

        private void _verificareCont(object ob, EventArgs ev)
        {
            try
            {
                string _bdInfo = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Util.db3");
                var _bd = new SQLiteConnection(_bdInfo);
                var _informatii = _bd.Table<Utilizator>();
                var _verifica = _informatii.Where(cl => cl.AdresaEmail == emailLogin.Text && cl.Parola == passLogin.Text
                && cl.NumeClient==userLogin.Text).FirstOrDefault();
                if (_verifica != null)
                {
                    ProgressDialog _progres = new ProgressDialog(this);
                    _progres.SetMessage("Authenticating, please wait...");
                    _progres.SetProgressStyle(ProgressDialogStyle.Horizontal);
                    _progres.Progress = 0;
                    _progres.Max = 100;
                    _progres.Show();
                    int _incarca = 0;
                    new Thread(new ThreadStart(delegate
                    {
                        while (_incarca < 100)
                        {
                            _incarca += 25;
                            _progres.Progress = _incarca;
                            Thread.Sleep(500);
                        }
                        Intent pag = new Intent(this, typeof(MainActivity));
                        Utilizator _util = new Utilizator
                        {
                            NumeClient=userLogin.Text
                        };
                        pag.PutExtra("Username", JsonConvert.SerializeObject(_util));
                        StartActivity(pag);
                        Finish();
                    })).Start();
                }
                else
                {
                    Toast.MakeText(this, "Incorrect credentials! Do you have an account? If yes, try again!", ToastLength.Short).Show();
                    emailLogin.SetTextColor(Color.IndianRed);
                    passLogin.SetTextColor(Color.IndianRed);
                }
            }
            catch (SQLiteException eroare)
            {
                Toast.MakeText(this, eroare.ToString(), ToastLength.Short).Show();
            }
        }

        private void _schimbareParola(object ob, EventArgs ev)
        {
            Intent _laSchimb = new Intent(this, typeof(SchimbareParola));
            StartActivity(_laSchimb);
            NavigateUpTo(_laSchimb);
        }
    }
}