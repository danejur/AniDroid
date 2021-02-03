using AniDroid.AniList.Models.UserModels;

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

        public static UserViewModel CreateUserFollowingViewModel(User model)
        {
            return new UserViewModel(model, UserDetailType.None, UserDetailType.None);
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