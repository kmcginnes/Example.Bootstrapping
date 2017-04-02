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
04:22:00.938 PM DEBUG [4][DatabaseContext]  Inside ctor()
04:22:00.947 PM DEBUG [4][FindJobsHandler]  Inside ctor()
04:22:00.968 PM DEBUG [4][LoggingBehavior<FindJobs,Unit>]  Inside ctor()
04:22:01.795 PM DEBUG [4][LoggingBehavior<FindJobs,Unit>]  Inside Handle()
04:22:01.967 PM  INFO [4][FindJobs]  Handling FindJobs {}
04:22:01.978 PM DEBUG [4][FindJobsHandler]  Inside Handle()
04:22:02.003 PM  INFO [4][FindJobs]  Handled FindJobs with response Unit {}
04:22:02.011 PM DEBUG [4][FindJobs]  Finished timing FindJobs. It took 206 ms
04:22:02.028 PM DEBUG [4][LoggingBehavior<FindJobs,Unit>]  Inside Dispose()
04:22:02.033 PM DEBUG [4][FindJobsHandler]  Inside Dispose()
04:22:02.040 PM DEBUG [4][DatabaseContext]  Inside Dispose()
```
