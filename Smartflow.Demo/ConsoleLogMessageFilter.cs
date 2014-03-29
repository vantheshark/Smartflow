using System;
using System.Diagnostics;
using Smartflow.Core;
using Smartflow.Demo.BingNewsSearch;

namespace Smartflow.Demo
{
    public class ConsoleLogMessageFilter : IMessageFilter
    {
        public bool AllowMultiple
        {
            get { return false; }
        }

        public int Order
        {
            get { return int.MaxValue; }
        }

        public void OnMessageExecuting(HandlerContext context)
        {
            Console.WriteLine("Processing " + context.MessageType.Name + " ...");
        }

        public void OnMessageExecuted(HandlerContext context)
        {
            Console.WriteLine("Finished " + context.MessageType.Name);
        }
    }


    public class HandlerPerformanceLogFilterAttribute : FilterAttribute, IMessageFilter<NewsSearchCommand>
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public void OnMessageExecuting(HandlerContext<NewsSearchCommand> context)
        {
            _stopwatch.Start();
        }

        public void OnMessageExecuted(MessageHandledContext<NewsSearchCommand> context)
        {
            _stopwatch.Stop();
            Console.WriteLine(string.Format("Search {0} in {1:##}s", context.Message.Query, _stopwatch.Elapsed.TotalSeconds));
        }
    }
}
