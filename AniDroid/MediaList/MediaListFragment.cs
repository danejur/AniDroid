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
using AniDroid.AniList;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Utils;
using Ninject;

namespace AniDroid.MediaList
{
    public class MediaListFragment : BaseAniDroidFragment<MediaListPresenter>, IMediaListView
    {
        private const string MediaTypeKey = "MEDIA_TYPE";

        private Media.MediaType _type;
        private View _view;

        public override string FragmentName => $"{_type?.Value ?? "MEDIA"}_LIST_FRAGMENT";

        protected override IReadOnlyKernel Kernel =>
            new StandardKernel(new ApplicationModule<IMediaListView, MediaListFragment>(this));

        public static MediaListFragment CreateMediaListFragment(Media.MediaType type)
        {
            var frag = new MediaListFragment();
            var bundle = new Bundle(6);
            bundle.PutString(MediaTypeKey, type.Value);
            frag.Arguments = bundle;
            return frag;

        }

        public override void OnError(IAniListError error)
        {
            // TODO: show error fragment here
        }

        public override View CreateView(ViewGroup container, Bundle savedInstanceState)
        {
            _type = AniListEnum.GetEnum<Media.MediaType>(Arguments.GetString(MediaTypeKey));

            CreatePresenter(savedInstanceState).GetAwaiter().GetResult();

            Presenter.GetMediaLists();

            return _view = LayoutInflater.Inflate(Resource.Layout.Fragment_NotImplemented, container, false);
        }

        public Media.MediaType GetMediaType()
        {
            return _type;
        }

        public void DisplayMediaLists(Media.MediaListCollection collection)
        {
            // TODO: display media lists here
        }
    }
}