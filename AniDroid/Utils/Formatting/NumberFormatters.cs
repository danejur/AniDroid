using System.Globalization;

namespace AniDroid.Utils.Formatting
{
    public static class NumberFormatters
    {
        /// <summary>
        /// Formats the float to a #.#K, #.#M, or #.#B string, depending on size.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string ToTruncatedString(this int num)
        {
            if (num > 999999999)
            {
                return num.ToString("0,,,.#B", CultureInfo.InvariantCulture);
            }
            if (num > 999999)
            {
                return num.ToString("0,,.#M", CultureInfo.InvariantCulture);
            }
            if (num > 999)
            {
                return num.ToString("0,.#K", CultureInfo.InvariantCulture);
            }
            {
                return num.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}