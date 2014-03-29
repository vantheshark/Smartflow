namespace Smartflow.Core
{
    /// <summary>
    /// The global filters collection which allow registration of filters at global scope
    /// </summary>
    public static class GlobalFilters
    {
        static GlobalFilters()
        {
            Filters = new GlobalFilterCollection();
        }

        /// <summary>
        /// The global fileter collection
        /// </summary>
        public static GlobalFilterCollection Filters { get; private set; }
    }
}