using System;

namespace Example.Bootstrapping
{
    public interface IDependencyContainer
    {
        void Register<T>(Func<T> resolve);
    }
}