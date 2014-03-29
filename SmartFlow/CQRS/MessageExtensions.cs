
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
        public static T OnDemand<T>(this T msg) where T : IMessage
        {
            msg.Priority = (uint)MessagePriority.OnDemand;
            return msg;
        }
    }
}
