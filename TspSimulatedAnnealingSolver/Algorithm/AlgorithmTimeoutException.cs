namespace TspSimulatedAnnealingSolver.Algorithm;

public class AlgorithmTimeoutException : Exception
{
    public AlgorithmTimeoutException(string? message) : base(message)
    {
    }
}