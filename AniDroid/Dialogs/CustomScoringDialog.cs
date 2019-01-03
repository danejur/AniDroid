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
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Widgets;
using Newtonsoft.Json.Linq;

namespace AniDroid.Dialogs
{
    public static class CustomScoringDialog
    {
        public static void Create(BaseAniDroidActivity context, List<string> customScoringMethods,
            User.ScoreFormat scoreFormat, List<float?> scores, Action<List<float?>> onSaveAction)
        {
            var random = new Random();

            var view = context.LayoutInflater.Inflate(Resource.Layout.Dialog_CustomScoring, null);

            var container = view.FindViewById<LinearLayout>(Resource.Id.CustomScoring_Container);

            var groupedScoringMethods = customScoringMethods.Select((value, index) => new {value, index})
                .GroupBy(x => x.index / 2, x => new {Value = x.value, Id = random.Next()}).ToList();

            foreach (var group in groupedScoringMethods)
            {
                var innerContainer = new LinearLayout(context)
                {
                    LayoutParameters =
                        new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                            ViewGroup.LayoutParams.WrapContent),
                    Orientation = Orientation.Horizontal
                };

                foreach (var method in group)
                {
                    var scoreWidget =
                        context.LayoutInflater.Inflate(Resource.Layout.View_PickerWithLabel, innerContainer, false);

                    var label = scoreWidget.FindViewById<TextView>(Resource.Id.PickerWithLabel_Label);
                    var picker = scoreWidget.FindViewById<Picker>(Resource.Id.PickerWithLabel_Picker);

                    // assign new Ids so we don't have collisions
                    label.Text = method.Value;
                    label.Id = method.Id + 1;
                    picker.Id = method.Id;

                    var score = scores?.ElementAtOrDefault(customScoringMethods.IndexOf(method.Value)) ?? 0;

                    if (scoreFormat == User.ScoreFormat.Hundred)
                    {
                        picker.SetMaxValue(100, 0, false, score);
                    }
                    else if (scoreFormat == User.ScoreFormat.TenDecimal)
                    {
                        picker.SetMaxValue(10, 1, false, score);
                    }

                    innerContainer.AddView(scoreWidget);
                }

                container.AddView(innerContainer);
            }

            var alert = new Android.Support.V7.App.AlertDialog.Builder(context,
                context.GetThemedResourceId(Resource.Attribute.Dialog_Theme));
            alert.SetView(view);
            alert.SetPositiveButton("Save", (sender, e) =>
            {
                var savedScores = groupedScoringMethods.SelectMany(x => x).Select(x =>
                    view.FindViewById<Picker>(x.Id).GetValue()).ToList();
                onSaveAction?.Invoke(savedScores);
            });
            alert.SetNegativeButton("Cancel", (sender, e) => { });

            alert.Show();
        }
    }
}