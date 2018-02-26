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

namespace AniDroid.Base
{
    public abstract class BaseAniDroidFragment : Android.Support.V4.App.Fragment
    {
        protected BaseAniDroidActivity BaseActivity => Activity as BaseAniDroidActivity;
        protected new LayoutInflater LayoutInflater => BaseActivity.LayoutInflater;
    }
}