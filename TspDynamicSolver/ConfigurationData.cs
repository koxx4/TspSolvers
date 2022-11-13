namespace TspDynamicSolver;

internal class ConfigurationLine
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

internal class ConfigurationData
{
    public List<ConfigurationLine> ConfigurationLines { get; }

    private ConfigurationData(List<ConfigurationLine> configurationLines)
    {
        ConfigurationLines = configurationLines;
    }

    public static ConfigurationData? LoadFromFile(string filename)
    {
        try
        {
            string[] fileLines = File.ReadAllLines(filename);

            List<ConfigurationLine> configurationLines = fileLines
                    .Where(line => !line.StartsWith('#'))
                    .Select(ParseConfigurationLine)
                    .ToList();

            return new ConfigurationData(configurationLines);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
            return null;
        }
    }

    private static ConfigurationLine ParseConfigurationLine(string line)
    {
        string[] lineValues = line.Split(' ',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        string fileName = lineValues[0];

        int algorithmPassCount = int.Parse(lineValues[1]);

        int optimalWeight = int.Parse(lineValues[2]);

        int[] optimalCycle = lineValues[3]
            .Replace('[', ' ')
            .Replace(']', ' ')
            .Trim()
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(number => int.TryParse(number, out _))
            .Select(number => Convert.ToInt32(number))
            .ToArray();

        return new ConfigurationLine(fileName, algorithmPassCount, optimalWeight, optimalCycle);
    }
}
