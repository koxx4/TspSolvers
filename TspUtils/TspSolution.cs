using System.Diagnostics;

namespace TspUtils;

public class TspSolution
{
    public int MinPathWeight { get; }
    public List<int> MinPath { get; }
    public TimeSpan ExecutionTime { get; }
    public long BytesUsed { get; }
    
    public TspSolution(int minPathWeight, List<int> minPath)
    {
        MinPathWeight = minPathWeight;
        MinPath = minPath;
    }
    
    public TspSolution(int minPathWeight, List<int> minPath, TimeSpan executionTime)
    {
        MinPathWeight = minPathWeight;
        MinPath = minPath;
        ExecutionTime = executionTime;
    }
    
    public TspSolution(int minPathWeight, List<int> minPath, TimeSpan executionTime, long bytesUsed)
    {
        MinPathWeight = minPathWeight;
        MinPath = minPath;
        ExecutionTime = executionTime;
        BytesUsed = bytesUsed;
    }

    public override string ToString()
    {
        return $"{MinPathWeight} {string.Join(' ' , MinPath)}";
    }
}