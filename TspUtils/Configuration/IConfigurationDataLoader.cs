namespace TspUtils.Configuration;

public interface IConfigurationDataLoader<T, TC> 
    where T : ConfigurationLine
    where TC : ConfigurationData<T>
{
    TC LoadConfiguration();
}