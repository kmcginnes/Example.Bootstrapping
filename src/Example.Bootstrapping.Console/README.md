# Console App

Here is the best way to lay out a console application.

## Third party dependencies

| Type      | Name    |
| --------- | ------- | 
| IoC       | AutoFac |
| Messaging | MediatR |

## Note on AutoFac and MediatR

When using MediatR with AutoFac, you can run into scoping issues. This means that your MediatR handlers may not get disposed when you expect them to.

You generally want your handler and its dependencies to be disposed after the `Handle()` method completes.

In order to accomplish this, I had to create a wrapper around `Mediator` called `ScopedMediator` that will create a new instance of `Mediator` with each request. This is so that I can define the lifetime scope of the dependency resolution through AutoFac.

```c#
public Task<TResponse> Send<TResponse>(
    IRequest<TResponse> request, 
    CancellationToken cancellationToken = new CancellationToken())
{
    using (var scope = _createScope())
    {
        var mediator = CreateScopedMediator(scope);
        return mediator.Send(request, cancellationToken);
    }
}
```

The `_createScope()` call will ask AutoFac to start a new lifetime scope using the registration:

```c#
builder.Register(ctx => (Func<ILifetimeScope>)(() => hackToRegisterContainer.Value.BeginLifetimeScope()));
```

And the `CreateScopedMediator(scope)` call will instantiate a new Mediator using resolvers off of the new lifetime scope:

```c#
private IMediator CreateScopedMediator(ILifetimeScope scope)
{
    var mediator = new Mediator(
        t => scope.Resolve(t),
        t => (IEnumerable<object>)scope.Resolve(typeof(IEnumerable<>).MakeGenericType(t)));
    return mediator;
}
```

This ends up with an execution pipeline that looks like this:

```
DatabaseContext.ctor()
FindJobsHandler.ctor()
LoggingBehavior.ctor()
LoggingBehavior.Handle()
FindJobsHandler.Handle()
LoggingBehavior.Dispose()
FindJobsHandler.Dispose()
DatabaseContext.Dispose()
```
