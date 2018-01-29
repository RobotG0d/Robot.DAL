using Robot.Expected;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Robot.DAL.Orchestration.Extensions
{
    internal static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }
    }
}
