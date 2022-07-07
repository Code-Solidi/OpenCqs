# OpenCqs
An implementation of CQS with built-in dependency injection of all handlers. More about it [here](https://blogs.cuttingedge.it/steven/posts/2011/meanwhile-on-the-command-side-of-my-architecture/) and [here](https://www.dotnetcurry.com/patterns-practices/1305/aspect-oriented-programming-aop-csharp-using-solid).

## What is CQS?
Command Query Separation (**CQS**) is a design pattern that first appeared in the Eiffel programming language, and was introduced by Bertrand Meyer. In just a sentence 
it divides class methods in two major categories: 
- commands - each command performs an action, or 
- queries - a query returns data to the caller.
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
  }
}
```

## Dependency Injection
Both ```QueryHandlerBase``` and ```CommandHandlerBase``` as well as their async twins implement ```IQueryHandler<T, TR>``` and '''ICommandHandler<T, TR>``` correspondingly:

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
 
The above definitions are excellent candidates for ctor DI. We can pass the required interfaces but leave the resolution of the implementations to the DI container:


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

There has to be a line 

```
  services.AddScoped<IQueryHandler<Query, object>, QueryHandle>();
```

(or ```AdTransient, AddSingleton```) preceeding the ctor injection. Imagine also a real appllication with **lots of queries, commands, and handlers**.

A good practice in CQS is to define many small, atomic handlers, thus splitting a complex tasks into smaller and manageable chunks. This is good if there's an issue - it can be spotted very easily, but also leads to forgotten registrations, which is an issue by itself.

### Automatic DI registrations
**OpenCqs** defines the ```AddHandlers()``` method which takes care of all registrations in an assembly, just invoke it where the <query, handler> (<command, handler>) registrations should go:

```
  services.AddHandlers(this.GetType().Assembly);
```

It discovers all ```IQueryHandler<T, TR>``` and ```ICommandHandler<T, TR>``` implementations and registers them for you. (It works on the ```async``` versions as well!)

## Decorating handlers

In addition there are "decorating" handlers which are wrappers arround the actual handlers:

```
public override string Handle(DecoratedTestQuery query)
{
  this.logger.LogInformation("...");
  var result = this.next?.Handle(query);
  this.logger.LogInformation("...");
  return result;
}
```

This handler logs something before and after calling the "original" ```Handle()``` method. Or this:

```
public override int Handle(DivisionByZeroQuery query)
{
  var result = default(int);
  try
  {
    result = this.next.Handle(query);
  }
  catch (Exception x)
  {
    if (!this.HandleException(x)) { throw; }
  }

  return result;
}
```	

which handles exceptions thrown in the original handler's ```Handle()``` body. 

Decorating handlers can be nested of course. So, you can have a exception catching handler wrapping the original one, which in turn is wrapped by a logging handler, which in turn... and so on.

### The [Decorator] attribute

Decorating handler follow exactly the same layout as "normal" handlers. They inherit the same class as the decorated handler, but are not injected the same way. Instead, the decorated handler keeps track of its decorators. 

The decorators are added in the decorated handler ctor like this:
```
public DecoratedQueryHandler(ILogger logger)
{
  this.Add(new ExceptionQueryHandler(logger));   // closer
  this.Add(new LoggingQueryHandler(logger));     // farther
}
```



	


