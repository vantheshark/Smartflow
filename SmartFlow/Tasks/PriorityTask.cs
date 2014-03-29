using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Smartflow.Core.Tasks
{
    /// <summary>
    /// A custom Task implement which can be scheduled to run by its priority using <see cref="LimitedConcurrencyLevelTaskScheduler"/>
    /// </summary>
    public class PriorityTask : Task
    {
        /// <summary>
        /// Overwrite the Factory of the TPL Task class as there was not way to change the Task.Factory value
        /// </summary>
        public new static TaskFactory Factory { get; private set; }

        /// <summary>
        /// Synchronization is not really neccessary here
        /// </summary>
        public static int PendingTaskCount
        {
            get
            {
                var limitedConcurrencyLevelTaskScheduler = Factory.Scheduler as LimitedConcurrencyLevelTaskScheduler;
                return limitedConcurrencyLevelTaskScheduler != null
                                                            ? limitedConcurrencyLevelTaskScheduler.GetPendingTaskCount()
                                                            : 0;
            }
        }

        /// <summary>
        /// This static constructor will set the Factory by a Factory object with LimitedConcurrencyLevelTaskScheduler
        /// </summary>
        static PriorityTask()
        {
            var concurrencyLevel = int.Parse(ConfigurationManager.AppSettings["TaskConcurrencyLevel"] ?? "4");
            Factory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(concurrencyLevel));
        }

        /// <summary>
        /// The priority of the message
        /// </summary>
        public uint Priority { get; set; }

        /// <summary>
        /// The age of the message
        /// </summary>
        internal DateTime CreatedTime { get; private set; }

        /// <summary>
        /// To let the LimitedConcurrencyLevelTaskScheduler decide to execute this thread straight away if it's true
        /// </summary>
        internal bool OnDemand { get; set; }

        #region -- Constructors crap of the base class --
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public PriorityTask(Action action)
            : base(action)
        {
            CreatedTime = DateTime.UtcNow;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        public PriorityTask(Action action, CancellationToken cancellationToken)
            : base(action, cancellationToken)
        {
            CreatedTime = DateTime.UtcNow;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="creationOptions"></param>
        public PriorityTask(Action action, TaskCreationOptions creationOptions)
            : base(action, creationOptions)
        {
            CreatedTime = DateTime.UtcNow;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="creationOptions"></param>
        public PriorityTask(Action action, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
            : base(action, cancellationToken, creationOptions)
        {
            CreatedTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        public PriorityTask(Action<object> action, object state)
            : base(action, state)
        {
            CreatedTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        /// <param name="cancellationToken"></param>
        public PriorityTask(Action<object> action, object state, CancellationToken cancellationToken)
            : base(action, state, cancellationToken)
        {
            CreatedTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        /// <param name="creationOptions"></param>
        public PriorityTask(Action<object> action, object state, TaskCreationOptions creationOptions)
            : base(action, state, creationOptions)
        {
            CreatedTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="creationOptions"></param>
        public PriorityTask(Action<object> action, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
            : base(action, state, cancellationToken, creationOptions)
        {
            CreatedTime = DateTime.UtcNow;
        } 
        #endregion
    }
}