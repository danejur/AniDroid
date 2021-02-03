using System;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Base;

namespace AniDroid.Adapters.General
{
    public class CheckBoxItemRecyclerAdapter : BaseRecyclerAdapter<CheckBoxItemRecyclerAdapter.CheckBoxItem>
    {
        public bool ToggleDescription { get; set; }

        public CheckBoxItemRecyclerAdapter(BaseAniDroidActivity context, List<CheckBoxItem> items) : base(context,
            items, RecyclerCardType.Custom)
        {
        }

        public override RecyclerView.ViewHolder CreateCustomViewHolder(ViewGroup parent, int viewType)
        {
            return new CheckBoxItemViewHolder(Context.LayoutInflater.Inflate(Resource.Layout.View_SettingItem_Checkbox, parent, false));
        }

        public override void BindCustomViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (!(holder is CheckBoxItemViewHolder checkBoxHolder))
            {
                return;
            }

            var item = Items[position];

            checkBoxHolder.CheckBox.Checked = item.IsChecked;
            checkBoxHolder.TitleView.Text = item.Title ?? "";
            checkBoxHolder.DescriptionView.Text = item.Description ?? "";

            checkBoxHolder.TitleView.Visibility =
                string.IsNullOrWhiteSpace(item.Title) ? ViewStates.Gone : ViewStates.Visible;
            checkBoxHolder.DescriptionView.Visibility =
                string.IsNullOrWhiteSpace(item.Description) || ToggleDescription ? ViewStates.Gone : ViewStates.Visible;

            checkBoxHolder.CheckBox.SetTag(Resource.Id.Object_Position, position);
            checkBoxHolder.CheckBox.CheckedChange -= CheckBox_CheckedChange;
            checkBoxHolder.CheckBox.CheckedChange += CheckBox_CheckedChange;

            if (ToggleDescription)
            {
                checkBoxHolder.Container.Click -= Container_Click;
                checkBoxHolder.Container.Click += Container_Click;
            }
        }

        private void Container_Click(object sender, EventArgs e)
        {
            var view = sender as View;
            var details = view?.FindViewById(Resource.Id.SettingItem_Details);

            if (details != null)
            {
                details.Visibility = details.Visibility == ViewStates.Visible ? ViewStates.Gone : ViewStates.Visible;
            }
        }

        private void CheckBox_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var position = (int)(sender as View)?.GetTag(Resource.Id.Object_Position);
            var item = Items[position];

            item.IsChecked = e.IsChecked;
        }

        public class CheckBoxItem
        {
            public int Id { get; set; }
            public bool IsChecked { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
        }

        protected class CheckBoxItemViewHolder : RecyclerView.ViewHolder
        {
            public CheckBox CheckBox { get; set; }
            public TextView TitleView { get; set; }
            public TextView DescriptionView { get; set; }
            public View Container { get; set; }

            public CheckBoxItemViewHolder(View itemView) : base(itemView)
            {
                CheckBox = itemView.FindViewById<CheckBox>(Resource.Id.SettingItem_Checkbox);
                TitleView = itemView.FindViewById<TextView>(Resource.Id.SettingItem_Name);
                DescriptionView = itemView.FindViewById<TextView>(Resource.Id.SettingItem_Details);
                Container = itemView.FindViewById(Resource.Id.SettingItem_Container);
            }
        }
    }
}