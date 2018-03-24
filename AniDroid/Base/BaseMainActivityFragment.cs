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
    public abstract class BaseMainActivityFragment<T> : BaseAniDroidFragment<T> where T : BaseAniDroidPresenter
    {
        private View _storedView;
        private bool _recreatingFragment;
        private static BaseMainActivityFragment<T> _instance;

        public static BaseMainActivityFragment<T> GetInstance(string fragmentName) => _instance?.FragmentName == fragmentName ? _instance : null;

        public override void OnDetach()
        {
            base.OnDetach();
            _instance = this;
        }

        public new void RecreateFragment()
        {
            _recreatingFragment = true;
            base.RecreateFragment();
        }

        public abstract View CreateMainActivityFragmentView(ViewGroup container, Bundle savedInstanceState);

        public sealed override View CreateView(ViewGroup container, Bundle savedInstanceState)
        {
            return _storedView = _recreatingFragment ? CreateMainActivityFragmentView(container, savedInstanceState): _storedView ?? CreateMainActivityFragmentView(container, savedInstanceState);
        }
    }
}