using System.Diagnostics;
using TspSimulatedAnnealingSolver.Algorithm.Neighbourhood;
using TspSimulatedAnnealingSolver.Algorithm.Temperature;
using TspSimulatedAnnealingSolver.Configuration;
using TspUtils;

namespace TspSimulatedAnnealingSolver.Algorithm;

public class SaTspSolver
{
    private const bool LOG_PROGRESS = false;
    private const double A = 0.99999;
    private const double B = 1;
    private readonly Stopwatch stopwatch = new();
    private const int FAIL_SAFE_MINUTES_VALUE = 5;

    private readonly Random _random;
    private readonly MatrixData _matrixData;
    private readonly int _startingVertex;
    private readonly double _startingTemperature;
    private readonly int _ageLength;
    private int _ageNumber;
    private double _currentTemperature;
    private int[] _firstPath;
    private int[] _currentBestPath;
    private int _currentBestPathWeight;
    private readonly ITemperatureStrategy _temperatureStrategy;
    private readonly INeighbourGenerationStrategy _neighbourGenerationStrategy;

    public SaTspSolver(
        MatrixData matrixData,
        int startingVertex,
        CoolingSchedule coolingSchedule,
        NeighbourGenerationMethod neighbourGenerationMethod,
        int ageLength,
        int startingTemperature)
    {
        _random = new Random();
        _matrixData = matrixData;
        _startingVertex = startingVertex;
        
        _temperatureStrategy = ChooseTemperatureStrategy(coolingSchedule);
        _neighbourGenerationStrategy = ChooseNeighbourGenerationStrategy(neighbourGenerationMethod);
        _ageLength = ageLength;
        _ageNumber = 1;

        //_startingTemperature = matrixData.NumberOfVertices * 3;
        _startingTemperature = startingTemperature;
        _currentTemperature = _startingTemperature;
        _firstPath = GenerateFirstPath(matrixData.NumberOfVertices, startingVertex);
        _currentBestPath = new List<int>(_firstPath).ToArray();
        _currentBestPathWeight = CalculatePathWeightOfCompletePath(_currentBestPath);
    }

    public TspSolution Solve()
    {
        stopwatch.Reset();
        stopwatch.Start();
        DoSolve();
        stopwatch.Stop();

        return new TspSolution(_currentBestPathWeight, _currentBestPath.Append(_startingVertex).Prepend(_startingVertex).ToList(), stopwatch.Elapsed);
    }

    private void DoSolve()
    {
        while (_currentTemperature > 1.0)
        {
            if (stopwatch.Elapsed.TotalMinutes >= FAIL_SAFE_MINUTES_VALUE)
            {
                throw new AlgorithmTimeoutException(
                    $"TIMEOUT! ABORTING AFTER {stopwatch.Elapsed.TotalMinutes}m{stopwatch.Elapsed.Seconds}s! Vertices {_matrixData.NumberOfVertices}.");
            }
            
            if (LOG_PROGRESS)
            {
                Console.Out.WriteAsync($"Temperature: {_currentTemperature, 30}, Age: {_ageNumber, 30}, {_currentBestPathWeight, 20}, \r");
            }
            
            for (int i = 0; i < _ageLength; i++)
            {
                int[] nextSolution = _neighbourGenerationStrategy.GenerateNextNeighbour(_currentBestPath);
                int nextSolutionCost = CalculatePathWeightOfCompletePath(nextSolution);

                int delta = nextSolutionCost - _currentBestPathWeight;

                if (delta < 0)
                {
                    _currentBestPath = nextSolution;
                    _currentBestPathWeight = nextSolutionCost;
                    
                    continue;
                }

                if (!ShouldAcceptWorsePath(delta))
                    continue;
                
                _currentBestPath = nextSolution;
                _currentBestPathWeight = nextSolutionCost;
            }

            _currentTemperature = _temperatureStrategy
                .GenerateNewTemperature(_currentTemperature, A, B, _ageNumber);

            _ageNumber++;
        }
    }
    
    private bool ShouldAcceptWorsePath(int delta)
    {
        double s = _random.NextDouble();

        return s < Math.Exp(-(delta / _currentTemperature));
    }

    private int CalculatePathWeightOfCompletePath(int[] path)
    {
        int[,] adjacencyMatrix = _matrixData.AdjacencyMatrixArray;
        
        int sum = adjacencyMatrix[path[0], _startingVertex];
        
        for (int i = 0; i < path.Length - 1; i++)
        {
            sum += adjacencyMatrix[path[i + 1], path[i]];
        }
        
        sum += adjacencyMatrix[_startingVertex, path[^1]];

        return sum;
    }
    
    private static int[] GenerateFirstPath(int numberOfVertices, int startingVertex)
    {
        return RandomPermutation(
            Enumerable.Range(0, numberOfVertices)
                .Where(x => x != startingVertex)
                .ToArray());
    }
    
    private static int[] RandomPermutation(int[] input)
    {
        Random rng = new Random();
        int[] output = input.ToArray();
        for (int i = 0; i < output.Length - 1; i++)
        {
            int j = rng.Next(i, output.Length);
            (output[i], output[j]) = (output[j], output[i]);
        }
        return output;
    }

    private static ITemperatureStrategy ChooseTemperatureStrategy(CoolingSchedule coolingSchedule)
    {
        ITemperatureStrategy temperatureStrategy = coolingSchedule switch
        {
            CoolingSchedule.LOGARITHMIC => new LogarithmicTemperatureStrategy(),
            CoolingSchedule.CAUCHY => new CauchyTemperatureStrategy(),
            CoolingSchedule.LINEAR => new LinearTemperatureStrategy(),
            CoolingSchedule.GEOMETRIC => new GeometricTemperatureStrategy(),
            _ => throw new ArgumentOutOfRangeException(nameof(coolingSchedule), coolingSchedule, null)
        };

        return temperatureStrategy;
    }
    
    private static INeighbourGenerationStrategy ChooseNeighbourGenerationStrategy(NeighbourGenerationMethod neighbourGenerationMethod)
    {
        INeighbourGenerationStrategy neighbourGenerationStrategy = neighbourGenerationMethod switch
        {
            NeighbourGenerationMethod.TWO_SWAP => new TwoSwapNeighbourGenerationStrategy(),
            NeighbourGenerationMethod.TWO_SWAP_N => new TwoSwapNNeighbourGenerationStrategy(),
            NeighbourGenerationMethod.TWO_INSERT => new TwoInsertNeighbourGenerationStrategy(),
            NeighbourGenerationMethod.REVERSING => new ReverseNeighbourGenerationStrategy(),
            _ => throw new ArgumentOutOfRangeException(nameof(neighbourGenerationMethod), neighbourGenerationMethod, null)
        };

        return neighbourGenerationStrategy;
    }

    private string GetCurrentPathString()
    {
        return string.Join('-', _currentBestPath);
    }
}