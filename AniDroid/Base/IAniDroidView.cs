using AniDroid.AniList.Interfaces;

namespace AniDroid.Base
{
    public interface IAniDroidView
    {
        void OnError(IAniListError error);
        void DisplaySnackbarMessage(string message, int length);
        void DisplayNotYetImplemented();
    }
}