namespace TspSimulatedAnnealingSolver.Algorithm.Temperature;

public interface ITemperatureStrategy
{
    double GenerateNewTemperature(double oldTemperature, double a, double b, double k);
}

public class LogarithmicTemperatureStrategy : ITemperatureStrategy
{
    public double GenerateNewTemperature(double oldTemperature, double a, double b, double k)
    {
        return oldTemperature / (1 + b * Math.Log(1 + k));
    }
}

public class CauchyTemperatureStrategy : ITemperatureStrategy
{
    public double GenerateNewTemperature(double oldTemperature, double a, double b, double k)
    {
        return oldTemperature / (a + b * k);
    }
}

public class GeometricTemperatureStrategy : ITemperatureStrategy
{
    public double GenerateNewTemperature(double oldTemperature, double a, double b, double k)
    {
        return Math.Pow(a, k) * oldTemperature;
    }
}

public class LinearTemperatureStrategy : ITemperatureStrategy
{
    public double GenerateNewTemperature(double oldTemperature, double a, double b, double k)
    {
        return a * oldTemperature - b;
    }
}