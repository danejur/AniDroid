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
using AniDroid.AniList.Dto;
using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using AniDroid.Dialogs;
using AniDroid.MediaList;
using AniDroid.Utils.Extensions;
using OneOf;

namespace AniDroid.Adapters.MediaAdapters
{
    public class MediaListRecyclerAdapter : AniDroidRecyclerAdapter<MediaListViewModel, AniList.Models.MediaModels.MediaList>
    {
        private readonly bool _isCustomList;
        private readonly string _listName;
        private readonly MediaListStatus _listStatus;
        private readonly bool _highlightPriorityItems;
        private readonly MediaListItemViewType _viewType;

        private readonly ColorStateList _priorityBackgroundColor;
        private readonly ColorStateList _upToDateTitleColor;
        private readonly ColorStateList _slightlyBehindTitleColor;
        private readonly ColorStateList _behindTitleColor;
        private readonly ColorStateList _veryBehindTitleColor;

        private readonly List<MediaListViewModel> _unfilteredItems;

        private IList<MediaFormat> _filteredFormats = new List<MediaFormat>();
        private IList<MediaStatus> _filteredStatuses = new List<MediaStatus>();

        public enum MediaListItemViewType
        {
            Normal = 0,
            Compact = 1,
            TitleOnly = 2
        }

        public enum MediaListProgressDisplayType
        {
            Never,
            Releasing,
            ReleasingExtended,
            Always
        }

        public MediaListRecyclerAdapter(BaseAniDroidActivity context, MediaListGroup mediaListGroup,
            RecyclerCardType cardType, Func<AniList.Models.MediaModels.MediaList, MediaListViewModel> createViewModelFunc, MediaListItemViewType viewType, bool highlightPriorityItems, bool useLongClickForEpisodeAdd, Action<MediaListViewModel, Action> episodeAddAction = null) : base(context,
            mediaListGroup.Entries.Select(createViewModelFunc).ToList(), cardType)
        {
            CreateViewModelFunc = createViewModelFunc;
            _isCustomList = mediaListGroup.IsCustomList;
            _listName = mediaListGroup.Name;
            _listStatus = mediaListGroup.Status;
            _unfilteredItems = Items;
            _viewType = viewType;
            _highlightPriorityItems = highlightPriorityItems;

            _priorityBackgroundColor =
                ColorStateList.ValueOf(new Color(Context.GetThemedColor(Resource.Attribute.ListItem_Priority)));
            _upToDateTitleColor =
                ColorStateList.ValueOf(new Color(Context.GetThemedColor(Resource.Attribute.ListItem_UpToDate)));
            _slightlyBehindTitleColor =
                ColorStateList.ValueOf(new Color(Context.GetThemedColor(Resource.Attribute.ListItem_SlightlyBehind)));
            _behindTitleColor =
                ColorStateList.ValueOf(new Color(Context.GetThemedColor(Resource.Attribute.ListItem_Behind)));
            _veryBehindTitleColor =
                ColorStateList.ValueOf(new Color(Context.GetThemedColor(Resource.Attribute.ListItem_VeryBehind)));

            ClickAction = (viewModel, position) =>
                MediaActivity.StartActivity(Context, viewModel.Model.Media?.Id ?? 0, BaseAniDroidActivity.ObjectBrowseRequestCode);

            // leave this as the non-edit action so we can leave the presenter out
            LongClickAction = (viewModel, position) =>
                Context.DisplaySnackbarMessage(viewModel.Model.Media?.Title?.UserPreferred, Snackbar.LengthLong);

            if (episodeAddAction != null)
            {
                if (useLongClickForEpisodeAdd)
                {
                    ButtonLongClickAction = (viewModel, pos, callback) =>
                        episodeAddAction.Invoke(viewModel as MediaListViewModel, callback);
                    ButtonClickAction = (viewModel, pos, callback) => Toast.MakeText(Context.ApplicationContext,
                        Context.GetString(Resource.String.MediaList_LongPressEnabledAlert), ToastLength.Short).Show();
                }
                else
                {
                    ButtonClickAction = (viewModel, pos, callback) =>
                        episodeAddAction.Invoke(viewModel as MediaListViewModel, callback);
                }
            }

            if (_viewType != MediaListItemViewType.Normal)
            {
                CardType = RecyclerCardType.Custom;
                CustomCardUseItemDecoration = true;
            }
        }

        public MediaListRecyclerAdapter(BaseAniDroidActivity context, RecyclerCardType cardType,
            IAsyncEnumerable<OneOf<IPagedData<AniList.Models.MediaModels.MediaList>, IAniListError>> enumerable,
            Func<AniList.Models.MediaModels.MediaList, MediaListViewModel> createViewModelFunc) : base(context, enumerable, cardType, createViewModelFunc)
        {


        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var viewModel = Items[position];

            holder.ContainerCard.CardBackgroundColor = _highlightPriorityItems && viewModel.IsPriority ? _priorityBackgroundColor : holder.DefaultBackgroundColor;

            holder.Image.SetBackgroundColor(viewModel.ImageColor);

            if (viewModel.WatchingStatus == MediaListViewModel.MediaListWatchingStatus.None)
            {
                holder.Name.SetTextColor(holder.DefaultNameColor);
            }
            else
            {
                holder.Name.SetTextColor(GetEpisodeStatusColor(viewModel.WatchingStatus, holder.DefaultNameColor));
            }
        }

        public void UpdateMediaListItem(int mediaId, AniList.Models.MediaModels.MediaList updatedMediaList)
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

        public void SetFilter(MediaListFilterModel filterModel)
        {
            if (_unfilteredItems?.Any() != true)
            {
                return;
            }

            if (filterModel?.IsFilteringActive != true)
            {
                Items = _unfilteredItems;
            }
            else
            {
                var items = _unfilteredItems.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(filterModel.Title))
                {
                    var loweredTitle = filterModel.Title.ToLowerInvariant();

                    items = items.Where(x =>
                        x.Model.Media?.Title?.English?.ToLowerInvariant().Contains(loweredTitle) == true ||
                        x.Model.Media?.Title?.Romaji?.ToLowerInvariant().Contains(loweredTitle) == true ||
                        x.Model.Media?.Title?.Native?.ToLowerInvariant().Contains(loweredTitle) == true);
                }
                if (filterModel.Format != null)
                {
                    items = items.Where(x => x.Model.Media?.Format == filterModel.Format);
                }
                if (filterModel.Status != null)
                {
                    items = items.Where(x => x.Model.Media?.Status == filterModel.Status);
                }
                if (filterModel.Season != null)
                {
                    items = items.Where(x => x.Model.Media?.Season == filterModel.Season);
                }
                if (filterModel.Year != null)
                {
                    items = items.Where(x => x.Model.Media?.SeasonYear == filterModel.Year || x.Model?.Media?.StartDate?.Year == filterModel.Year);
                }
                if (filterModel.Source != null)
                {
                    items = items.Where(x => x.Model.Media?.Source == filterModel.Source);
                }
                if (filterModel.LicensedBy?.Any() == true)
                {
                    items = items.Where(x => x.Model.Media?.ExternalLinks?.Select(y => y.Site).ContainsAny(filterModel.LicensedBy) == true);
                }
                if (filterModel.IncludedGenres?.Any() == true)
                {
                    items = items.Where(x => x.Model.Media?.Genres.ContainsAny(filterModel.IncludedGenres) == true);
                }
                if (filterModel.IncludedTags?.Any() == true)
                {
                    items = items.Where(x => x.Model.Media?.Tags?.Select(x => x.Name).ContainsAny(filterModel.IncludedTags) == true);
                }

                Items = items.ToList();
            }

            NotifyDataSetChanged();
        }

        public override RecyclerView.ViewHolder CreateCustomViewHolder(ViewGroup parent, int viewType)
        {
            if (_viewType == MediaListItemViewType.Compact)
            {
                var card = new CardItem(Context.LayoutInflater.Inflate(
                    Resource.Layout.View_CardItem_FlatHorizontalCompact,
                    parent, false));

                SetupButtonClickActions(card);
                SetupRowClickActions(card);

                return SetupCardItemViewHolder(card);
            }
            if (_viewType == MediaListItemViewType.TitleOnly)
            {
                var card = new CardItem(Context.LayoutInflater.Inflate(
                    Resource.Layout.View_CardItem_FlatHorizontalTitleOnly,
                    parent, false));

                SetupRowClickActions(card);

                return SetupCardItemViewHolder(card);
            }

            return base.CreateCustomViewHolder(parent, viewType);
        }

        public override void BindCustomViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewModel = Items[position];

            if (!(holder is CardItem cardHolder))
            {
                return;
            }

            if (_viewType == MediaListItemViewType.TitleOnly)
            {
                cardHolder.Name.Visibility = viewModel.TitleVisibility;
                cardHolder.Button.Visibility = ViewStates.Gone;

                cardHolder.Name.Text = viewModel.TitleText ?? "";

                cardHolder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
                cardHolder.Button.SetTag(Resource.Id.Object_Position, position);
            }
            else
            {
                BindViewHolderByType(cardHolder, position, RecyclerCardType.FlatHorizontal);
            }
        }

        private ColorStateList GetEpisodeStatusColor(MediaListViewModel.MediaListWatchingStatus status, ColorStateList defaultColor)
        {
            var retColor = defaultColor;

            switch (status)
            {
                case MediaListViewModel.MediaListWatchingStatus.UpToDate:
                    retColor = _upToDateTitleColor;
                    break;
                case MediaListViewModel.MediaListWatchingStatus.SlightlyBehind:
                    retColor = _slightlyBehindTitleColor;
                    break;
                case MediaListViewModel.MediaListWatchingStatus.Behind:
                    retColor = _behindTitleColor;
                    break;
                case MediaListViewModel.MediaListWatchingStatus.VeryBehind:
                    retColor = _veryBehindTitleColor;
                    break;
            }

            return retColor;
        }
    }
}