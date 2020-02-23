using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace PrevenireRiscIT.Fragments
{
    public class Info : DialogFragment
    {
        public override View OnCreateView(LayoutInflater _infl, ViewGroup _continut, Bundle _instanta)
        {
            base.OnCreateView(_infl, _continut, _instanta);
            View _vedere = _infl.Inflate(Resource.Layout.InfoDialog, _continut, false);
            return _vedere;
        }

        public override void OnActivityCreated(Bundle _instanta)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(_instanta);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.InfoDialogAnimation;
        }
    }
}