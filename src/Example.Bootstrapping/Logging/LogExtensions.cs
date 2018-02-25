using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Example.Bootstrapping.Logging;

// ReSharper disable once CheckNamespace
// The logging extension should exist in your root application namespace
namespace Example.Bootstrapping
{
    /// <summary>
    /// Extensions to help make logging awesome - this should be installed into the root namespace of your application
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Concurrent dictionary that ensures only one instance of a logger for a type.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, ILog> Cache = new ConcurrentDictionary<Type, ILog>();

        /// <summary>
        /// Gets the logger for <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance to use for the logger context type.</param>
        /// <returns>Instance of a logger for the object.</returns>
        public static ILog Log<T>(this T instance)
        {
            var realType = instance.GetType();
            return Log(realType);
        }

        /// <summary>
        /// Gets the logger for the specified object name.
        /// </summary>
        /// <param name="type">Either use the fully qualified object name or the short. If used with Log&lt;T&gt;() you must use the fully qualified object name"/></param>
        /// <returns>Instance of a logger for the object.</returns>
        public static ILog Log(this Type type)
        {
            return Cache.GetOrAdd(type, Logging.Log.GetLoggerFor);
        }
        
        /// <summary>
        /// Starts a timer until the return value is disposed, and then prints out how long it took.
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="context">A name for what is being timed</param>
        /// <returns></returns>
        public static IDisposable Time(this ILog logger, string context)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            return Disposable.Create(() =>
            {
                stopwatch.Stop();
                logger.Debug($"Finished timing {context}. It took {stopwatch.ElapsedMilliseconds} ms");
            });
        }
    }
}