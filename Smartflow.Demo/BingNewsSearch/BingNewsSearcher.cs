using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using log4net;
using Smartflow.Demo.Common;

namespace Smartflow.Demo.BingNewsSearch
{
    public class BingNewsSearcher : IBingNewsSearcher
    {
        private readonly ILog _logger;

        public BingNewsSearcher(ILog logger)
        {
            _logger = logger;
        }

        public async Task<BingNewsSearchPage> SearchAsync(string searchKeyword)
        {
            var pageUrl = BingNewsSearchUrlBuilder.BuildSearchUrl(searchKeyword);

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(pageUrl);
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.79 Safari/537.1";
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (var apiResponse = (HttpWebResponse) await request.GetResponseAsync().ConfigureAwait(false))
                using (var apiResponseStream = apiResponse.GetResponseStream())
                {
                    if (apiResponseStream == null)
                    {
                        return null;
                    }
                    using (var loResponseStream = new StreamReader(apiResponseStream))
                    {
                        var xmlData = await loResponseStream.ReadToEndAsync().ConfigureAwait(false);
                        var result = ParseResult(xmlData);
                        result.SearchedKeyword = searchKeyword;
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return null;
            }
        }

        private BingNewsSearchPage ParseResult(string xmlData)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(xmlData);
            var result = new List<SearchResult>();
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                var reader = XmlReader.Create(stream);
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                foreach (SyndicationItem item in feed.Items)
                {
                    result.Add(new SearchResult
                    {
                        Title = item.Title.Text,
                        Description = item.Summary.Text,
                        Link = item.Links.Count > 0 ? item.Links[0].Uri.ToString() : null,
                        PublishDate = item.PublishDate.Date
                    });
                }
            }
            return new BingNewsSearchPage
            {
                Data = result.ToArray()
            };
        }
    }
}
