namespace Smartflow.Core.CQRS
{
    /// <summary>
    /// Implement this abstract class to handle events and commands or anything implement IMessage
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MessageHandler<T> : IHandler<T> where T : class, IMessage
    {
        /// <summary>
        /// Handle the message
        /// </summary>
        /// <param name="message"></param>
        public abstract void Handle(T message);

        /// <summary>
        /// The handler context
        /// </summary>
        public HandlerContext<T> Context { get; set; }

        /// <summary>
        /// This method is called before message is executed
        /// </summary>
        public virtual void OnMessageExecuting()
        {
        }

        /// <summary>
        /// This method is called after message is executed
        /// </summary>
        public virtual void OnMessageExecuted()
        {
        }
    }
}