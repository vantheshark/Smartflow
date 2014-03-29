using System.Web;

namespace Smartflow.Demo.BingNewsSearch
{
    public class BingNewsSearchUrlBuilder
    {
        public static string BuildSearchUrl(string keyword)
        {
            var query = HttpUtility.UrlEncode(keyword);
            return string.Format("http://www.bing.com/news/search?q={0}&go=&qs=n&form=NWBQBN&pq=mh370&sc=8-4&sp=-1&sk=&format=RSS", query);
        }
    }
}
