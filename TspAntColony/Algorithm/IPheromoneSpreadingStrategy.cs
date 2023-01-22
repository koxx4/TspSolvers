namespace TspAntColony.Algorithm;


public interface IPheromoneSpreadingStrategy
{
    double GetSpreadValue(long transitionCost, long currentAntPathCost);
}

public class QasSpreadingStrategy : IPheromoneSpreadingStrategy
{
    public double GetSpreadValue(long transitionCost, long currentAntPathCost)
    {
        return 10.0 / transitionCost;
    }
}

public class CasSpreadingStrategy : IPheromoneSpreadingStrategy
{
    public double GetSpreadValue(long transitionCost, long currentAntPathCost)
    {
        return 10.0 / currentAntPathCost;
    }
}

public class DasSpreadingStrategy : IPheromoneSpreadingStrategy
{
    public double GetSpreadValue(long transitionCost, long currentAntPathCost)
    {
        return 10;
    }
}