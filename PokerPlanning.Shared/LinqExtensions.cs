using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerPlanning.Shared
{
    public static class LinqExtensions
    {
        public static bool In<T>(this T source, params T[] values)
        {
            return values.Contains(source);
        }

        public static bool In<T>(this T source, IEnumerable<T> values)
        {
            return values.Contains(source);
        }
    }
}