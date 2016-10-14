package md50b3136709769d771756044d73e28b613;


public abstract class ActivityBase_1
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("DemoNavigation.ActivityBase`1, DemoNavigation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ActivityBase_1.class, __md_methods);
	}


	public ActivityBase_1 () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ActivityBase_1.class)
			mono.android.TypeManager.Activate ("DemoNavigation.ActivityBase`1, DemoNavigation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

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
