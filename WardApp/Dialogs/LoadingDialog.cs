using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
//using Android.App;
//using Android.App;

namespace WardApp.Dialogs
{
    public class LoadingDialog : DialogFragment
    {
        private string text;

        public LoadingDialog(string loadText)
        {
            text = loadText;
        }

        public override void OnStart()
        {
            base.OnStart();            
            Dialog.Window.SetLayout(WindowManagerLayoutParams.WrapContent, WindowManagerLayoutParams.WrapContent);
            
        }        

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            Dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            View v = inflater.Inflate(Resource.Layout.dialog_loading_screen, container, false);
            TextView lblLoadScreen = v.FindViewById<TextView>(Resource.Id.lblLoadScreen);

            lblLoadScreen.Text = text;
            return v;
        }
    }
}