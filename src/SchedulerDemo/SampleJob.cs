using System;
using SchedulerDemo.Scheduler;

namespace SchedulerDemo
{
    internal class SampleJob : IJob
    {
        private readonly int _id;

        public SampleJob(int id)
        {
            _id = id;
        }

        public void Execute()
        {
            Console.WriteLine($"Hello{_id}");
        }
    }
}
