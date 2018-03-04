﻿using AniDroid.Base;

namespace AniDroid.AniListObject.Media
{
    public interface IMediaView : IAniListObjectView
    {
        int GetMediaId();
        void SetupMediaView(AniList.Models.Media media);
    }
}