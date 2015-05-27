using System;
using System.Configuration;
using System.Linq;

using WindowsService.Scheduling.Settings;



namespace WindowsService.Scheduling.Configuration
{
    public class WorkerSettingsSection : ConfigurationSection, IWorkerSettings
    {
        [ConfigurationProperty("failureInterval")]
        private IntervalElement FailureIntervalElement { get { return (IntervalElement)this["failureInterval"]; } }

        [ConfigurationProperty("loadingIntervals")]
        private LoadingIntervalsElementCollection LoadingLoadingIntervalsCollection
        {
            get { return (LoadingIntervalsElementCollection)this["loadingIntervals"]; }
        }

        public TimeSpan FailureInterval { get { return FailureIntervalElement.Interval; } }
        public ILoadingInterval[] LoadingIntervals { get { return LoadingLoadingIntervalsCollection.Cast<ILoadingInterval>().ToArray(); } }
    }
}
