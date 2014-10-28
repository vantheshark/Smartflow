using System;

namespace Smartflow.Core.CQRS
{
    /// <summary>
    /// A special command that have a result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Query<T> : Command
    {
        /// <summary>
        /// The result
        /// </summary>
        public T Result { get; set; }
    }
}