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

namespace Core.Droid
{
    public class ApplicationBase : Android.App.Application
    {
        public Context CurrentContext { get; set; }

        public ApplicationBase(IntPtr handle, JniHandleOwnership ownerShip)
            : base(handle, ownerShip)
        {
        }
    }
}