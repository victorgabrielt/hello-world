package md592452012d61c676221a90252fbb6dcb5;


public class LoginActivity
	extends md592452012d61c676221a90252fbb6dcb5.BaseActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_btnLogin_Click:()V:__export__\n" +
			"n_btnSair_Click:()V:__export__\n" +
			"";
		mono.android.Runtime.register ("WardApp.LoginActivity, WardApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", LoginActivity.class, __md_methods);
	}


	public LoginActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == LoginActivity.class)
			mono.android.TypeManager.Activate ("WardApp.LoginActivity, WardApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void btnLogin_Click ()
	{
		n_btnLogin_Click ();
	}

	private native void n_btnLogin_Click ();


	public void btnSair_Click ()
	{
		n_btnSair_Click ();
	}

	private native void n_btnSair_Click ();

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
