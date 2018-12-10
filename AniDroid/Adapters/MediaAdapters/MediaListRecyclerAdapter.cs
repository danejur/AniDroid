using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using AniDroid.Dialogs;

namespace AniDroid.Adapters.MediaAdapters
{
    public class MediaListRecyclerAdapter : AniDroidRecyclerAdapter<MediaListViewModel, Media.MediaList>
    {
        private readonly bool _isCustomList;
        private readonly string _listName;
        private readonly Media.MediaListStatus _listStatus;
        private readonly bool _highlightPriorityItems;
        private readonly bool _displayProgressColors;
        private readonly bool _useLongClickForEpisodeAdd;

        private readonly ColorStateList _priorityBackgroundColor;
        private readonly ColorStateList _upToDateTitleColor;
        private readonly ColorStateList _behindTitleColor;

        private readonly List<MediaListViewModel> _unfilteredItems;

        private IList<Media.MediaFormat> _filteredFormats = new List<Media.MediaFormat>();
        private IList<Media.MediaStatus> _filteredStatuses = new List<Media.MediaStatus>();

        public MediaListRecyclerAdapter(BaseAniDroidActivity context, Media.MediaListGroup mediaListGroup,
            RecyclerCardType cardType, Func<Media.MediaList, MediaListViewModel> createViewModelFunc, bool highlightPriorityItems, bool displayProgressColors, bool useLongClickForEpisodeAdd, Action<MediaListViewModel, Action> episodeAddAction = null) : base(context,
            mediaListGroup.Entries.Select(createViewModelFunc).ToList(), cardType)
        {
            CreateViewModelFunc = createViewModelFunc;
            _isCustomList = mediaListGroup.IsCustomList;
            _listName = mediaListGroup.Name;
            _listStatus = mediaListGroup.Status;
            _unfilteredItems = Items;
            _highlightPriorityItems = highlightPriorityItems;
            _displayProgressColors = displayProgressColors;
            _useLongClickForEpisodeAdd = useLongClickForEpisodeAdd;

            _priorityBackgroundColor =
                ColorStateList.ValueOf(new Color(Context.GetThemedColor(Resource.Attribute.ListItem_Priority)));
            _upToDateTitleColor =
                ColorStateList.ValueOf(new Color(Context.GetThemedColor(Resource.Attribute.ListItem_UpToDate)));
            _behindTitleColor =
                ColorStateList.ValueOf(new Color(Context.GetThemedColor(Resource.Attribute.ListItem_Behind)));

            ClickAction = viewModel =>
                MediaActivity.StartActivity(Context, viewModel.Model.Media?.Id ?? 0, BaseAniDroidActivity.ObjectBrowseRequestCode);

            // leave this as the non-edit action so we can leave the presenter out
            LongClickAction = viewModel =>
                Context.DisplaySnackbarMessage(viewModel.Model.Media?.Title?.UserPreferred, Snackbar.LengthLong);

            if (episodeAddAction != null)
            {
                if (_useLongClickForEpisodeAdd)
                {
                    ButtonLongClickAction = (viewModel, pos, callback) =>
                        episodeAddAction.Invoke(viewModel as MediaListViewModel, callback);
                }
                else
                {
                    ButtonClickAction = (viewModel, pos, callback) =>
                        episodeAddAction.Invoke(viewModel as MediaListViewModel, callback);
                }
            }
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var viewModel = Items[position];

            holder.ContainerCard.CardBackgroundColor = _highlightPriorityItems && viewModel.IsPriority ? _priorityBackgroundColor : holder.DefaultBackgroundColor;

            if (_displayProgressColors && viewModel.DisplayEpisodeProgressColor)
            {
                holder.Name.SetTextColor(viewModel.IsBehind ? _behindTitleColor : _upToDateTitleColor);
            }
            else
            {
                holder.Name.SetTextColor(holder.DefaultNameColor);
            }
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
            _filteredFormats = filteredFormats;
            _filteredStatuses = filteredStatuses;

            var items = _unfilteredItems.AsEnumerable();

            if (_filteredFormats?.Any() == true)
            {
                items = items.Where(x => x.Model.Media?.Format?.EqualsAny(_filteredFormats) == true);
            }

            if (_filteredStatuses?.Any() == true)
            {
                items = items.Where(x => x.Model.Media?.Status?.EqualsAny(_filteredStatuses) == true);
            }

            Items = items.ToList();

            NotifyDataSetChanged();
        }
    }
}