using Smartflow.Core.Tasks;

namespace Smartflow.Core
{
    /// <summary>
    /// This class contains global settings shared within the library
    /// </summary>
    public static class Local
    {
        /// <summary>
        /// Return pending task count for current system;
        /// </summary>
        public static int PendingTaskCount
        {
            get { return PriorityTask.PendingTaskCount; }
        }
    }
}
