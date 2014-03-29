
namespace Smartflow.Core.CQRS
{
    /// <summary>
    /// Implement this class as a message object which contains information for a handler to handle it
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// A message should have a priority to be processed. The higher priority it is, the faster the message will be processed
        /// </summary>
        uint Priority { get; set; }
    }
}
