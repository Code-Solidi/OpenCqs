using OpenCqs2.Abstractions;
using OpenCqs2.Policies.Exceptions;
using OpenCqs2.Policies.Logging;
using OpenCqs2.Proxies;

namespace OpenCqs2Demo.Demos
{
    public class DemoQueryHandler : IQueryHandler<DemoQuery, string?>
    {
        public HandlerResult<string?> Handle(DemoQuery query)
        {
            var result = new HandlerResult<string?> { Result = $"From query: {query.Text}" };
            //Console.WriteLine($"*** {result} ***");
            return result;
        }

        internal static IQueryHandler<DemoQuery, string?> CreateProxy(IServiceProvider provider)
        {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));

            var handler = LoggingProxy<IQueryHandler<DemoQuery, string?>>.Create(new DemoQueryHandler(), new LoggingPolicy(provider))!;
            handler = ExceptionProxy<IQueryHandler<DemoQuery, string?>>.Create(handler, new ExceptionPolicy(provider));

            return handler!;
        }
    }
}