using System;

namespace Example.Bootstrapping
{
    /// <summary>
    /// The disposable action.
    /// </summary>
    public class Disposable : IDisposable
    {
        private Action _dispose;

        public static IDisposable Create(Action action)
        {
            return new Disposable(action);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposable"/> class.
        /// </summary>
        /// <param name="dispose">
        /// The dispose.
        /// </param>
        private Disposable(Action dispose)
        {
            if (dispose == null) throw new ArgumentNullException("dispose");

            this._dispose = dispose;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposable"/> class.
        /// </summary>
        /// <param name="construct">
        /// The construct.
        /// </param>
        /// <param name="dispose">
        /// The dispose.
        /// </param>
        public Disposable(Action construct, Action dispose)
        {
            if (construct == null) throw new ArgumentNullException("construct");
            if (dispose == null) throw new ArgumentNullException("dispose");

            construct();

            this._dispose = dispose;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._dispose == null)
                {
                    return;
                }

                try
                {
                    this._dispose();
                }
                catch (Exception)
                {
                    // ToDo: Log error?
                }

                this._dispose = null;
            }
        }
    }
}