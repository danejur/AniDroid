using Android.Content;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Java.Interop;

namespace AniDroid.Utils.Behaviors
{
    [Register("AniDroid.Utils.Behaviors.FloatingActionButtonScrollBehavior")]
    public class FloatingActionButtonScrollBehavior : CoordinatorLayout.Behavior
    {
        public FloatingActionButtonScrollBehavior(Context context, IAttributeSet attr) { }

        public override bool LayoutDependsOn(CoordinatorLayout parent, Java.Lang.Object child, View dependency)
        {
            return (dependency is Snackbar.SnackbarLayout);
        }

        public override bool OnDependentViewChanged(CoordinatorLayout parent, Java.Lang.Object child, View dependency)
        {
            var fabChild = child as FloatingActionButton;

            var translationY = System.Math.Min(0, dependency.TranslationY - dependency.Height);
            fabChild.TranslationY = translationY;
            return true;
        }

        public override void OnNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dxConsumed, int dyConsumed, int dxUnconsumed, int dyUnconsumed)
        {
            base.OnNestedScroll(coordinatorLayout, child, target, dxConsumed, dyConsumed, dxUnconsumed, dyUnconsumed);
            var fabChild = JavaObjectExtensions.JavaCast<FloatingActionButton>(child);

            if (dyConsumed > 0 && fabChild.Visibility == ViewStates.Visible)
            {
                fabChild.Hide(new CustomOnVisibilityChangedListener());
            }
            else if (dyConsumed < 0 && fabChild.Visibility != ViewStates.Visible)
            {
                fabChild.Show();
            }
        }

        public override bool OnStartNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View directTargetChild, View target, int nestedScrollAxes)
        {
            return nestedScrollAxes == ViewCompat.ScrollAxisVertical;
        }

        private class CustomOnVisibilityChangedListener : FloatingActionButton.OnVisibilityChangedListener
        {
            public override void OnHidden(FloatingActionButton fab)
            {
                base.OnHidden(fab);
                fab.Visibility = ViewStates.Invisible;
            }
        }
    }
}