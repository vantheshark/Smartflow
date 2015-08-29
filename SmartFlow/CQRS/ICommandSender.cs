using System.Threading.Tasks;

namespace Smartflow.Core.CQRS
{
    /// <summary>
    /// Implement this interface to send commands
    /// The default "InternalBus" implementation will be responsible for this as an internal memory bus
    /// </summary>
    public interface ICommandSender
    {
        /// <summary>
        /// Send a command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        void Send<T>(T command) where T : Command;

        /// <summary>
        /// Ask for data immediately
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<T> Query<T>(Query<T> query);
    }
}