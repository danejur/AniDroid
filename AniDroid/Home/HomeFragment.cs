using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.AniListActivityAdapters;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Dialogs;
using AniDroid.Utils;
using Ninject;
using OneOf;

namespace AniDroid.Home
{
    public class HomeFragment : BaseAniDroidFragment<HomePresenter>, IHomeView
    {
        private AniListActivityRecyclerAdapter _recyclerAdapter;

        public override bool HasMenu => true;
        public override string FragmentName => "HOME_FRAGMENT";
        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule<IHomeView, HomeFragment>(this));

        public override View CreateView(ViewGroup container, Bundle savedInstanceState)
        {
            CreatePresenter(savedInstanceState).GetAwaiter().GetResult();
            return LayoutInflater.Inflate(Resource.Layout.View_List, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            Presenter.GetAniListActivity(true);
        }

        public override void SetupMenu(IMenu menu)
        {
            menu.Clear();
            var inflater = new MenuInflater(Context);
            inflater.Inflate(Resource.Menu.Home_ActionBar, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.Menu_Home_Refresh:
                    RefreshActivity();
                    return true;
                case Resource.Id.Menu_Home_PostStatus:
                    AniListActivityCreateDialog.Create(Activity, Presenter.PostStatusActivity);
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnError(IAniListError error)
        {
            throw new NotImplementedException();
        }

        public void ShowUserActivity(IAsyncEnumerable<OneOf<IPagedData<AniListActivity>, IAniListError>> activityEnumerable, int userId)
        {
            var recycler = View.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            recycler.SetAdapter(_recyclerAdapter = new AniListActivityRecyclerAdapter(Activity, Presenter, activityEnumerable, userId));
        }

        public void ShowAllActivity(IAsyncEnumerable<OneOf<IPagedData<AniListActivity>, IAniListError>> activityEnumerable, int userIUd)
        {
            throw new NotImplementedException();
        }

        public void UpdateActivity(int activityPosition, AniListActivity activity)
        {
            _recyclerAdapter.Items[activityPosition] = activity;
            _recyclerAdapter.NotifyItemChanged(activityPosition);
        }

        public void RefreshActivity()
        {
            _recyclerAdapter?.ResetAdapter();
        }
    }
}