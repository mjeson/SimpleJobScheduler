using SchedulerDemo.Scheduler;

namespace SchedulerDemo
{
    internal class SampleTrigger : IJobTrigger
    {
        private int _runFor = 10;

        public bool Begin()
        {
            return true; //immediately
        }

        public bool Repeat()
        {
            return (0 < --_runFor);
        }
    }
}
