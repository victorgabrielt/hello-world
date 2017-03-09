using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace WardApp.Dialogs
{
    class DatepickerDialog : DialogFragment
    {
        private string text;

        public DatepickerDialog(string loadText)
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
            View v = inflater.Inflate(Resource.Layout.dialog_datepicker, container, false);
            DatePicker dtpicker = v.FindViewById<DatePicker>(Resource.Id.dtpFilterReserva);

            return v;
        }
    }
}