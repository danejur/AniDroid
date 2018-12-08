using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Media;
using AniDroid.Base;

namespace AniDroid.Adapters.MediaAdapters
{
    public class MediaListRecyclerAdapter : AniDroidRecyclerAdapter<MediaListViewModel, Media.MediaList>
    {
        private readonly bool _isCustomList;
        private readonly string _listName;
        private readonly Media.MediaListStatus _listStatus;

        public MediaListRecyclerAdapter(BaseAniDroidActivity context, Media.MediaListGroup mediaListGroup,
            RecyclerCardType cardType, Func<Media.MediaList, MediaListViewModel> createViewModelFunc) : base(context,
            mediaListGroup.Entries.Select(createViewModelFunc).ToList(), cardType)
        {
            CreateViewModelFunc = createViewModelFunc;
            _isCustomList = mediaListGroup.IsCustomList;
            _listName = mediaListGroup.Name;
            _listStatus = mediaListGroup.Status;

            ClickAction = viewModel =>
                MediaActivity.StartActivity(Context, viewModel.Model.Media?.Id ?? 0, BaseAniDroidActivity.ObjectBrowseRequestCode);

            // TODO: should this really be the default long click action for this list type?
            LongClickAction = viewModel =>
                Context.DisplaySnackbarMessage(viewModel.Model.Media?.Title?.UserPreferred, Snackbar.LengthLong);
        }

        public void UpdateMediaListItem(int mediaId, Media.MediaList updatedMediaList)
        {
            var position = Items.FindIndex(x => x.Model.Media?.Id == mediaId);

            if (position >= 0)
            {
                if (updatedMediaList.HiddenFromStatusLists && !_isCustomList ||
                    _isCustomList && !updatedMediaList.CustomLists.Any(x => x?.Enabled == true && x.Name == _listName) ||
                    !_isCustomList && _listStatus != updatedMediaList.Status)
                {
                    Items.RemoveAt(position);
                    NotifyDataSetChanged();
                }
                else
                {
                    // we take the media object that already exists on the list and reuse it because it has
                    // a lot more stuff already in it from the original request
                    updatedMediaList.Media = Items[position].Model?.Media;
                    Items[position] = CreateViewModelFunc?.Invoke(updatedMediaList);
                    NotifyItemChanged(position);
                }
            }
            else if (_isCustomList && updatedMediaList.CustomLists.Any(x => x?.Enabled == true && x.Name == _listName) ||
                     !_isCustomList && _listStatus == updatedMediaList.Status)
            {
                Items.Insert(0, CreateViewModelFunc?.Invoke(updatedMediaList));
                NotifyDataSetChanged();
            }
        }

        public void ResetMediaListItem(int mediaId)
        {
            var position = Items.FindIndex(x => x.Model?.Media.Id == mediaId);

            if (position >= 0)
            {
                NotifyItemChanged(position);
            }
        }

        public void RemoveMediaListItem(int mediaListId)
        {
            Items.RemoveAll(x => x.Model?.Id == mediaListId);
            NotifyDataSetChanged();
        }

        public void UpdateFilters(IList<Media.MediaFormat> filteredFormats, IList<Media.MediaStatus> filteredStatuses)
        {
            //_filteredFormats = filteredFormats;
            //_filteredStatuses = filteredStatuses;

            //var items = _unfilteredItems.AsEnumerable();

            //if (_filteredFormats?.Any() == true)
            //{
            //    items = items.Where(x => x.Media?.Format?.EqualsAny(_filteredFormats) == true);
            //}

            //if (_filteredStatuses?.Any() == true)
            //{
            //    items = items.Where(x => x.Media?.Status?.EqualsAny(_filteredStatuses) == true);
            //}

            //Items = items.ToList();

            //NotifyDataSetChanged();
        }
    }
}