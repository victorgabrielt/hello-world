
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;

namespace WardApp
{
    public abstract class BaseActivity : AppCompatActivity
    {
        int theme = -1;

        public Toolbar Toolbar
        {
            get;
            set;
        }
                     

        protected override void OnCreate(Bundle bundle)
        {
            if (Database.local.appTheme == (int)Enums.AppTheme.LightTheme)
            {
                theme = 0;
                SetTheme(Resource.Style.Theme2Light);
            }
            else
            {
                theme = 1;
                SetTheme(Resource.Style.Theme2);
            }
            
            base.OnCreate(bundle);
            SetContentView(LayoutResource);
            Toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            if (Toolbar != null)
            {
                SetSupportActionBar(Toolbar);
                
            }
        }

        protected abstract int LayoutResource
        {
            get;
        }

        protected int ActionBarIcon
        {
            set { Toolbar.SetNavigationIcon(value); }
        }

        protected override void OnRestart()
        {
            base.OnRestart();

            if (theme != Database.local.appTheme)
            {
                if (Database.local.appTheme == (int)Enums.AppTheme.LightTheme)
                {
                    theme = 0;
                    SetTheme(Resource.Style.Theme2Light);
                }
                else
                {
                    theme = 1;
                    SetTheme(Resource.Style.Theme2);
                }

                this.Recreate();
            }
           
        }
    }


}