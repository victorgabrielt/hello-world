package md5dedb204a94b0ea25755a3c1aa77240ee;


public class DatepickerDialog
	extends android.support.v4.app.DialogFragment
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onStart:()V:GetOnStartHandler\n" +
			"n_onCreateView:(Landroid/view/LayoutInflater;Landroid/view/ViewGroup;Landroid/os/Bundle;)Landroid/view/View;:GetOnCreateView_Landroid_view_LayoutInflater_Landroid_view_ViewGroup_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("WardApp.Dialogs.DatepickerDialog, WardApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", DatepickerDialog.class, __md_methods);
	}


	public DatepickerDialog () throws java.lang.Throwable
	{
		super ();
		if (getClass () == DatepickerDialog.class)
			mono.android.TypeManager.Activate ("WardApp.Dialogs.DatepickerDialog, WardApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public DatepickerDialog (java.lang.String p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == DatepickerDialog.class)
			mono.android.TypeManager.Activate ("WardApp.Dialogs.DatepickerDialog, WardApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "System.String, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0 });
	}


	public void onStart ()
	{
		n_onStart ();
	}

	private native void n_onStart ();


	public android.view.View onCreateView (android.view.LayoutInflater p0, android.view.ViewGroup p1, android.os.Bundle p2)
	{
		return n_onCreateView (p0, p1, p2);
	}

	private native android.view.View n_onCreateView (android.view.LayoutInflater p0, android.view.ViewGroup p1, android.os.Bundle p2);

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
