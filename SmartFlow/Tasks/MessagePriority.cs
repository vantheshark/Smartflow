
namespace Smartflow.Core.Tasks
{
    /// <summary>
    /// This enum is a logical priority in the system.
    /// The core project is expected to be working with any uint Priority number
    /// </summary>
    public enum MessagePriority : uint
    {
        /// <summary>
        /// Priority 0, lowest
        /// </summary>
        Zero = 0,

        /// <summary>
        /// Priority 1
        /// </summary>
        One = 1,

        /// <summary>
        /// Priority 2
        /// </summary>
        Two = 2,

        /// <summary>
        /// Priority 3
        /// </summary>
        Three = 3,

        /// <summary>
        /// Priority 4
        /// </summary>
        Four = 4,

        /// <summary>
        /// Priority 5
        /// </summary>
        Five = 5,

        /// <summary>
        /// Priority 6
        /// </summary>
        Six = 6,

        /// <summary>
        /// Priority 7
        /// </summary>
        Seven = 7,

        /// <summary>
        /// Priority 8
        /// </summary>
        Eight = 8,

        /// <summary>
        /// Priority 9
        /// </summary>
        Nine = 9,

        /// <summary>
        /// Priority 10
        /// </summary>
        Ten = 10,

        /// <summary>
        /// Highest priority, any message with this priority once scheduled with the InternalBus will be sent to the Scheduler to execute straight away
        /// </summary>
        Highest = uint.MaxValue
    }
}

