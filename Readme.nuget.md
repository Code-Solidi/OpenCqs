An implementation of Command-Query Separation with built-in dependency injection of all handlers.

**OpenCqs** defines both ```IQuery``` and ```ICommand``` which serve as the implementation bases for actual queries and commands. Handlers implement ```IQueryHandler``` and ```ICommandHandler``` interfaces and come in "regular" and ```async``` flavours. In addition "decorating" handlers (much like in AOP) may be *injected* into implementing handlers in order to augment functionality.

More about **OpenCqs** [here](https://github.com/Code-Solidi/OpenCqs).
