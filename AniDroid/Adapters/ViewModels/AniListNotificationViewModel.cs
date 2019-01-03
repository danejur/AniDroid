using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Media;
using AniDroid.AniListObject.User;
using AniDroid.Base;

namespace AniDroid.Adapters.ViewModels
{
    public class AniListNotificationViewModel : AniDroidAdapterViewModel<AniListNotification>
    {
        public Action ClickAction { get; protected set; }
        public ISpanned FormattedTitle { get; protected set; }
        public string Timestamp { get; protected set; }

        private readonly BaseAniDroidActivity _context;

        public AniListNotificationViewModel(AniListNotification model, BaseAniDroidActivity context, Color accentColor) : base(model)
        {
            _context = context;

            FormattedTitle = BaseAniDroidActivity.FromHtml(Model.GetNotificationHtml($"#{accentColor & 0xffffff:X6}"));
            Timestamp = Model.GetAgeString(model.CreatedAt);
            ImageUri = Model.GetImageUri();
            ClickAction = GetNotificationClickAction();
        }

        public static AniListNotificationViewModel CreateViewModel(AniListNotification model, BaseAniDroidActivity context, Color accentColor)
        {
            return new AniListNotificationViewModel(model, context, accentColor);
        }

        private Action GetNotificationClickAction()
        {
            Action retAction = () => { };
            var actionType = Model.GetNotificationActionType();

            if (actionType.Equals(AniListNotification.NotificationActionType.Media))
            {
                retAction = () => MediaActivity.StartActivity(_context, Model.Media.Id);
            }
            else if (actionType.Equals(AniListNotification.NotificationActionType.User))
            {
                retAction = () => UserActivity.StartActivity(_context, Model.User.Id);
            }
            else if (actionType.Equals(AniListNotification.NotificationActionType.Activity))
            {
                retAction = () => _context.StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse($"https://anilist.co/activity/{Model.ActivityId}")));
            }
            else if (actionType.EqualsAny(AniListNotification.NotificationActionType.Thread, AniListNotification.NotificationActionType.Comment))
            {
                retAction = () => _context.StartActivity(new Intent(Intent.ActionView,
                    Android.Net.Uri.Parse(
                        $"https://anilist.co/forum/thread/{Model.Thread?.Id}{(Model.CommentId > 0 ? $"/comment/{Model.CommentId}" : "")}")));
            }

            return retAction;
        }
    }
}