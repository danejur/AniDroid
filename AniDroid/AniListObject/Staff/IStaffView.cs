using System.Collections.Generic;
using AniDroid.Base;

namespace AniDroid.AniListObject.Staff
{
    public interface IStaffView : IAniListObjectView
    {
        int GetStaffId();
        void SetupStaffView(AniList.Models.StaffModels.Staff staff);
    }
}