using Android.App;
using Android.Content;
using Android.Graphics;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.DrawerLayout.Widget;

namespace AniDroid.Widgets
{
    public class BadgeDrawerToggle : ActionBarDrawerToggle
    {
        private BadgeDrawerArrowDrawable _badgeDrawable;

        public BadgeDrawerToggle(Activity activity, DrawerLayout drawerLayout, Toolbar toolbar, int openDrawerContentDescRes, int closeDrawerContentDescRes) : base(activity, drawerLayout, toolbar, openDrawerContentDescRes, closeDrawerContentDescRes)
        {
            Init(activity);
        }

        public BadgeDrawerToggle(Activity activity, DrawerLayout drawerLayout, int openDrawerContentDescRes, int closeDrawerContentDescRes) : base(activity, drawerLayout, openDrawerContentDescRes, closeDrawerContentDescRes)
        {
            Init(activity);
        }

        private void Init(Context activity)
        {
            _badgeDrawable = new BadgeDrawerArrowDrawable(activity);
            DrawerArrowDrawable = _badgeDrawable;
        }

        public bool BadgeEnabled
        {
            get => _badgeDrawable.IsEnabled;
            set => _badgeDrawable.IsEnabled = value;
        }

        public void SetBadgeText(string text)
        {
            _badgeDrawable.Text = text;
        }

        public void SetToggleColor(Color color)
        {
            _badgeDrawable.Color = color;
        }
    }
}