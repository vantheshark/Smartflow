using System;
using System.Collections.Generic;
using Smartflow.Core.CQRS;

namespace Smartflow.Core
{
    /// <summary>
    /// A provider to resolve all handlers for a message type
    /// </summary>
    public interface IHandlerProvider
    {
        /// <summary>
        /// Get all handlers for a message type
        /// </summary>
        /// <returns></returns>
        IEnumerable<IHandler<IMessage>> GetHandlers(Type messageType);
    }
}