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
using AniDroid.AniList.Models.StudioModels;

namespace AniDroid.Adapters.ViewModels
{
    public class StudioViewModel : AniDroidAdapterViewModel<Studio>
    {
        public StudioEdge StudioEdge { get; protected set; }

        public StudioViewModel(Studio model, StudioDetailType primaryStudioDetailType, StudioDetailType secondaryStudioDetailType, bool isButtonVisible) : base(model)
        {
            TitleText = Model.Name;
            DetailPrimaryText = GetDetail(primaryStudioDetailType);
            DetailSecondaryText = GetDetail(secondaryStudioDetailType);
            IsButtonVisible = isButtonVisible;
        }

        public enum StudioDetailType
        {
            None,
            IsMainStudio
        }

        public static StudioViewModel CreateStudioViewModel(Studio model)
        {
            return new StudioViewModel(model, StudioDetailType.None, StudioDetailType.None, model.IsFavourite);
        }

        private string GetDetail(StudioDetailType detailType)
        {
            string retString = null;

            if (detailType == StudioDetailType.IsMainStudio)
            {
                retString = StudioEdge?.IsMain == true ? "Main Studio" : "";
            }

            return retString;
        }
    }
}