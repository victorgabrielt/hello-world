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
using Android.Support.V7.App;
using System.Threading.Tasks;
using Android.Util;

namespace WardApp.Activities
{
    [Activity(Theme = "@style/SplashTheme", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        protected override void OnResume()
        {
            base.OnResume();

            Task startupWork = new Task(() => 
            {
                Task.Delay(3000);  // Simulate a bit of startup work.
            });

            startupWork.ContinueWith(t => 
            {
                StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
            }, TaskScheduler.FromCurrentSynchronizationContext());

            startupWork.Start();
        }
    }
}