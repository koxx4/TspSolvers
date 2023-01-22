using TspUtils.Configuration;

namespace TspSimulatedAnnealingSolver.Configuration;

public class SaConfigurationLine : ConfigurationLine
{
    public CoolingSchedule CoolingSchedule { get; }
    public NeighbourGenerationMethod NeighbourGenerationMethod { get; }
    public NeighbourTraversingMethod NeighbourTraversingMethod { get; }
    public int AgeLifespan { get; }

    public SaConfigurationLine(
        string fileName,
        int algorithmPassCount,
        int optimalWeight,
        CoolingSchedule coolingSchedule,
        NeighbourGenerationMethod neighbourGenerationMethod,
        NeighbourTraversingMethod neighbourTraversingMethod,
        int ageLifespan,
        int[] optimalCycle) : base(fileName, algorithmPassCount, optimalWeight, optimalCycle)
    {
        CoolingSchedule = coolingSchedule;
        NeighbourGenerationMethod = neighbourGenerationMethod;
        NeighbourTraversingMethod = neighbourTraversingMethod;
        AgeLifespan = ageLifespan;
    }
}