using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smartflow.Core.Tasks
{
    internal class PriorityTaskComparer : IComparer<Task>
    {
        public int Compare(Task x, Task y)
        {
            var task1Priority = x is PriorityTask
                              ? (x as PriorityTask).Priority
                              : 0;

            var task1CreatedTime = x is PriorityTask
                                 ? (x as PriorityTask).CreatedTime
                                 : DateTime.MinValue;


            var task2Priority = y is PriorityTask
                              ? (y as PriorityTask).Priority
                              : 0;

            var task2CreatedTime = y is PriorityTask
                                 ? (y as PriorityTask).CreatedTime
                                 : DateTime.MinValue;

            if (task1Priority == task2Priority)
            {
                return task1CreatedTime < task2CreatedTime ? 1 : -1;
            }
            
            return (int)task1Priority - (int)task2Priority;
        }
    }
}
