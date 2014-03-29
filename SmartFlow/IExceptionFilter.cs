
namespace Smartflow.Core
{
    /// <summary>
    /// Implement this interface to handle 
    /// </summary>
    public interface IExceptionFilter : IFilter
    {
        /// <summary>
        /// Handle the exception context
        /// </summary>
        /// <param name="exceptionContext"></param>
        void OnException(ExceptionContext exceptionContext);
    }
}