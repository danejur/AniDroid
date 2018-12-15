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
    public class StudioEdgeViewModel : AniDroidAdapterViewModel<Studio.Edge>
    {
        public override ViewStates ImageVisibility => ViewStates.Gone;

        public StudioEdgeViewModel(Studio.Edge model, StudioEdgeDetailType primaryStudioEdgeDetailType,
            StudioEdgeDetailType secondaryStudioEdgeDetailType) : base(model)
        {
            TitleText = Model.Node?.Name;
            DetailPrimaryText = GetDetailString(primaryStudioEdgeDetailType);
            DetailSecondaryText = GetDetailString(secondaryStudioEdgeDetailType);
        }

        public enum StudioEdgeDetailType
        {
            None,
            IsMainStudio
        }

        public static StudioEdgeViewModel CreateStudioEdgeViewModel(Studio.Edge model)
        {
            return new StudioEdgeViewModel(model, StudioEdgeDetailType.IsMainStudio, StudioEdgeDetailType.None);
        }

        private string GetDetailString(StudioEdgeDetailType detailType)
        {
            string retString = null;

            if (detailType == StudioEdgeDetailType.IsMainStudio)
            {
                retString = Model.IsMain ? "Main Studio" : null;
            }

            return retString;
        }
    }
}