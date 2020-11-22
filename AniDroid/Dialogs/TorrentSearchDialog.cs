using System;
using System.Linq;
using Android.Content;
using Android.Widget;
using AndroidX.AppCompat.App;
using AniDroid.Base;
using AniDroid.Torrent.NyaaSi;

namespace AniDroid.Dialogs
{
    public class TorrentSearchDialog
    {
        public static void Create(BaseAniDroidActivity context, Action<NyaaSiSearchRequest> searchAction, NyaaSiSearchRequest previousSearchRequest = null)
        {
            var dialogue = context.LayoutInflater.Inflate(Resource.Layout.Dialog_TorrentSearch, null);
            var searchCategorySpinner = dialogue.FindViewById<Spinner>(Resource.Id.TorrentSearch_CategorySpinner);
            var searchFilterSpinner = dialogue.FindViewById<Spinner>(Resource.Id.TorrentSearch_FilterSpinner);
            var searchTerm = dialogue.FindViewById<EditText>(Resource.Id.TorrentSearch_SearchText);
            searchCategorySpinner.Adapter = new ArrayAdapter<string>(context, Resource.Layout.View_SpinnerDropDownItem, NyaaSiConstants.TorrentCategoryTuples.Select(x => x.Value).ToList());
            searchFilterSpinner.Adapter = new ArrayAdapter<string>(context, Resource.Layout.View_SpinnerDropDownItem, NyaaSiConstants.TorrentFilterTuples.Select(x => x.Value).ToList());

            if (previousSearchRequest != null)
            {
                searchTerm.Text = previousSearchRequest.SearchTerm;
                searchCategorySpinner.SetSelection(
                    NyaaSiConstants.TorrentCategoryTuples.FindIndex(x => x.Key == previousSearchRequest.Category));
                searchFilterSpinner.SetSelection(
                    NyaaSiConstants.TorrentFilterTuples.FindIndex(x => x.Key == previousSearchRequest.Filter));
            }

            var a = new AlertDialog.Builder(context, context.GetThemedResourceId(Resource.Attribute.Dialog_Theme)).Create();
            a.SetView(dialogue);
            a.SetTitle("Search Torrents");
            a.SetButton((int)DialogButtonType.Positive, "Search", async (aS, ev) =>
            {
                var category = NyaaSiConstants.TorrentCategoryTuples[searchCategorySpinner.SelectedItemPosition].Key;
                var filter = NyaaSiConstants.TorrentFilterTuples[searchFilterSpinner.SelectedItemPosition].Key;
                var term = searchTerm.Text;

                var request = new NyaaSiSearchRequest { Category = category, Filter = filter, SearchTerm = term };
                searchAction?.Invoke(request);
            });
            a.SetButton((int)DialogButtonType.Neutral, "Cancel", (aS, eV) =>
            {
                a.Dismiss();
            });
            a.Show();
        }
    }
}