namespace SchedulerDemo.Scheduler
{
    public interface IJobTrigger
    {
        bool Begin();
        bool Repeat();
    }
}
