using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using PrevenireRiscIT.Classes;
using Android.Graphics;
using System.Text.RegularExpressions;
using SQLite;
using System;
using SQLite.Net.Cipher.Security;
using SQLite.Net.Cipher.Interfaces;
using System.Linq;

namespace PrevenireRiscIT.Activities
{
    [Activity(Label = "Inregistrare")]
    public partial class Inregistrare : Activity
    {
        private ImageView regImage;
        private TextView userTV;
        private EditText userEdit;
        private TextView passTV;
        private EditText passEdit;
        private TextView emailTV;
        private EditText emailEdit;
        private Button regButton;
        private TextView requirements;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Inregistrare);
            regImage = FindViewById<ImageView>(Resource.Id.ivImage);
            userTV = FindViewById<TextView>(Resource.Id.tvUser);
            userEdit = FindViewById<EditText>(Resource.Id.editUsername);
            passTV = FindViewById<TextView>(Resource.Id.tvPassword);
            passEdit = FindViewById<EditText>(Resource.Id.editPassword);
            emailTV = FindViewById<TextView>(Resource.Id.tvEmail);
            emailEdit = FindViewById<EditText>(Resource.Id.editEmail);
            regButton = FindViewById<Button>(Resource.Id.Register);
            requirements = FindViewById<TextView>(Resource.Id.tvRequirements);

            regButton.Click += _inregistrareCuSucces;
        }

        private void _inregistrareCuSucces(object ob, EventArgs ev)
        {
            var _client = new Utilizator()
            {
                NumeClient = userEdit.Text,
                Parola = passEdit.Text,
                AdresaEmail = emailEdit.Text
            };
            var m_reg_succes = _validareInregistrare(_client);
            if (m_reg_succes)
            {
                _inregistrareInBazaDeDate(_client);
                //_notificare(_client.AdresaEmail);
            }
            else
            {
                AlertDialog.Builder alerta = new AlertDialog.Builder(this);
                alerta.SetTitle("Warning!");
                alerta.SetMessage("Invalid registration, please check your credentials!");
                alerta.SetNeutralButton("OK", (senderAlert, args) =>
                {
                    Toast.MakeText(this, "Try again!", ToastLength.Short).Show();
                });
                alerta.Show();
            }
        }

        private bool _validareInregistrare(Utilizator _client)
        {
            if (_valideazaUsername(_client.NumeClient) && _valideazaParola(_client.Parola)
                && _valideazaEmail(_client.AdresaEmail))
                return true;
            else
                return false;
        }

        private bool _valideazaUsername(string _user)
        {
            const string _username = @"^[A-Za-z]+$";
            _user = userEdit.Text;
            bool _valid = false;
            if (_valid = Regex.IsMatch(_user, _username) && _user != "")
            {
                userEdit.SetTextColor(Color.DarkGreen);
                userEdit.SetBackgroundColor(Color.White);
                userEdit.SetBackgroundResource(Resource.Drawable.StyleEditText);
                return true;
            }
            else
            {
                userEdit.SetTextColor(Color.Black);
                userEdit.SetBackgroundColor(Color.IndianRed);
                return false;
            }
        }

        private bool _valideazaParola(string _parol)
        {
            const string _parola = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[$@$!%*#?&])[A-Za-z\d$@$!%*#?&]{8,50}$";
            _parol = passEdit.Text;
            bool _valid = false;
            if (_valid = Regex.IsMatch(_parol, _parola) && _parol != "")
            {
                passEdit.SetTextColor(Color.DarkGreen);
                passEdit.SetBackgroundColor(Color.White);
                passEdit.SetBackgroundResource(Resource.Drawable.StyleEditText);
                return true;
            }
            else
            {
                passEdit.SetTextColor(Color.Black);
                passEdit.SetBackgroundColor(Color.IndianRed);
                return false;
            }
        }

        private bool _esteEmailValid(string email)
        {
            return Android.Util.Patterns.EmailAddress.Matcher(email).Matches();
        }

        private bool _valideazaEmail(string _email)
        {
            _email = emailEdit.Text;
            var _valid = _esteEmailValid(_email);
            if (_valid && _email != "")
            {
                emailEdit.SetTextColor(Color.DarkGreen);
                emailEdit.SetBackgroundColor(Color.White);
                emailEdit.SetBackgroundResource(Resource.Drawable.StyleEditText);
                return true;
            }
            else
            {
                emailEdit.SetTextColor(Color.Black);
                emailEdit.SetBackgroundColor(Color.IndianRed);
                return false;
            }
        }

        private void _inregistrareInBazaDeDate(Utilizator _client)
        {
            string _bdInfo = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Util.db3");
            var _bd = new SQLiteConnection(_bdInfo);
            _bd.CreateTable<Utilizator>();
            _client.NumeClient = userEdit.Text;
            _client.Parola = passEdit.Text;
            _client.AdresaEmail = emailEdit.Text;
            try
            {
                _bd.Insert(_client);
                Intent laLogare = new Intent(this, typeof(Logare));
                StartActivity(laLogare);
                Finish();
            }
            catch (SQLiteException)
            {
                emailEdit.SetTextColor(Color.IndianRed);
                Toast.MakeText(this, "Email already used!", ToastLength.Short).Show();
            }
        }

        private void _notificare(string _adr)
        {
            _adr = emailEdit.Text;
            Intent _mail = new Intent(Intent.ActionSendto);
            _mail.PutExtra(Intent.ExtraEmail, _adr);
            _mail.PutExtra(Intent.ExtraSubject, "Authentication succeeded!");
            _mail.PutExtra(Intent.ExtraText, "Welcome to RiskIT app!");
            _mail.SetType("message/rfc822");
            StartActivity(Intent.CreateChooser(_mail, "Send mail notification"));
        }
    }
}