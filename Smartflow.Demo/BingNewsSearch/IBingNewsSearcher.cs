
using System.Threading.Tasks;

namespace Smartflow.Demo.BingNewsSearch
{
    public interface IBingNewsSearcher
    {
        Task<BingNewsSearchPage> SearchAsync(string searchKeyword);
    }
}
