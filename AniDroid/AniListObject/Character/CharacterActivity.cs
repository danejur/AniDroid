using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using AniDroid.Base;
using AniDroid.Utils;
using Ninject;

namespace AniDroid.AniListObject.Character
{
    [Activity(Label = "Character")]
    public class CharacterActivity : BaseAniListObjectActivity<CharacterPresenter>, ICharacterView
    {
        public const string CharacterIdIntentKey = "CHARACTER_ID";

        protected override IReadOnlyKernel Kernel =>
            new StandardKernel(new ApplicationModule<ICharacterView, CharacterActivity>(this));

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            // TODO: implement
        }

        public static void StartActivity(BaseAniDroidActivity context, int characterId, int? requestCode = null)
        {
            var intent = new Intent(context, typeof(CharacterActivity));
            intent.PutExtra(CharacterIdIntentKey, characterId);

            if (requestCode.HasValue)
            {
                context.StartActivityForResult(intent, requestCode.Value);
            }
            else
            {
                context.StartActivity(intent);
            }
        }
    }
}