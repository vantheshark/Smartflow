using System;
using Smartflow.Core.CQRS;

namespace Smartflow.Demo.RssSearch
{
    public enum FeedType
    {
        Rss,
        Atom
    }

    public class FeedSearchCommand : Command, IDistributedMessage
    {
        public MonitoringFeed Feed { get; set; }
    }

    /// <summary>
    /// Dto class contain information about a RSS feed we want to monitor
    /// </summary>
    public class MonitoringFeed
    {
        public Guid Id { get; set; }
        public FeedType Type { get; set; }
        public string Url { get; set; }
    }
}
