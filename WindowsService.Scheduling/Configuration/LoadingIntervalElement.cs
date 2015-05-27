using System.Configuration;

using WindowsService.Scheduling.Entities;
using WindowsService.Scheduling.Settings;



namespace WindowsService.Scheduling.Configuration
{
    public class LoadingIntervalElement : IntervalElement, ILoadingInterval
    {
        [ConfigurationProperty("loading", IsRequired = true)]
        public Loading Loading { get { return (Loading)this["loading"]; } }
    }
}
