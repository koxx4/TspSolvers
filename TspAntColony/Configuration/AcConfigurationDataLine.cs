using TspUtils.Configuration;

namespace TspAntColony.Configuration;

public class AcConfigurationDataLine : ConfigurationLine
{
    public PheromoneSpreadingStrategy PheromoneSpreadingStrategy{ get;}
    public double Alpha { get; }
    public double Beta { get; }
    
    public AcConfigurationDataLine(
        PheromoneSpreadingStrategy pheromoneSpreadingStrategy,
        double alpha,
        double beta,
        string fileName,
        int algorithmPassCount,
        int optimalWeight,
        int[] optimalCycle)
        : base(fileName, algorithmPassCount, optimalWeight, optimalCycle)
    {
        this.PheromoneSpreadingStrategy = pheromoneSpreadingStrategy;
        this.Alpha = alpha;
        this.Beta = beta;
    }
}