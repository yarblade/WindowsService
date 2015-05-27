using System.Configuration;



namespace WindowsService.Scheduling.Configuration
{
    [ConfigurationCollection(typeof(LoadingIntervalElement), AddItemName = "loadingInterval")]
    public class LoadingIntervalsElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new LoadingIntervalElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LoadingIntervalElement)element).Loading;
        }
    }
}
