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
using AniDroid.Adapters.Base;
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
            View.CreateCardTypeSettingItem(await _settings.GetCardTypeAsync());
            View.CreateAniDroidThemeSettingItem(await _settings.GetThemeAsync());
        }

        public override async Task RestoreState(IList<string> savedState)
        {
            await Init();
        }

        public void SetCardType(BaseRecyclerAdapter.CardType cardType)
        {
            _settings.SetCardType(cardType);
        }

        public void SetTheme(BaseAniDroidActivity.AniDroidTheme theme)
        {
            _settings.SetTheme(theme);
        }
    }
}