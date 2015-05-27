using System;



namespace WindowsService.Scheduling.Settings
{
    public interface IWorkerSettings
    {
        TimeSpan FailureInterval { get; }
        ILoadingInterval[] LoadingIntervals { get; }
    }
}
