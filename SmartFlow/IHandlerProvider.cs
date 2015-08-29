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
        /// Get all handlers for message T
        /// </summary>
        /// <returns></returns>
        IEnumerable<ICommandHandler<T>> GetCommandHandlers<T>() where T : Command;

        /// <summary>
        /// Get all async handler for message T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<IAsyncCommandHandler<T>> GetAsyncCommandHandlers<T>() where T : Command;

        /// <summary>
        /// Get all handlers for message T
        /// </summary>
        /// <returns></returns>
        IEnumerable<IEventHandler<T>> GetEventHandlers<T>(Type eventType) where T : Event;

        /// <summary>
        /// Get all async handler for message T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<IAsyncEventHandler<T>> GetAsyncEventHandlers<T>(Type eventType) where T : Event;

        /// <summary>
        /// Register a command handler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        void RegisterCommandHandler<T>(ICommandHandler<T> handler) where T : Command;


        /// <summary>
        /// Register an async command handler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        void RegisterCommandHandler<T>(IAsyncCommandHandler<T> handler) where T : Command;

        /// <summary>
        /// Register an event handler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        void RegisterEventHandler<T>(IEventHandler<T> handler) where T : Event;


        /// <summary>
        /// Register an async event handler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        void RegisterEventHandler<T>(IAsyncEventHandler<T> handler) where T : Event;
    }
}