
using Android.Views;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Java.Interop;
using Android.Content;
using Android.Util;
using Android.Widget;

namespace WardApp
{
    class ScrollFABBehavior : CoordinatorLayout.Behavior
    {
        Context ctx;
        public ScrollFABBehavior(Context context, IAttributeSet attrs) : base()
        { ctx = context; }

        public override bool OnStartNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View directTargetChild, View target, int nestedScrollAxes)
        {
            return nestedScrollAxes == ViewCompat.ScrollAxisVertical || base.OnStartNestedScroll(coordinatorLayout, child, directTargetChild, target, nestedScrollAxes);
        }

        public override void OnNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dxConsumed, int dyConsumed, int dxUnconsumed, int dyUnconsumed)
        {
            base.OnNestedScroll(coordinatorLayout, child, target, dxConsumed, dyConsumed, dxUnconsumed, dyUnconsumed);
            //FloatingActionButton fab = child.JavaCast<FloatingActionButton>();
            //if (fab.Visibility != ViewStates.Invisible)
            //{
            //    if (dyUnconsumed > 0 && fab.Visibility == ViewStates.Visible)
            //    {
            //        fab.Hide();
            //    }
            //    else if (dyUnconsumed <= 0 && fab.Visibility != ViewStates.Visible)
            //    {
            //        fab.Show();
            //    }
            //}
        }
    }
}