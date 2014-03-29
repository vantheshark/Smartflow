namespace Smartflow.Core
{
    /// <summary>
    /// Copy from System.Web.Mvc
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Whether allow multiple filter of the same type
        /// </summary>
        bool AllowMultiple { get; }

        /// <summary>
        /// The order of the filter
        /// </summary>
        int Order { get; }
    }
}