using System;
using System.Configuration;



namespace WindowsService.Scheduling.Configuration
{
    public class IntervalElement : ConfigurationElement
    {
        [ConfigurationProperty("interval", IsRequired = true, DefaultValue = "00:00:00.100")]
        [TimeSpanValidatorAttribute(MinValueString = "00:00:00.100")]
        public TimeSpan Interval
        {
            get { return (TimeSpan)this["interval"]; }
        }
    }
}
