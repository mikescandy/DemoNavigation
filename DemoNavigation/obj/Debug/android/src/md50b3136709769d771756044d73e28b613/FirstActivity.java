package md50b3136709769d771756044d73e28b613;


public class FirstActivity
	extends md50b3136709769d771756044d73e28b613.ActivityBase_1
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("DemoNavigation.FirstActivity, DemoNavigation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", FirstActivity.class, __md_methods);
	}


	public FirstActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == FirstActivity.class)
			mono.android.TypeManager.Activate ("DemoNavigation.FirstActivity, DemoNavigation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
