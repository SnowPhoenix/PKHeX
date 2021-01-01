﻿namespace PKHeX.Core
{
    public interface IContestStats
    {
        int CNT_Cool { get; }
        int CNT_Beauty { get; }
        int CNT_Cute { get; }
        int CNT_Smart { get; }
        int CNT_Tough { get; }
        int CNT_Sheen { get; }
    }

    public interface IContestStatsMutable
    {
        int CNT_Cool { set; }
        int CNT_Beauty { set; }
        int CNT_Cute { set; }
        int CNT_Smart { set; }
        int CNT_Tough { set; }
        int CNT_Sheen { set; }
    }

    public static partial class Extensions
    {
        public static int[] GetContestStats(this IContestStats stats) => new[]
        {
            stats.CNT_Cool,
            stats.CNT_Beauty,
            stats.CNT_Cute,
            stats.CNT_Smart,
            stats.CNT_Tough,
            stats.CNT_Sheen
        };

        /// <summary>
        /// Checks if any contest stat value is nonzero.
        /// </summary>
        /// <param name="stats">Object containing contest stat data.</param>
        /// <returns>True if has any nonzero contest stat, false if all are zero.</returns>
        public static bool HasContestStats(this IContestStats stats)
        {
            if (stats.CNT_Cool != 0)
                return true;
            if (stats.CNT_Beauty != 0)
                return true;
            if (stats.CNT_Cute != 0)
                return true;
            if (stats.CNT_Smart != 0)
                return true;
            if (stats.CNT_Tough != 0)
                return true;
            if (stats.CNT_Sheen != 0)
                return true;
            return false;
        }

        public static bool IsContestBelow(this IContestStats current, IContestStats initial) => !current.IsContestAboveOrEqual(initial);

        public static bool IsContestAboveOrEqual(this IContestStats current, IContestStats initial)
        {
            if (current.CNT_Cool   < initial.CNT_Cool)
                return false;
            if (current.CNT_Beauty < initial.CNT_Beauty)
                return false;
            if (current.CNT_Cute   < initial.CNT_Cute)
                return false;
            if (current.CNT_Smart  < initial.CNT_Smart)
                return false;
            if (current.CNT_Tough  < initial.CNT_Tough)
                return false;
            if (current.CNT_Sheen  < initial.CNT_Sheen)
                return false;
            return true;
        }

        public static void CopyContestStatsTo(this IContestStats source, IContestStatsMutable dest)
        {
            dest.CNT_Cool = source.CNT_Cool;
            dest.CNT_Beauty = source.CNT_Beauty;
            dest.CNT_Cute = source.CNT_Cute;
            dest.CNT_Smart = source.CNT_Smart;
            dest.CNT_Tough = source.CNT_Tough;
            dest.CNT_Sheen = source.CNT_Sheen;
        }
    }
}
