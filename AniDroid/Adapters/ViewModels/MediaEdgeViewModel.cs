﻿using System;
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
using AniDroid.AniList.Models.MediaModels;

namespace AniDroid.Adapters.ViewModels
{
    public class MediaEdgeViewModel : AniDroidAdapterViewModel<MediaEdge>
    {
        public MediaEdgeViewModel(MediaEdge model, MediaEdgeDetailType primaryMediaEdgeDetailType,
            MediaEdgeDetailType secondaryMediaEdgeDetailType) : base(model)
        {
            TitleText = Model.Node?.Title?.UserPreferred;
            DetailPrimaryText = GetDetail(primaryMediaEdgeDetailType);
            DetailSecondaryText = GetDetail(secondaryMediaEdgeDetailType);
            ImageUri = model.Node?.CoverImage?.Large ?? model.Node?.CoverImage?.Medium;
        }

        public enum MediaEdgeDetailType
        {
            None,
            Format,
            Relation,
            StaffRole,
            CharacterRole,
            IsMainStudio
        }

        public static MediaEdgeViewModel CreateMediaEdgeViewModel(MediaEdge model)
        {
            return new MediaEdgeViewModel(model, MediaEdgeDetailType.Format, MediaEdgeDetailType.None);
        }

        public static MediaEdgeViewModel CreateMediaRelationViewModel(MediaEdge model)
        {
            return new MediaEdgeViewModel(model, MediaEdgeDetailType.Format, MediaEdgeDetailType.Relation);
        }

        public static MediaEdgeViewModel CreateStaffMediaViewModel(MediaEdge model)
        {
            return new MediaEdgeViewModel(model, MediaEdgeDetailType.Format, MediaEdgeDetailType.StaffRole);
        }

        public static MediaEdgeViewModel CreateCharacterMediaViewModel(MediaEdge model)
        {
            return new MediaEdgeViewModel(model, MediaEdgeDetailType.Format, MediaEdgeDetailType.CharacterRole);
        }

        public static MediaEdgeViewModel CreateStudioMediaViewModel(MediaEdge model)
        {
            return new MediaEdgeViewModel(model, MediaEdgeDetailType.Format, MediaEdgeDetailType.IsMainStudio);
        }

        private string GetDetail(MediaEdgeDetailType detailType)
        {
            string retString = null;

            if (detailType == MediaEdgeDetailType.Format)
            {
                retString = $"{Model.Node?.Format}";
            }
            else if (detailType == MediaEdgeDetailType.Relation)
            {
                retString = $"{Model.RelationType}";
            }
            else if (detailType == MediaEdgeDetailType.StaffRole)
            {
                retString = $"{Model.StaffRole}";
            }
            else if (detailType == MediaEdgeDetailType.CharacterRole)
            {
                retString = $"{Model.CharacterRole}";
            }
            else if (detailType == MediaEdgeDetailType.IsMainStudio)
            {
                retString = Model.IsMainStudio ? "Main Studio" : "";
            }

            return retString;
        }
    }
}