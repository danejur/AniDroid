namespace AniDroid.AniList.Interfaces
{
    public interface IAuthCodeResolver
    {
        /// <summary>
        /// The Authorization Code presented by AniList to be used with all requests. If this does not exist, only Anonymous requests should be allowed.
        /// </summary>
        string AuthCode { get; }
        
        /// <summary>
        /// Returns whether the <see cref="AuthCode"/> is still valid. Typically, this should just be a !String.IsNullOrWhitespace.
        /// </summary>
        bool IsAuthorized { get; }

        /// <summary>
        /// Sets the <see cref="AuthCode"/> as invalid. Typically, this should just clear its value.
        /// </summary>
        void Invalidate();
    }
}
