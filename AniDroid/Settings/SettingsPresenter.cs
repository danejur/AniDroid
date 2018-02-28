using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;

namespace AniDroid.Settings
{
    public class SettingsPresenter : BaseAniDroidPresenter<ISettingsView>
    {
        private readonly IAniDroidSettings _settings;

        public SettingsPresenter(ISettingsView view, IAniListService service, IAniDroidSettings settings) : base(view, service)
        {
            _settings = settings;
        }

        public override async Task Init()
        {
            var test = await _settings.GetCardTypeAsync();
        }
    }
}