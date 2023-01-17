Live demo [here](https://infostore.codesolidi.com/). 

*The full source code of the application is [here](https://github.com/Code-Solidi/InfoStore). More specifically, look in the following folders `UseCases` for commands and queries, and `Data\Handlers` for handlers. These are available in every CoreXF extension except `Bookshelf`.*

# OpenCqs
An implementation of CQS with built-in dependency injection of all handlers. More about it [here](https://blogs.cuttingedge.it/steven/posts/2011/meanwhile-on-the-command-side-of-my-architecture/) and [here](https://www.dotnetcurry.com/patterns-practices/1305/aspect-oriented-programming-aop-csharp-using-solid).

The **NuGet** package is available for [download](https://www.nuget.org/packages/OpenCqs/):

```dotnet add package OpenCqs --version 1.0.1```

## What is CQS?
Command Query Separation (**CQS**) is a design pattern that first appeared in the Eiffel programming language, and was introduced by Bertrand Meyer. In just a sentence 
it divides class methods in two major categories: 
- commands: each command performs an action, or 
- queries: a query returns data to the caller.


This is the Single Responsibility Principle on a method level, which states the a method is either a query or a command but never both.

In addition commands returns void and queries change nothing, so they have no side effects.

The main concept may be defined like this:

```
class Query 
{
  // parameters here
}

class QueryHandler 
{
  object Handle(Query query)
  {
    // implementation here
    return something;
  }
}
```
or
```
class Command 
{
  // parameters here
}

class CommandHandler 
{
  void Handle(Command command)
  {
    // implementation here
  }
}
```

## What is OpenCqs?
**OpenCqs** defines both ```IQuery``` and ```ICommand``` which serve as the implementation bases for actual queries and commands. **OpenCqs** defines two base clasees for implementing the corresponding handlers: 
```HandlerBase<T, TR>``` and ```HandlerBaseAsync<T, TR>``` each one being the base class for ```QueryHandlerBase<T, TR>```,  ```QueryHandlerBaseAsync<T, TR>``` and ```CommandHandlerBase<T, TR>```, ```CommandHandlerBaseAsync<T, TR>``` respectivelly.

Thus ```Query``` becomes

```
class Query : IQuery
{
  // parameters here
}
```

and ```QueryHandler``` is defined like this:


```
class QueryHandler : QueryHandlerBase<Query, object>
{
  object Handle(Query query)
  {
    // implementation here
    return something;
  }
}
```

## Dependency Injection
Both ```QueryHandlerBase``` and ```CommandHandlerBase``` as well as their async twins implement ```IQueryHandler<T, TR>``` and ```ICommandHandler<T, TR>``` correspondingly:

```
 public abstract class QueryHandlerBase<T, TR> : HandlerBase<T, TR>, IQueryHandler<T, TR> where T : IQuery
 {
 }
```	

and 

```
 public abstract class CommandHandlerBase<T, TR> : HandlerBase<T, TR>, ICommandHandler<T, TR> where T : ICommand
 {
 }
```	
 
The above definitions are excellent candidates for constructor DI. We can pass the required interfaces but leave the resolution of the implementations to the DI container:


```
public MyClass(IQueryHandler<Query, object> handler)
{
  this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
}
```

The DI container resolves the ```handler``` to ```QueryHandler``` and later on when a call 

```
  var result = this.handler.Handle(new Query { /*...*/ });
```

is issued it invokes exactly the ```Handler``` method defined in ```QueryHandler```.  

All good and well but this requires some manual coding. Say we use the built in ```ServiceCollection``` DI which comes with .NET. 

For each and every handler there has to be a registration like the one below: 

```
  services.AddScoped<IQueryHandler<Query, object>, QueryHandle>();
```

(or ```AdTransient, AddSingleton```) preceeding the constructor injection. Imagine also a real appllication with **lots of queries, commands, and handlers**.

A good practice in CQS is to define many small, atomic handlers, thus splitting complex tasks into smaller and manageable chunks. This is good if there's an issue &ndash; it can be spotted very easily, but also leads to forgotten registrations, which is an issue by itself. The missing registration becomes really difficult to find among the large number of similar ones.

### Automatic DI registrations
**OpenCqs** defines the ```AddHandlers()``` method which takes care of all registrations in an assembly, just invoke it where the <query, handler> (<command, handler>) registrations should go:

```
  services.AddHandlers(this.GetType().Assembly);
```

It discovers all ```IQueryHandler<T, TR>``` and ```ICommandHandler<T, TR>``` implementations and registers them for you. (It works on the ```async``` versions as well.)
As a result the large number of DI registration lines (like ```services.Add...<>()```) are replaced with a single line  (the one above).

## Decorating handlers

In addition there are "decorating" handlers which are wrappers arround the actual handlers:

```
public override string Handle(DecoratedTestQuery query)
{
  this.logger.LogInformation("...");
  
  var result = this.next.Handle(query); // inner (decorated) handlers called
  
  this.logger.LogInformation("...");
  return result;
}
```

This handler logs something before and after calling the "original" ```Handle()``` method. Or this:

```
public override int Handle(StopWatchQuery query)
{
  var stopWatch = new Stopwatch();
  stopWatch.Start();

  var result = this.next.Handle(query); // inner (decorated) handlers called

  stopWatch.Stop();
  var ts = stopWatch.Elapsed;

  this.logger.LogDebug($"Execution time {(ts.TotalMilliseconds / 1000):0.0000000}s.");
  
  return result;
}
```	

which estimates the execution time for the original handler's ```Handle()``` body. 

Decorating handlers can be nested. So, you can have an error (exception catching) handler wrapping the original one, which in turn is wrapped by a logging handler, which in turn... and so on.

Thus you can define a preparation step, and a post-call step having all the necessary tasks done by other handlers leaving the original totally ignorant of the context in whcih they are used. Sounds much like AOP, eh?

For example, you can define a event-generating handler which fires an event either before, or after calling the original one, or both. You can also have an access control handler which, when executing the command/query, deteremines if the user (either anonymous, or authenticated) is allowed to perform this type of action on this object. (Remember the good old ACE and ACL in Windows NT? I'm going to show you how to do this somewhat later &mdash; this is yet to come.)

### The [Decorator] attribute

Decorating handlers follow exactly the same layout as "normal" handlers. They inherit the same class as the decorated handler, but are not injected the same way. Instead, the decorated handler keeps track of its decorators. 

The decorators are added in the decorated handler constructor like this:
```
public DecoratedQueryHandler(ILogger logger)
{
  this.Add(new StopWatchQueryHandler(logger));   // closest
  this.Add(new ExceptionQueryHandler(logger));   
  //...
  this.Add(new LoggingQueryHandler(logger));     // farthest
}
```

One thing to note &ndash; the first call to ```Add(...)``` adds a decorating handler which is the closest to the "original" one, the next call wraps both in the second handler, and so on. The actual excution starts with the farthest hadler and digs down and inside until it reaches the original one. Then execution 'pops up' until the farthest handler is reached again.

## Roadmap
This project is in a relatively stable state. However, its functionality may be extended, or it may contain bugs or other deffects and omissions.

In such a case if you want to contribute please, open an issue or, if you have fixed something in the source code, create a pull request. 

Thank you!
