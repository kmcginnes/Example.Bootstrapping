# Logging Infrastructure

Logging should be as simple as possible, and pervasive throughout your code base.

This is an example of an implementation of `this.Log()` logging abstraction. I've seen a few flavors of this, but this one is mine.

## DefaultBaseLog.cs

Houses a default implementation of all of `ILog`, as well as some default formatting of the string output.

Inheritors of this abstract class need only implement the `Write()` and `WriteLazy()` methods.

There are two subclasses of `DefaultBaseLog`: `ConsoleLog` and `FileLog`

## FileLog.cs

Dirt simple implementation of a file appending logger. There is no retention policy, size limits, etc.

## ConsoleLog.cs

Logs using `Console.WriteLine()`.
