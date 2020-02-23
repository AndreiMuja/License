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
    [Activity(Label = "SchimbareParola")]
    public partial class SchimbareParola : Activity
    {
        private ImageView checkImage;
        private TextView emailCheckTV;
        private EditText emailCheck;
        private TextView passChangeTV;
        private EditText passChange;
        private Button changeButton;
        private TextView changeReq;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SchimbareParola);
            checkImage = FindViewById<ImageView>(Resource.Id.ivParola);
            emailCheckTV = FindViewById<TextView>(Resource.Id.tvCheckEmail);
            emailCheck = FindViewById<EditText>(Resource.Id.editCheckEmail);
            passChangeTV = FindViewById<TextView>(Resource.Id.tvChangePassword);
            passChange = FindViewById<EditText>(Resource.Id.editCheckPassword);
            changeButton = FindViewById<Button>(Resource.Id.Change);
            changeReq = FindViewById<TextView>(Resource.Id.tvChangeRequirements);

            changeButton.Click += _schimbareCuSuccesParola;
        }

        private void _schimbareCuSuccesParola(object ob, EventArgs ev)
        {
            try
            {
                string _bdInfo = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Util.db3");
                var _bdExistent = new SQLiteConnection(_bdInfo);
                var _dateExistente = _bdExistent.Table<Utilizator>();
                string _mailExistent = emailCheck.Text;
                var _cauta = (from valori in _dateExistente where valori.AdresaEmail == _mailExistent select valori).Single();
                _cauta.Parola = passChange.Text;
                _bdExistent.Update(_cauta);
                Toast.MakeText(this, "Password changed successfully! Go back to sign in with your new credentials!", ToastLength.Short).Show();
                Intent _pagLogare = new Intent(this, typeof(Logare));
                StartActivity(_pagLogare);
                NavigateUpTo(_pagLogare);
            }
            catch (SQLiteException)
            {
                emailCheck.SetTextColor(Color.IndianRed);
                Toast.MakeText(this, "Username not registered! Do you have an account? If not, sign up now!", ToastLength.Long).Show();
            }
        }
    }
}