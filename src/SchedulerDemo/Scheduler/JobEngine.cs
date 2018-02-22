using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulerDemo.Scheduler
{
    public class JobEngine
    {
        private class JobSchedule
        {
            public int JobId { get; set; }
            public IJob Job { get; set; }
            public IJobTrigger JobTrigger { get; set; }
            public bool ToRemove { get; set; }
            public bool IsRunning { get; set; }
        }

        private readonly BackgroundWorker _engineWorker;
        private readonly List<JobSchedule> _jobsToRunList;
        private readonly List<int> _jobsToRemoveList;
        private readonly object _addLock = new object();
        private readonly object _removeLock = new object();
        private int _jobId;

        public JobEngine()
        {
            _jobsToRemoveList = new List<int>();
            _jobsToRunList = new List<JobSchedule>();
            _engineWorker = new BackgroundWorker();
            _engineWorker.WorkerSupportsCancellation = true;
            _engineWorker.DoWork += doEngineLoop;
        }

        public void StartEngine()
        {
            _engineWorker.RunWorkerAsync();
        }

        public void StopEngine()
        {
            _engineWorker.CancelAsync();
        }

        public int AddJob(IJob job, IJobTrigger jobTrigger)
        {
            lock (_addLock)
            {
                ++_jobId;
                var jobSchedule = new JobSchedule()
                {
                    JobId = _jobId,
                    Job = job,
                    JobTrigger = jobTrigger
                };
                _jobsToRunList.Add(jobSchedule);
                return _jobId;
            }
        }

        public void RemoveJob(int jobId)
        {
            lock (_removeLock)
            {
                _jobsToRemoveList.Add(jobId);
            }
        }

        private void doEngineLoop(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Name = "JobEngine";

            while (!e.Cancel)
            {
                JobSchedule[] array = _jobsToRunList.ToArray();
                foreach (JobSchedule item in array)
                {
                    if (e.Cancel)
                    {
                        break;
                    }
                    if (_jobsToRemoveList.Contains(item.JobId))
                    {
                        item.ToRemove = true;
                    }
                    else if (!item.IsRunning && item.JobTrigger.Begin())
                    {
                        item.IsRunning = true;
                        Task t = new Task(() =>
                        {
                            try
                            {
                                item.Job.Execute();
                            }
                            finally
                            {
                                item.ToRemove = !item.JobTrigger.Repeat();
                                item.IsRunning = false;
                            }
                        });
                        t.Start();
                    }
                }

                //cleanup
                lock (_removeLock)
                {
                    _jobsToRunList.RemoveAll(j => j.ToRemove || _jobsToRemoveList.Contains(j.JobId));
                    _jobsToRemoveList.Clear();
                }
            }
        }
    }
}
