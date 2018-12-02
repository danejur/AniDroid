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
    public class StaffViewModel : AniDroidAdapterViewModel<Staff>
    {
        public Staff.Edge ModelEdge { get; protected set; }

        public StaffViewModel(Staff model, StaffDetailType primaryStaffDetailType, StaffDetailType secondaryStaffDetailType) : base(model)
        {
            TitleText = $"{Model.Name?.FormattedName}";
            DetailPrimaryText = GetDetail(primaryStaffDetailType);
            DetailSecondaryText = GetDetail(secondaryStaffDetailType);
            ImageUri = model.Image?.Large ?? model.Image?.Medium;
        }

        public enum StaffDetailType
        {
            None,
            NativeName,
            Language,
            Role
        }

        private string GetDetail(StaffDetailType detailType)
        {
            var retString = "";

            if (detailType == StaffDetailType.NativeName)
            {
                retString = $"{Model.Name?.Native}";
            }
            else if (detailType == StaffDetailType.Language)
            {
                retString = $"{Model.Language}";
            }
            else if (detailType == StaffDetailType.Role)
            {
                retString = $"{ModelEdge?.Role}";
            }

            return retString;
        }
    }
}