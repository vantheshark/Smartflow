namespace Smartflow.Core
{
    /// <summary>
    /// A static class to get all filter providers
    /// </summary>
    public static class FilterProviders
    {
        static FilterProviders()
        {
            Providers = new FilterProviderCollection();
            Providers.Add(GlobalFilters.Filters);
            Providers.Add(new FilterAttributeFilterProvider());
        }

        /// <summary>
        /// Get all registered filter providers
        /// </summary>
        public static FilterProviderCollection Providers { get; private set; }
    }
}