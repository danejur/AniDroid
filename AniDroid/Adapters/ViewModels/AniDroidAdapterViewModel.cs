using Android.Views;

namespace AniDroid.Adapters.ViewModels
{
    public abstract class AniDroidAdapterViewModel<T> where T : class
    {
        public T Model { get; }

        public string TitleText { get; protected set; }
        public string DetailPrimaryText { get; protected set; }
        public string DetailSecondaryText { get; protected set; }
        public string ImageUri { get; protected set; }
        public bool IsButtonVisible { get; protected set; }
        public int? ButtonIcon { get; protected set; }
        public bool LoadImage { get; protected set; }

        public virtual ViewStates TitleVisibility => TitleText != null ? ViewStates.Visible : ViewStates.Gone;
        public virtual ViewStates DetailPrimaryVisibility => DetailPrimaryText != null ? ViewStates.Visible : ViewStates.Gone;
        public virtual ViewStates DetailSecondaryVisibility => DetailSecondaryText != null ? ViewStates.Visible : ViewStates.Gone;
        public virtual ViewStates ButtonVisibility => IsButtonVisible ? ViewStates.Visible : ViewStates.Gone;
        public virtual ViewStates ImageVisibility => ImageUri != null ? ViewStates.Visible : ViewStates.Invisible;

        protected AniDroidAdapterViewModel(T model)
        {
            Model = model;
            LoadImage = true;
        }

        public virtual void RecreateViewModel()
        {

        }
    }
}