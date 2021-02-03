using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using AniDroid.Adapters.Base;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Torrent.NyaaSi;
using OneOf;

namespace AniDroid.Adapters.TorrentAdapters
{
    public class NyaaSiSearchRecyclerAdapter : LazyLoadingRecyclerViewAdapter<NyaaSiSearchResult>
    {
        private ColorStateList DefaultBackgroundColor { get; set; }
        private ColorStateList TrustedBackgroundColor { get; set; }
        private ColorStateList RemakeBackgroundColor { get; set; }

        public NyaaSiSearchRecyclerAdapter(BaseAniDroidActivity context,
            IAsyncEnumerable<OneOf<IPagedData<NyaaSiSearchResult>, IAniListError>> enumerable) : base(context,
            enumerable, RecyclerCardType.Custom)
        {
            InitializeColors();
            CustomCardUseItemDecoration = true;
        }

        public NyaaSiSearchRecyclerAdapter(BaseAniDroidActivity context,
            LazyLoadingRecyclerViewAdapter<NyaaSiSearchResult> adapter) : base(context, adapter)
        {
            InitializeColors();
            CustomCardUseItemDecoration = true;
        }

        private void InitializeColors()
        {
            DefaultBackgroundColor = ColorStateList.ValueOf(new Color(Context.GetThemedColor(Resource.Attribute.Background)));
            TrustedBackgroundColor = ColorStateList.ValueOf(new Color(Context.GetThemedColor(Resource.Attribute.Background_TrustedTorrent)));
            RemakeBackgroundColor = ColorStateList.ValueOf(new Color(Context.GetThemedColor(Resource.Attribute.Background_RemakeTorrent)));
        }

        public override RecyclerView.ViewHolder CreateCustomViewHolder(ViewGroup parent, int viewType)
        {
            return SetupCardItemViewHolder(new CardItem(Context.LayoutInflater.Inflate(Resource.Layout.View_CardItem_TorrentSearchResult,
                parent, false)));
        }

        public override void BindCustomViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = Items[position];

            var cardHolder = holder as CardItem;

            cardHolder.Name.Text = item.Title;
            cardHolder.DetailPrimary.Text = $"Size: {item.Size}";
            cardHolder.DetailSecondary.Text = $"Published: {item.PublishDate.ToString()}";
            cardHolder.Image.SetImageResource(CategoryImages[item.Category]);

            switch (item.Description)
            {
                case "trusted":
                    cardHolder.ContainerCard.CardBackgroundColor = TrustedBackgroundColor;
                    break;
                case "remake":
                    cardHolder.ContainerCard.CardBackgroundColor = RemakeBackgroundColor;
                    break;
                default:
                    cardHolder.ContainerCard.CardBackgroundColor = DefaultBackgroundColor;
                    break;
            }

            cardHolder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
        }


        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.ContainerCard.Click -= ItemClick;
            item.ContainerCard.Click += ItemClick;

            return item;
        }

        private void ItemClick(object sender, EventArgs e)
        {
            var view = sender as View;
            var position = (int)view.GetTag(Resource.Id.Object_Position);
            var item = Items[position];

            var a = new AlertDialog.Builder(Context).Create();
            a.SetTitle("Open Torrent");
            a.SetMessage("Would you like to open this torrent (magnet)?");
            a.SetButton2("Yes", (aS, ev) => {
                var intent = new Intent(Intent.ActionView);
                intent.SetData(Android.Net.Uri.Parse(item.Link));
                intent.AddFlags(ActivityFlags.NewTask);
                Context.StartActivity(intent);
            });
            a.SetButton("Open in Browser", (aS, eV) => {
                var intent = new Intent(Intent.ActionView);
                intent.AddFlags(ActivityFlags.NewTask);
                intent.SetData(Android.Net.Uri.Parse(item.Guid));
                Context.StartActivity(intent);
            });
            a.Show();
        }

        private static readonly Dictionary<string, int> CategoryImages = new Dictionary<string, int>
        {
            {"Anime - Anime Music Video", Resource.Drawable.category_32},
            {"Anime - English-translated", Resource.Drawable.category_37},
            {"Anime - Non-English-translated", Resource.Drawable.category_38},
            {"Anime - Raw", Resource.Drawable.category_11},
            {"Audio - Lossless", Resource.Drawable.category_14},
            {"Audio - Lossy", Resource.Drawable.category_15},
            {"Literature - English-translated", Resource.Drawable.category_12},
            {"Literature - Non-English-translated", Resource.Drawable.category_39},
            {"Literature - Raw", Resource.Drawable.category_13},
            {"Live Action - English-translated", Resource.Drawable.category_19},
            {"Live Action - Idol/Promotional Video", Resource.Drawable.category_22},
            {"Live Action - Non-English-translated", Resource.Drawable.category_21},
            {"Live Action - Raw", Resource.Drawable.category_20},
            {"Pictures - Graphics", Resource.Drawable.category_18},
            {"Pictures - Photos", Resource.Drawable.category_17},
            {"Software - Applications", Resource.Drawable.category_23},
            {"Software - Games", Resource.Drawable.category_24}
        };
    }
}