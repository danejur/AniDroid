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

namespace AniDroid.Dialogs
{
    public static class BrowseFilterDialog
    {
        public static void Create(BaseAniDroidActivity context)
        {
            var dialog =
                new BrowseFilterDialogFragment
                {
                    Cancelable = true
                };
            var transaction = context.SupportFragmentManager.BeginTransaction();
            transaction.SetTransition((int) FragmentTransit.FragmentOpen);
            transaction.Add(Android.Resource.Id.Content, dialog)
                .AddToBackStack(BrowseFilterDialogFragment.BackstackTag).Commit();
        }

        public class BrowseFilterDialogFragment : AppCompatDialogFragment
        {
            public const string BackstackTag = "BROWSE_FILTER_DIALOG";

            private const int DefaultMaxPickerValue = 9999;

            //private readonly IAniListMediaListEditPresenter _presenter;
            //private readonly Media _media;
            //private readonly Media.MediaList _mediaList;
            //private readonly User.UserMediaListOptions _mediaListOptions;
            //private readonly List<string> _mediaStatusList;
            //private readonly HashSet<string> _customLists;
            //private readonly bool _completeMedia;
            //private bool _customScoringEnabled;
            //private new BaseAniDroidActivity Activity => base.Activity as BaseAniDroidActivity;
            //private bool _isPrivate;
            //private bool _hideFromStatusLists;
            //private int _priority;
            private bool _pendingDismiss;
            //private List<float?> _advancedScores;

            //private CoordinatorLayout _coordLayout;
            //private Picker _scorePicker;
            //private AppCompatSpinner _statusSpinner;
            //private Picker _progressPicker;
            //private Picker _progressVolumesPicker;
            //private Picker _repeatPicker;
            //private EditText _notesView;
            //private DatePickerTextView _startDateView;
            //private DatePickerTextView _finishDateView;

            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                var view = Activity.LayoutInflater.Inflate(Resource.Layout.Fragment_BrowseFilterDialog, container,
                    false);

                return view;
            }

            public override void OnResume()
            {
                base.OnResume();

                if (_pendingDismiss)
                {
                    Activity.SupportFragmentManager.PopBackStack(BackstackTag, (int) PopBackStackFlags.Inclusive);
                    DismissAllowingStateLoss();
                }
            }
        }
    }
}