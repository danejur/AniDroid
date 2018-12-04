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

namespace AniDroid.Adapters.ViewModels
{
    public class UserViewModel : AniDroidAdapterViewModel<User>
    {
        public UserViewModel(User model, UserDetailType primaryUserDetailType, UserDetailType secondaryUserDetailType) : base(model)
        {
            TitleText = Model.Name;
            DetailPrimaryText = GetDetail(primaryUserDetailType);
            DetailSecondaryText = GetDetail(secondaryUserDetailType);
            ImageUri = model.Avatar?.Large ?? model.Avatar?.Medium;
        }

        public enum UserDetailType
        {
            None,
            Following,
            Donator
        }

        public static UserViewModel CreateUserViewModel(User model)
        {
            return new UserViewModel(model, UserDetailType.Following, UserDetailType.None);
        }

        private string GetDetail(UserDetailType detailType)
        {
            string retString = null;

            if (detailType == UserDetailType.Following)
            {
                retString = Model.IsFollowing ? "Following" : "";
            }
            else if (detailType == UserDetailType.Donator)
            {
                retString = Model.DonatorTier > 0 ? "Donator" : "";
            }

            return retString;
        }
    }
}