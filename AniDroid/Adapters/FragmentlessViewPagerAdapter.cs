using System.Collections.Generic;
using Android.Support.V4.View;
using Android.Views;
using Java.Lang;

namespace AniDroid.Adapters
{
    public class FragmentlessViewPagerAdapter : PagerAdapter
    {
        private readonly List<KeyValuePair<string, View>> _viewList;

        public FragmentlessViewPagerAdapter()
        {
            _viewList = new List<KeyValuePair<string, View>>();
        }

        public void AddView(View view, string title, int position = -1)
        {
            if (position >= _viewList.Count || position < 0)
            {
                _viewList.Add(new KeyValuePair<string, View>(title, view));
            }
            else
            {
                _viewList.Insert(position, new KeyValuePair<string, View>(title, view));
            }
        }

        public override Object InstantiateItem(ViewGroup container, int position)
        {
            var view = _viewList[position];
            container.AddView(view.Value);
            return view.Value;
        }

        public override void DestroyItem(ViewGroup container, int position, Object @object)
        {
            container.RemoveView((View)@object);
        }

        public override bool IsViewFromObject(View view, Object @object)
        {
            return view == @object;
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new String(_viewList[position].Key);
        }

        public override int Count => _viewList?.Count ?? 0;
    }
}