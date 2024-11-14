using System;
using System.Collections.Generic;
using System.Linq;
using AniDroid.AniList.DataTypes;
using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.AniList.Enums.UserEnums;
using AniDroid.AniList.Models.UserModels;

namespace AniDroid.AniList.Models.MediaModels
{
    public class MediaList : AniListObject
    {
        public int UserId { get; set; }
        public int MediaId { get; set; }
        public MediaListStatus Status { get; set; }
        public float Score { get; set; }
        public int? Progress { get; set; }
        public int? ProgressVolumes { get; set; }
        public int? Repeat { get; set; }
        public int Priority { get; set; }
        public bool Private { get; set; }
        public string Notes { get; set; }
        public bool HiddenFromStatusLists { get; set; }
        public List<MediaCustomList> CustomLists { get; set; }
        // TODO: work-around. see about getting this changed to an array
        public dynamic AdvancedScores { get; set; }
        public FuzzyDate StartedAt { get; set; }
        public FuzzyDate CompletedAt { get; set; }
        public int UpdatedAt { get; set; }
        public int CreatedAt { get; set; }
        public Media Media { get; set; }
        public User User { get; set; }

        public string GetScoreString(ScoreFormat scoreFormat)
        {
            if (scoreFormat == ScoreFormat.ThreeSmileys)
            {
                return new[] {"🤔 (no score)", "🙁", "😐", "🙂"}[Math.Min((int)Score, 3)];
            }

            if (Score == 0)
            {
                return "No score given";
            }

            if (scoreFormat == ScoreFormat.TenDecimal)
            {
                return $"{Score:#.#} / 10";
            }

            if (scoreFormat == ScoreFormat.FiveStars)
            {
                return string.Concat(Enumerable.Repeat("★", (int)Score));
            }

            return scoreFormat == ScoreFormat.Ten ? $"{Score:#} / 10" : $"{Score:#} / 100";
        }

        public string GetFormattedProgressString(MediaType type, int? maxProgress)
        {
            var retStr = "";

            if (Status == MediaListStatus.Completed)
            {
                retStr = CompletedAt?.IsValid() == true
                    ? $"Completed on:  {CompletedAt.GetFuzzyDateString()}"
                    : "Completed on unknown date";
            }
            else if (Status == MediaListStatus.Planning)
            {
                retStr = $"Added to lists:  {GetFormattedDateString(CreatedAt)}";
            }
            else if (Status == MediaListStatus.Current || Status == MediaListStatus.Dropped || Status == MediaListStatus.Paused || Status == MediaListStatus.Repeating)
            {
                var progressType = "";

                if (type == MediaType.Anime)
                {
                    progressType = $"episode{(Progress == 1 ? "" : "s")}";
                }
                else if (type == MediaType.Manga)
                {
                    progressType = $"chapter{(Progress == 1 ? "" : "s")}";
                }

                retStr =
                    $"Progress:  {Progress}{(maxProgress.HasValue ? $" / {maxProgress} " : " ")}{progressType}";
            }

            if ((Repeat ?? 0) > 0)
            {
                retStr += $"  (Repeats:  {Repeat})";
            }

            return retStr;
        }
    }
}