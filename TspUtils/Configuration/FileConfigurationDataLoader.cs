namespace TspUtils.Configuration;

public abstract class FileConfigurationDataLoader<T, TC> : IConfigurationDataLoader<T, TC>
    where T : ConfigurationLine
    where TC : ConfigurationData<T>
{
    protected string _configurationFilePath;
    
    protected FileConfigurationDataLoader(string path)
    {
        _configurationFilePath = path;
    }
    
    public virtual TC LoadConfiguration()
    {
        return ReadFromFile();
    }

    private TC ReadFromFile()
    {
        string[] fileLines = File.ReadAllLines(_configurationFilePath)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();

        return ParseFileLines(fileLines);
    }

    protected abstract TC ParseFileLines(string[] fileLines);
}