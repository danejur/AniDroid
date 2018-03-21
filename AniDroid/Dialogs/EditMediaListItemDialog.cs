using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AniDroid.Base;
using AniDroid.Widgets;

namespace AniDroid.Dialogs
{
    public static class EditMediaListItemDialog
    {
        public static void Create(BaseAniDroidActivity context)
        {
            var transaction = context.SupportFragmentManager.BeginTransaction();
            transaction.SetTransition((int)FragmentTransit.FragmentOpen);
            transaction.Add(Android.Resource.Id.Content, new EditMediaListItemDialogFragment()).AddToBackStack(null).Commit();
        }

        public class EditMediaListItemDialogFragment : AppCompatDialogFragment
        {
            private new BaseAniDroidActivity Activity => base.Activity as BaseAniDroidActivity;

            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                var view = Activity.LayoutInflater.Inflate(Resource.Layout.Fragment_EditMediaListItem, container, false);

                var list = Enumerable.Range(1, 100).Select(x => new KeyValuePair<string, string>($"{x}", $"{x}")).ToList();

                var scorePicker = view.FindViewById<Picker>(Resource.Id.EditMediaListItem_ScorePicker);
                scorePicker.SetItems(Activity, list);

                return view;
            }
        }
    }
}