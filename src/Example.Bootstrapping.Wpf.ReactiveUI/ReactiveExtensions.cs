using System;
using System.Reactive.Disposables;

namespace Example.Bootstrapping.Wpf.ReactiveUI
{
    public static class ReactiveExtensions
    {
        public static void DisposeWith(this IDisposable disposable, CompositeDisposable compositeDisposable)
        {
            if (disposable == null) throw new ArgumentNullException(nameof(disposable), "Must provide non-null object to dispose.");
            if (compositeDisposable == null) throw new ArgumentNullException(nameof(compositeDisposable), "Must provide non-null CompositeDisposable object.");

            compositeDisposable.Add(disposable);
        }
    }
}
