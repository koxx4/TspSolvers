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

    protected ConfigurationData(List<T> configurationLines)
    {
        ConfigurationLines = configurationLines;
    }

    private ConfigurationData()
    {
        ConfigurationLines = new List<T>();
    }
}