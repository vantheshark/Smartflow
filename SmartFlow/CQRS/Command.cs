
namespace Smartflow.Core.CQRS
{
    /// <summary>
    /// A command sender should send a Command to only 1 target which can handle it
    /// In other words, a command should be handled by only 1 handler
    /// </summary>
    public abstract class Command : IMessage
    {
        /// <summary>
        /// A command should have a priority to be processed. The higher priority it is, the faster the command will be processed
        /// </summary>
        public uint Priority { get; set; }
    }
}
