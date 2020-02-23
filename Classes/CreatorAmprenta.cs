using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Hardware.Fingerprints;
using Android;

namespace PrevenireRiscIT.Classes
{
    internal partial class CreatorAmprenta:FingerprintManager.AuthenticationCallback
    {
        private Context _amprenta;

        public CreatorAmprenta(Context _amprenta)
        {
            this._amprenta = _amprenta;
        }

        internal void _startAutentificare(FingerprintManager amp, FingerprintManager.CryptoObject criptat)
        {
            CancellationSignal _semnal = new CancellationSignal();
            if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(_amprenta, Manifest.Permission.UseFingerprint) != (int)Android.Content.PM.Permission.Granted)
                return;
            amp.Authenticate(criptat, _semnal, 0, this, null);
        }

        public override void OnAuthenticationFailed()
        {
            Toast.MakeText(_amprenta, "Authentication did not succeed!", ToastLength.Short).Show();
        }

        public override void OnAuthenticationSucceeded(FingerprintManager.AuthenticationResult result)
        {
            Toast.MakeText(_amprenta, "Authentication succeeded!", ToastLength.Short).Show();
        }
    }
}