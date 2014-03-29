namespace Smartflow.Core
{
    /// <summary>
    /// Copy from ASP.NET mvc
    /// </summary>
    public enum FilterScope
    {
        /// <summary>
        /// The filter with this scope will be executed first, normally it's used in Global filters
        /// </summary>
        First = 0,
        /// <summary>
        /// This scope is normally used in Global filter
        /// </summary>
        Global = 10,
        /// <summary>
        /// This scope is normally used in Attribute base filter for the Handler 
        /// </summary>
        Handler = 20,
        /// <summary>
        /// This scope is normally used in Attribute base filter for decrating on the Message itself
        /// </summary>
        Message = 40,
        /// <summary>
        /// This scope is normally used in Attribute base filter for the Handle method of the handlers
        /// </summary>
        Action = 80,
        /// <summary>
        /// The filter with this scope will be executed last, normally it's used in Global filters
        /// </summary>
        Last = 160,
    }
}