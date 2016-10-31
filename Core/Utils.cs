using System;
using System.Collections.Generic;
using Core;
using Knuj.Interfaces.Services;
 

namespace Knuj
{
    public static class Utils
    {
        public static string AnalyticsMapSecondsToRange(double s)
        {
            if (s < 11)
                return AnalyticEventNames.ViewedFor10s;
            if (s < 61)
                return AnalyticEventNames.ViewedFor60s;
            if (s < 601)
                return AnalyticEventNames.ViewedFor600s;

            return AnalyticEventNames.ViewedForLong;
        }

        public static double GetElapsedSeconds(long ticksT1, long ticksT2)
        {
            var elapsedTicks = ticksT2 - ticksT1;
            var elapsedSpan = new TimeSpan(elapsedTicks);
            return elapsedSpan.TotalSeconds;
        }

        public static bool IsExpired(long ticksT1, long ticksT2, long expirationInSeconds)
        {
            var expired = (GetElapsedSeconds(ticksT1, ticksT2) - expirationInSeconds) > 0;
            return expired;
        }

        public static IEnumerable<T> SkipLastN<T>(this IEnumerable<T> source, int n)
        {
            var it = source.GetEnumerator();
            bool hasRemainingItems = false;
            var cache = new Queue<T>(n + 1);

            do
            {
                if (hasRemainingItems = it.MoveNext())
                {
                    cache.Enqueue(it.Current);
                    if (cache.Count > n)
                        yield return cache.Dequeue();
                }
            } while (hasRemainingItems);
        }

    }
}
