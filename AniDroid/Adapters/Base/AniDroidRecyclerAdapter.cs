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
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.Base
{
    public abstract class AniDroidRecyclerAdapter<T, TModel> : BaseRecyclerAdapter<T> where T : AniDroidAdapterViewModel<TModel> where TModel : class
    {
        private readonly IAsyncEnumerable<OneOf<IPagedData<TModel>, IAniListError>> _asyncEnumerable;
        private IAsyncEnumerator<OneOf<IPagedData<TModel>, IAniListError>> _asyncEnumerator;
        private bool _isLazyLoading;
        private bool _dataLoaded;

        protected int LoadingCardWidth = ViewGroup.LayoutParams.MatchParent;
        protected int LoadingCardHeight = ViewGroup.LayoutParams.WrapContent;

        public Color? LoadingItemBackgroundColor { get; set; }
        public event EventHandler<bool> DataLoaded;
        public Func<TModel, T> CreateViewModelFunc { get; set; }

        public Action<AniDroidAdapterViewModel<TModel>> ClickAction { get; set; }
        public Action<AniDroidAdapterViewModel<TModel>> LongClickAction { get; set; }

        public Action<AniDroidAdapterViewModel<TModel>> ButtonClickAction { get; set; }
        public Action<AniDroidAdapterViewModel<TModel>> ButtonLongClickAction { get; set; }

        protected AniDroidRecyclerAdapter(BaseAniDroidActivity context,
            IAsyncEnumerable<OneOf<IPagedData<TModel>, IAniListError>> enumerable, RecyclerCardType cardType,
            Func<TModel, T> createViewModelFunc) : this(context, new List<T> {null}, cardType)
        {
            CreateViewModelFunc = createViewModelFunc;
            _asyncEnumerable = enumerable;
            _asyncEnumerator = enumerable.GetEnumerator();
        }

        protected AniDroidRecyclerAdapter(BaseAniDroidActivity context,
            List<T> items, RecyclerCardType cardType) : base(context, items, cardType)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            CardType = cardType;
            DefaultIconColor = ColorStateList.ValueOf(new Color(context.GetThemedColor(Resource.Attribute.Secondary_Dark)));
            FavoriteIconColor = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(context, Resource.Color.Favorite_Red)));
        }

        protected AniDroidRecyclerAdapter(BaseAniDroidActivity context,
            AniDroidRecyclerAdapter<T, TModel> adapter) : base(context, adapter.Items, adapter.CardType)
        {
            _asyncEnumerable = adapter._asyncEnumerable;
            _asyncEnumerator = adapter._asyncEnumerator;
        }

        public void ResetAdapter()
        {
            if (_asyncEnumerable != null)
            {
                _asyncEnumerator = _asyncEnumerable.GetEnumerator();
                Items.Clear();
                Items.Add(null);
            }

            NotifyDataSetChanged();
        }

        public override bool InsertItem(int position, T item, bool notify = true)
        {
            if (_asyncEnumerable != null && _isLazyLoading)
            {
                return true;
            }

            return base.InsertItem(position, item, notify);
        }

        public override bool ReplaceItem(int position, T item, bool notify = true)
        {
            if (_asyncEnumerable != null && _isLazyLoading)
            {
                return true;
            }

            return base.ReplaceItem(position, item, notify);
        }

        public void AddItems(IEnumerable<T> itemsToAdd, bool hasNextPage)
        {
            var initialAdd = Items.Count == 1 && Items[0] == null;

            if (hasNextPage)
            {
                itemsToAdd = itemsToAdd.Append(null);
            }

            Items.AddRange(itemsToAdd);

            NotifyDataSetChanged();

            if (initialAdd)
            {
                var controller = AnimationUtils.LoadLayoutAnimation(Context, Resource.Animation.Layout_Animation_Falldown);
                RecyclerView.LayoutAnimation = controller;
                RecyclerView.ScheduleLayoutAnimation();
            }
        }

        public void RefreshAdapter()
        {
            OnAttachedToRecyclerView(RecyclerView);
        }

        public sealed override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == ProgressBarViewType)
            {
                var view = Context.LayoutInflater.Inflate(Resource.Layout.View_IndeterminateProgressIndicator, parent,
                    false);
                view.LayoutParameters.Width = LoadingCardWidth;
                view.LayoutParameters.Height = LoadingCardHeight;
                var holder = new ProgressBarViewHolder(view, LoadingItemBackgroundColor);

                return holder;
            }

            if (CardType == RecyclerCardType.Custom)
            {
                return CreateCustomViewHolder(parent, viewType);
            }

            var layoutResource = Resource.Layout.View_CardItem_Horizontal;

            switch (CardType)
            {
                case RecyclerCardType.FlatHorizontal:
                    layoutResource = Resource.Layout.View_CardItem_FlatHorizontal;
                    break;
                case RecyclerCardType.Vertical:
                    layoutResource = Resource.Layout.View_CardItem_Vertical;
                    break;
            }

            var cardHolder = new CardItem(Context.LayoutInflater.Inflate(layoutResource, parent, false));

            if (ClickAction != null)
            {
                cardHolder.ContainerCard.Click -= RowClick;
                cardHolder.ContainerCard.Click += RowClick;
            }
            if (LongClickAction != null)
            {
                cardHolder.ContainerCard.LongClick -= RowLongClick;
                cardHolder.ContainerCard.LongClick += RowLongClick;
            }

            if (ButtonClickAction != null)
            {
                cardHolder.Button.Click -= ButtonClick;
                cardHolder.Button.Click += ButtonClick;
            }
            if (ButtonLongClickAction != null)
            {
                cardHolder.Button.LongClick -= ButtonLongClick;
                cardHolder.Button.LongClick += ButtonLongClick;
            }

            return SetupCardItemViewHolder(cardHolder);
        }

        public sealed override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (Items[position] != null)
            {
                OnBindViewHolderInternal(holder, position);
                return;
            }

            if (position < ItemCount - 1 || _isLazyLoading)
            {
                return;
            }

            _isLazyLoading = true;

            var moveNextResult = await _asyncEnumerator.MoveNextAsync();

            _asyncEnumerator.Current?.Switch((IAniListError error) =>
                    Context.DisplaySnackbarMessage("Error occurred while getting next page of data", Snackbar.LengthLong))
                .Switch(data =>
                {
                    if (!moveNextResult)
                    {
                        return;
                    }

                    if (!_dataLoaded)
                    {
                        DataLoaded?.Invoke(RecyclerView, data.PageInfo.Total > 0);
                        _dataLoaded = true;
                    }

                    AddItems(data.Data.Select(model => CreateViewModelFunc(model)), data.PageInfo.HasNextPage);
                });

            RemoveItem(position);

            _isLazyLoading = false;
        }

        private void OnBindViewHolderInternal(RecyclerView.ViewHolder holder, int position)
        {
            if (CardType == RecyclerCardType.Custom)
            {
                BindCustomViewHolder(holder, position);
            }
            else if (holder is CardItem cardHolder)
            {
                var viewModel = Items[position];

                cardHolder.Name.Visibility = viewModel.TitleVisibility;
                cardHolder.DetailPrimary.Visibility = viewModel.DetailPrimaryVisibility;
                cardHolder.DetailSecondary.Visibility = viewModel.DetailSecondaryVisibility;
                cardHolder.Image.Visibility = viewModel.ImageVisibility;
                cardHolder.Button.Visibility = viewModel.ButtonVisibility;

                cardHolder.Name.Text = viewModel.TitleText ?? "";
                cardHolder.DetailPrimary.Text = viewModel.DetailPrimaryText ?? "";
                cardHolder.DetailSecondary.Text = viewModel.DetailSecondaryText ?? "";
                Context.LoadImage(cardHolder.Image, viewModel.ImageUri ?? "");

                cardHolder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
                cardHolder.Button.SetTag(Resource.Id.Object_Position, position);

                BindCardViewHolder(cardHolder, position);
            }
        }

        private void RowClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var mediaPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var viewModel = Items[mediaPos];

            ClickAction?.Invoke(viewModel);
        }

        private void RowLongClick(object sender, View.LongClickEventArgs longClickEventArgs)
        {
            var senderView = sender as View;
            var mediaPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var viewModel = Items[mediaPos];

            LongClickAction?.Invoke(viewModel);
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var mediaPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var viewModel = Items[mediaPos];

            ButtonClickAction?.Invoke(viewModel);
        }

        private void ButtonLongClick(object sender, View.LongClickEventArgs longClickEventArgs)
        {
            var senderView = sender as View;
            var mediaPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var viewModel = Items[mediaPos];

            ButtonLongClickAction?.Invoke(viewModel);
        }

        public sealed override int GetItemViewType(int position)
        {
            return Items[position] == null ? ProgressBarViewType : 0;
        }

        private class ProgressBarViewHolder : RecyclerView.ViewHolder
        {
            public ProgressBarViewHolder(View itemView, Color? backgroundColor = null) : base(itemView)
            {
                if (backgroundColor.HasValue)
                {
                    itemView.SetBackgroundColor(backgroundColor.Value);
                }
            }
        }

        #region Constants

        private const int ProgressBarViewType = -1;

        #endregion
    }
}