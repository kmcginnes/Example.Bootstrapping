using System;
using System.ComponentModel;

namespace Example.Bootstrapping.Logging
{
    /// <summary>
    /// Logger type initialization
    /// </summary>
    public static class Log
    {
        private static ILog _testLogger;
        private static Func<Type, ILog> _createLogger = type => new NullLog(type);

        /// <summary>
        /// Sets up logging to be with a certain type
        /// </summary>
        /// <typeparam name="T">The type of ILog for the application to use</typeparam>
        public static void InitializeWith<T>() where T : ILog
        {
            _createLogger = type => Activator.CreateInstance(typeof(T), type) as ILog;
        }

        /// <summary>
        /// Sets up logging to be with a certain type
        /// </summary>
        /// <typeparam name="T">The type of ILog for the application to use</typeparam>
        public static void InitializeWith<T>(Func<Type, T> createLogger) where T : ILog, new()
        {
            _createLogger = type => createLogger(type);
        }

        /// <summary>
        /// Sets up logging to be with a certain instance. The other method is preferred.
        /// </summary>
        /// <param name="testLoggerType">Type of the logger.</param>
        /// <remarks>This is mostly geared towards testing</remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void InitializeWith(ILog testLoggerType)
        {
            _testLogger = testLoggerType;
        }

        /// <summary>
        /// Initializes a new instance of a logger for an object.
        /// This should be done only once per object name.
        /// </summary>
        /// <param name="loggerTypeContext">Name of the object.</param>
        /// <returns>ILog instance for an object if log type has been intialized; otherwise null</returns>
        public static ILog GetLoggerFor(Type loggerTypeContext)
        {
            if (_testLogger != null) return _testLogger;

            var logger = _createLogger(loggerTypeContext);

            return logger;
        }
    }
}