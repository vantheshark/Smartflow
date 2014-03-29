using System;

namespace Smartflow.Core
{
    /// <summary>
    /// The class that provide access to global information like the total polling messages which can be overwritten by Core.RabbitMQ module
    /// </summary>
    public static class Global
    {
        static Global()
        {
            PendingJobCount = () => (uint)Local.PendingTaskCount;
        }

        /// <summary>
        /// Get the total queueing jobs
        /// </summary>
        public static Func<uint> PendingJobCount;
    }
}