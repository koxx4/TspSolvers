using System.Text;
using TspUtils;

namespace TspSimulatedAnnealingSolver.Configuration;

public static class SaConfigurationBasedFilenameBuilder
{
    private static readonly char JOIN_CHAR = '-';
    
    public static string BuildName(SaConfigurationLine saConfigurationLine, MatrixData matrixData, string extension, bool includeInstanceName = true)
    {
        StringBuilder stringBuilder = new StringBuilder();

        string instanceNameWithoutExtension = "";
        if (includeInstanceName)
        {
            instanceNameWithoutExtension = saConfigurationLine.FileName.Remove(saConfigurationLine.FileName.IndexOf('.'));
        }
        
        string coolingScheduleName = saConfigurationLine.CoolingSchedule.ToString().ToUpper();
        string neighbourGenerationMethodName = saConfigurationLine.NeighbourGenerationMethod.ToString().ToUpper();
        string localSearchMethodName = saConfigurationLine.NeighbourTraversingMethod.ToString().ToUpper();
        string ageLength = Convert.ToString(saConfigurationLine.AgeLifespan);

        return stringBuilder
            .Append(instanceNameWithoutExtension)
            .Append(instanceNameWithoutExtension.Length == 0 ? ' ' : JOIN_CHAR)
            .Append(coolingScheduleName)
            .Append(JOIN_CHAR)
            .Append(neighbourGenerationMethodName)
            .Append(JOIN_CHAR)
            .Append(localSearchMethodName)
            .Append(JOIN_CHAR)
            .Append(ageLength)
            .Append('.')
            .Append(extension)
            .ToString()
            .Trim();
    } 
}