namespace TspUtils.Configuration;

public class ConfigurationLine
{
    public string FileName { get; }
    public int AlgorithmPassCount { get; }
    public int OptimalWeight { get; }
    public int[] OptimalCycle { get; }

    public ConfigurationLine(string fileName, int algorithmPassCount, int optimalWeight, int[] optimalCycle)
    {
        FileName = fileName;
        AlgorithmPassCount = algorithmPassCount;
        OptimalWeight = optimalWeight;
        OptimalCycle = optimalCycle;
    }
}

public abstract class ConfigurationData<T> where T : ConfigurationLine
{
    public List<T> ConfigurationLines { get; }

    private ConfigurationData(List<T> configurationLines)
    {
        ConfigurationLines = configurationLines;
    }

    protected abstract T ParseConfigurationLine(string line);

    protected bool IsNotComment(string configurationLine)
    {
        return !configurationLine.TrimStart().StartsWith('#');
    }
}