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
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public static IEnumerable<T> ToEnumerable<T>(this T instance)
        {
            return new[] {instance};
        }

        /// <summary>
        /// Uses the provided string if source string is null or empty
        /// </summary>
        /// <param name="source">The string to test</param>
        /// <param name="defaultValue">The string value to use if source is null or empty</param>
        /// <returns></returns>
        public static string IfNullOrEmpty(this string source, string defaultValue)
        {
            return String.IsNullOrEmpty(source)
                ? defaultValue
                : source;
        }

        /// <summary>
        /// Uses the provided string if source string is null or whitespace
        /// </summary>
        /// <param name="source">The string to test</param>
        /// <param name="defaultValue">The string value to use if source is null or whitespace</param>
        /// <returns></returns>
        public static string IfNullOrWhiteSpace(this string source, string defaultValue)
        {
            return String.IsNullOrWhiteSpace(source)
                ? defaultValue
                : source;
        }

        /// <summary>
        /// Makes generics look right
        /// </summary>
        public static string GetFriendlyName(this Type type)
        {
            string friendlyName = type.Name;
            if (type.IsGenericType)
            {
                int iBacktick = friendlyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    friendlyName = friendlyName.Remove(iBacktick);
                }
                friendlyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = typeParameters[i].Name;
                    friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
                }
                friendlyName += ">";
            }

            return friendlyName;
        }
    }
}
