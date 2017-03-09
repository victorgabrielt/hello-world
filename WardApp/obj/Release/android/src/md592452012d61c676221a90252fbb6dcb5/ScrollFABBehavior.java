package md592452012d61c676221a90252fbb6dcb5;


public class ScrollFABBehavior
	extends android.support.design.widget.CoordinatorLayout.Behavior
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onStartNestedScroll:(Landroid/support/design/widget/CoordinatorLayout;Landroid/view/View;Landroid/view/View;Landroid/view/View;I)Z:GetOnStartNestedScroll_Landroid_support_design_widget_CoordinatorLayout_Landroid_view_View_Landroid_view_View_Landroid_view_View_IHandler\n" +
			"n_onNestedScroll:(Landroid/support/design/widget/CoordinatorLayout;Landroid/view/View;Landroid/view/View;IIII)V:GetOnNestedScroll_Landroid_support_design_widget_CoordinatorLayout_Landroid_view_View_Landroid_view_View_IIIIHandler\n" +
			"";
		mono.android.Runtime.register ("WardApp.ScrollFABBehavior, WardApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ScrollFABBehavior.class, __md_methods);
	}


	public ScrollFABBehavior () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ScrollFABBehavior.class)
			mono.android.TypeManager.Activate ("WardApp.ScrollFABBehavior, WardApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public ScrollFABBehavior (android.content.Context p0, android.util.AttributeSet p1) throws java.lang.Throwable
	{
		super (p0, p1);
		if (getClass () == ScrollFABBehavior.class)
			mono.android.TypeManager.Activate ("WardApp.ScrollFABBehavior, WardApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0, p1 });
	}


	public boolean onStartNestedScroll (android.support.design.widget.CoordinatorLayout p0, android.view.View p1, android.view.View p2, android.view.View p3, int p4)
	{
		return n_onStartNestedScroll (p0, p1, p2, p3, p4);
	}

	private native boolean n_onStartNestedScroll (android.support.design.widget.CoordinatorLayout p0, android.view.View p1, android.view.View p2, android.view.View p3, int p4);


	public void onNestedScroll (android.support.design.widget.CoordinatorLayout p0, android.view.View p1, android.view.View p2, int p3, int p4, int p5, int p6)
	{
		n_onNestedScroll (p0, p1, p2, p3, p4, p5, p6);
	}

	private native void n_onNestedScroll (android.support.design.widget.CoordinatorLayout p0, android.view.View p1, android.view.View p2, int p3, int p4, int p5, int p6);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
