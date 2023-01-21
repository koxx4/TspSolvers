using TspUtils.Configuration;

namespace TspSimulatedAnnealingSolver.Configuration;

public class SaFileConfigurationDataLoader : FileConfigurationDataLoader<SaConfigurationLine, SaConfigurationData>
{
    private const string LineValuesSeparator = ",";
    private const char OptimalPathSeparator = '-';
    private const int ExpectedArgsInLine = 8;
    
    public SaFileConfigurationDataLoader(string path) : base(path)
    {
    }

    protected override SaConfigurationData ParseFileLines(string[] fileLines)
    {
        List<SaConfigurationLine> configurationLines = fileLines
                .Where(IsNotComment)
                .Select(ParseConfigurationLine)
                .ToList();

            return new SaConfigurationData(configurationLines);
    }
    
    private SaConfigurationLine ParseConfigurationLine(string line)
    {
        string[] lineValues = ExtractParametersFromLine(line);

        if (lineValues.Length != ExpectedArgsInLine)
            throw new WrongConfigurationDataFormatException("Wrong configuration format for SA algorithm!");

        string fileName = lineValues[0];
        int algorithmPassCount = int.Parse(lineValues[1]);
        int optimalWeight = int.Parse(lineValues[2]);
        int[] optimalCycle = ParseOptimalCycle(lineValues[3]);
        CoolingSchedule coolingSchedule = (CoolingSchedule) Enum.Parse(typeof(CoolingSchedule), lineValues[4], true);
        NeighbourGenerationMethod neighbourGenerationMethod = (NeighbourGenerationMethod) Enum.Parse(typeof(NeighbourGenerationMethod), lineValues[5], true);
        NeighbourTraversingMethod neighbourTraversingMethod= (NeighbourTraversingMethod) Enum.Parse(typeof(NeighbourTraversingMethod), lineValues[6], true);
        int ageLength = int.Parse(lineValues[7]);

        return new SaConfigurationLine(
            fileName,
            algorithmPassCount,
            optimalWeight,
            coolingSchedule,
            neighbourGenerationMethod,
            neighbourTraversingMethod,
            ageLength,
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