SimpleJobScheduler, barebone version of a job scheduler engine.

Inspired by Quartz.NET that separates `IJobTrigger` from `IJob` interface.

Sample usage:

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

SampleJob.cs

    class SampleJob : IJob
    {
        public void Execute()
        {
            Console.WriteLine("Hello");
        }
    }
            
SampleJobTrigger.cs

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
