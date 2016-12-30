using System;
using System.Collections.Generic;
using System.Linq;

namespace Example.Bootstrapping
{
    public static class Extensions
    {
        public static IEnumerable<T> Do<T>(this IEnumerable<T> source, Action<T> action)
        {
            return source.Select(x =>
            {
                action(x);
                return x;
            });
        }

        public static IEnumerable<T> ToEnumerable<T>(this T instance)
        {
            return new[] {instance};
        }
    }
}
