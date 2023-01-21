using System.Globalization;
using TspUtils.Configuration;

namespace TspAntColony.Configuration;

public class AcFileConfigurationDataLoader : FileConfigurationDataLoader<AcConfigurationDataLine, AcConfigurationData>
{
    private const string LineValuesSeparator = ",";
    private const char OptimalPathSeparator = '-';
    private const int ExpectedArgsInLine = 7;
    
    public AcFileConfigurationDataLoader(string path) : base(path)
    {
    }

    protected override AcConfigurationData ParseFileLines(string[] fileLines)
    {
        List<AcConfigurationDataLine> configurationLines = fileLines
            .Where(IsNotComment)
            .Select(ParseConfigurationLine)
            .ToList();

        return new AcConfigurationData(configurationLines);
    }

    private AcConfigurationDataLine ParseConfigurationLine(string line)
    {
        string[] lineValues = ExtractParametersFromLine(line);

        if (lineValues.Length != ExpectedArgsInLine)
            throw new WrongConfigurationDataFormatException(
                $"Zły format pliku konfiguracyjnego - linia konfiguracyjna powinna zawierać {ExpectedArgsInLine} argumentów");

        string fileName = lineValues[0];
        int algorithmPassCount = int.Parse(lineValues[1]);
        int optimalWeight = int.Parse(lineValues[2]);
        int[] optimalCycle = ParseOptimalCycle(lineValues[3]);
        PheromoneSpreadingStrategy pheromoneSpreadingStrategy = (PheromoneSpreadingStrategy) Enum.Parse(typeof(PheromoneSpreadingStrategy), lineValues[4], true);
        double alpha = Convert.ToDouble(lineValues[5], CultureInfo.InvariantCulture);
        double beta = Convert.ToDouble(lineValues[6], CultureInfo.InvariantCulture);

        return new AcConfigurationDataLine(
            pheromoneSpreadingStrategy,
            alpha,
            beta,
            fileName,
            algorithmPassCount,
            optimalWeight,
            optimalCycle
        );
    }
    
    private bool IsNotComment(string configurationLine)
    {
        return !configurationLine.TrimStart().StartsWith('#');
    }

    private string[] ExtractParametersFromLine(string line)
    {
        return line.Split(LineValuesSeparator,
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private int[] ParseOptimalCycle(string line)
    {
        return line
            .Replace('[', ' ')
            .Replace(']', ' ')
            .Trim()
            .Split(OptimalPathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(number => int.TryParse(number, out _))
            .Select(number => Convert.ToInt32(number))
            .ToArray();
    }
}