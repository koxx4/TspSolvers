using System.Globalization;
using System.Text;
using TspAntColony.Configuration;
using TspUtils;

namespace TspSimulatedAnnealingSolver.Configuration;

public static class AcConfigurationBasedFilenameBuilder
{
    private static readonly char JOIN_CHAR = '-';
    
    public static string BuildName(AcConfigurationDataLine acConfigurationDataLine, MatrixData matrixData, string extension, bool includeInstanceName = true)
    {
        StringBuilder stringBuilder = new StringBuilder();

        string instanceNameWithoutExtension = "";
        if (includeInstanceName)
        {
            instanceNameWithoutExtension = acConfigurationDataLine.FileName.Remove(acConfigurationDataLine.FileName.IndexOf('.'));
        }
        
        string pheromoneSpreadType = acConfigurationDataLine.PheromoneSpreadingStrategy.ToString().ToUpper();
        string alpha = acConfigurationDataLine.Alpha.ToString(CultureInfo.InvariantCulture).ToUpper();
        string beta = acConfigurationDataLine.Beta.ToString(CultureInfo.InvariantCulture).ToUpper();

        return stringBuilder
            .Append(instanceNameWithoutExtension)
            .Append(instanceNameWithoutExtension.Length == 0 ? ' ' : JOIN_CHAR)
            .Append(pheromoneSpreadType)
            .Append(JOIN_CHAR)
            .Append($"A_{alpha}")
            .Append(JOIN_CHAR)
            .Append($"B_{beta}")
            .Append('.')
            .Append(extension)
            .ToString()
            .Trim();
    } 
}