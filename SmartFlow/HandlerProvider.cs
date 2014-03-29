namespace Smartflow.Core
{
    /// <summary>
    /// Provide ways to register and resolve handlers
    /// </summary>
    public static class HandlerProvider
    {
        static HandlerProvider()
        {
            Providers = new HandlerProviderCollection();
            Providers.Add(new DefaultHandlerProvider());
            Providers.Add(new DependencyResolverHandlerProvider());
        }

        /// <summary>
        /// All current providers
        /// </summary>
        public static HandlerProviderCollection Providers { get; set; }
    }
}