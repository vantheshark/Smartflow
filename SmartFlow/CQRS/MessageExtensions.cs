
using Smartflow.Core.Tasks;

namespace Smartflow.Core.CQRS
{
    /// <summary>
    /// This class provides some extension methods for the IMessage object
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        /// Change the priority of current message to be max value so it can be executed asap
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static T WithHighestPriority<T>(this T msg) where T : IMessage
        {
            msg.Priority = (uint)MessagePriority.Highest;
            return msg;
        }
    }

    /// <summary>
    /// This class provides some extension methods for the IMessage object
    /// </summary>
    public static class CommandSenderExtensions
    {
        /// <summary>
        /// Send the query command and wait for result synchronously
        /// </summary>
        /// <returns></returns>
        public static T WaitForResult<T>(this ICommandSender commandSender, Query<T> query)
        {
            var task = commandSender.Query(query);
            task.Wait();

            if (task.Exception != null)
            {
                if (task.Exception.InnerExceptions.Count == 1)
                {
                    throw task.Exception.InnerExceptions[0];
                }
                throw task.Exception;
            }
            return task.Result;
        }
    }
}
