using System;

using WindowsService.Scheduling.Entities;



namespace WindowsService.Scheduling.Settings
{
    public interface ILoadingInterval
    {
        Loading Loading { get; }
        TimeSpan Interval { get; }
    }
}
