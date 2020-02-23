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
using Android.Graphics;
using SQLite.Net.Cipher.Security;
using SQLite.Net.Cipher.Interfaces;

namespace PrevenireRiscIT.Activities
{
    [Activity(Label = "Criptare")]
    public partial class Criptare : Activity
    {
        private ImageView cryptImage;
        private EditText userCrypt;
        private EditText emailCrypt;
        private EditText passCrypt;
        private Button cryptBtn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Criptare);
            Toast.MakeText(this, "Your account data is not that safe as you think! For greater security measures, " +
                " they should be encrypted as soon as you login. In this application we will show you how it must be done." +
                " Use this activity to encrypt your data and save your credentials in other table so that a potential attacker could " +
                " not read them.", ToastLength.Long).Show();
            cryptImage = FindViewById<ImageView>(Resource.Id.cryptImage);
            userCrypt = FindViewById<EditText>(Resource.Id.userCrypt);
            emailCrypt = FindViewById<EditText>(Resource.Id.emailCrypt);
            passCrypt = FindViewById<EditText>(Resource.Id.parolaCrypt);
            cryptBtn = FindViewById<Button>(Resource.Id.saveCrypt);

            cryptBtn.Click += _verificaUtilizatorBackup;
         }

        private void _verificaUtilizatorBackup(object ob, EventArgs ev)
        {
            try
            {
                string _bdInfo = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Util.db3");
                var _bd = new SQLiteConnection(_bdInfo);
                var _informatii = _bd.Table<Utilizator>();
                var _verifica = _informatii.Where(cl => cl.AdresaEmail == emailCrypt.Text && cl.Parola == passCrypt.Text
                                                  && cl.NumeClient == userCrypt.Text).FirstOrDefault();
                if (_verifica != null)
                {
                    var _ut = new UtilCriptat
                    {
                        //Id = Guid.NewGuid().ToString(),
                        NumeClient = userCrypt.Text,
                        AdresaEmail = emailCrypt.Text,
                        Parola = passCrypt.Text
                    };
                    string _bdInfoCriptare = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Cript.db3");
                    var _bdCriptare = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();
                    string _incriptare = CryptoService.GenerateRandomKey(24);
                    var _cheie = "Prevenire RiscIT";
                    ISecureDatabase _bdSecurizata = new BDUtilizator(_bdCriptare, _bdInfoCriptare, _incriptare);

                    var _ch = _bdSecurizata.SecureQuery<UtilCriptat>("SELECT * FROM UtilCriptat WHERE AdresaEmail= '" + _ut.AdresaEmail + "'AND Parola= '" + _ut.Parola + "'AND NumeClient='" + _ut.NumeClient + "'", _cheie, 0).FirstOrDefault();
                    if (_ch != null)
                    {
                        try
                        {
                            _bdSecurizata.SecureInsert(_ut, _cheie);
                            userCrypt.SetTextColor(Color.DarkGreen);
                            emailCrypt.SetTextColor(Color.DarkGreen);
                            passCrypt.SetTextColor(Color.DarkGreen);
                            Toast.MakeText(this, "Data encrypted. Backup security succeedeed!", ToastLength.Short).Show();
                        }
                        catch (SQLiteException)
                        {
                            Toast.MakeText(this, "We could not encrypt data or data already encrypted..Backup security failed!", ToastLength.Short).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, "Data already encrypted...", ToastLength.Short).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this, "Incorrect credentials! Do you have an account? If yes, try again!", ToastLength.Short).Show();
                    userCrypt.SetTextColor(Color.IndianRed);
                    emailCrypt.SetTextColor(Color.IndianRed);
                    passCrypt.SetTextColor(Color.IndianRed);
                }
            }
            catch (SQLiteException eroare)
            {
                Toast.MakeText(this, eroare.ToString(), ToastLength.Short).Show();
            }
        }
    }
}