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

        public ViewStates TitleVisibility => TitleText != null ? ViewStates.Visible : ViewStates.Gone;
        public ViewStates DetailPrimaryVisibility => DetailPrimaryText != null ? ViewStates.Visible : ViewStates.Gone;
        public ViewStates DetailSecondaryVisibility => DetailSecondaryText != null ? ViewStates.Visible : ViewStates.Gone;
        public ViewStates ImageVisibility => ImageUri != null ? ViewStates.Visible : ViewStates.Invisible;
        public ViewStates ButtonVisibility => IsButtonVisible ? ViewStates.Visible : ViewStates.Gone;

        protected AniDroidAdapterViewModel(T model)
        {
            Model = model;
        }
    }
}