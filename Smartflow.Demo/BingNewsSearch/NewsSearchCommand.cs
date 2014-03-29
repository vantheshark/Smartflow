using System;
using Smartflow.Core.CQRS;

namespace Smartflow.Demo.BingNewsSearch
{
    public class NewsSearchCommand : Command
    {
        public string Query { get; set; }
        public DateTime LatestArticleDate { get; set; }
    }
}
