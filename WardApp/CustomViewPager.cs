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
using Android.Support.V4.View;
using Android.Util;
using WardApp.Fragments;

namespace WardApp
{
    public class CustomViewPager : ViewPager
    {
        private bool pageSwipe;
        public bool PageSwipe
        {
            get
            {
                return pageSwipe;
            }

            set
            {
                pageSwipe = value;
            }
        }

        public CustomViewPager(Context ctx, IAttributeSet attrs) : base(ctx)
        {
            pageSwipe = true;
        }

        public override void SetCurrentItem(int item, bool smoothScroll)
        {
            //((NrViewPagerAdapter)Adapter).GetPage(item).LoadSalas();

            base.SetCurrentItem(item, smoothScroll);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (pageSwipe)
                return base.OnTouchEvent(e);
            
            return false;           
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (pageSwipe)
                return base.OnInterceptTouchEvent(ev);

            return false;
        }
    }
}