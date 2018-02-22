using System;
using System.Threading;
using System.Threading.Tasks;
using SchedulerDemo.Scheduler;

namespace SchedulerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            JobEngine engine = new JobEngine();
            engine.StartEngine();

            Task.Run(() =>
            {
                Console.WriteLine("Add first job");
                int job1 = engine.AddJob(new SampleJob(1), new SampleTrigger());

                Console.WriteLine("Add second job");
                int job2 = engine.AddJob(new SampleJob(2), new SampleTrigger());

                Thread.Sleep(200);
                engine.RemoveJob(job2);

                Thread.Sleep(200);
                engine.RemoveJob(job1);
            });

            Console.WriteLine("Press enter to end");
            Console.ReadLine();
            engine.StopEngine();
        }
    }
}
