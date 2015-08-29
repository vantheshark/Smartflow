using System;
using Smartflow.Core.CQRS;

namespace Smartflow.Demo.Common
{
    public class NewsSearchCommand : Command, IDistributedMessage
    {
        public string Query { get; set; }
        public DateTime LatestArticleDate { get; set; }
    }
}
